﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blazor.Fluxor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.Fluxor
{
	/// <see cref="IStore"/>
	public class Store : IStore
	{
		/// <see cref="IStore.Features"/>
		public IReadOnlyDictionary<string, IFeature> Features => FeaturesByName;
		/// <see cref="IStore.Initialized"/>
		public Task Initialized => InitializedCompletionSource.Task;

		private IBrowserInteropService BrowserInteropService;
		private readonly Dictionary<string, IFeature> FeaturesByName = new Dictionary<string, IFeature>(StringComparer.InvariantCultureIgnoreCase);
		private readonly List<IEffectFuncs> EffectFuncs = new List<IEffectFuncs>();
		private readonly List<IMiddleware> Middlewares = new List<IMiddleware>();
		private readonly List<IMiddleware> ReversedMiddlewares = new List<IMiddleware>();
		private readonly Queue<object> QueuedActions = new Queue<object>();
		private readonly TaskCompletionSource<bool> InitializedCompletionSource = new TaskCompletionSource<bool>();

		private int BeginMiddlewareChangeCount;
		private bool HasActivatedStore;
		private bool IsInsideMiddlewareChange => BeginMiddlewareChangeCount > 0;
		private Action<IFeature, object> NotifyFeatureOfDispatch;

		/// <summary>
		/// Creates an instance of the store
		/// </summary>
		/// <param name="browserInteropService">The BrowserInteropService the Browser will use to initialise the store</param>
		public Store(IBrowserInteropService browserInteropService)
		{
			MethodInfo dispatchNotifictionFromStoreMethodInfo =
				typeof(IFeature)
				.GetMethod(nameof(IFeature.ReceiveDispatchNotificationFromStore));
			NotifyFeatureOfDispatch = (Action<IFeature, object>)
				Delegate.CreateDelegate(typeof(Action<IFeature, object>), dispatchNotifictionFromStoreMethodInfo);

			BrowserInteropService = browserInteropService;
			BrowserInteropService.PageLoaded += OnPageLoaded;
			Dispatch(new StoreInitializedAction());
		}

		/// <see cref="IStore.AddFeature(IFeature)"/>
		public void AddFeature(IFeature feature)
		{
			if (feature == null)
				throw new ArgumentNullException(nameof(feature));

			FeaturesByName.Add(feature.GetName(), feature);
		}

		/// <see cref="IDispatcher.Dispatch(object)"/>
		public void Dispatch(object action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			// Do not allow task dispatching inside a middleware-change.
			// These change cycles are for things like "jump to state" in Redux Dev Tools
			// and should be short lived.
			// We avoid dispatching inside a middleware change because we don't want UI events (like component Init)
			// that trigger actions (such as fetching data from a server) to execute
			if (IsInsideMiddlewareChange)
				return;

			// If there was already an action in the Queue then an action dispatch is already in progress, so we will just
			// let this new action be added to the queue and then exit
			// Note: This is to cater for the following scenario
			//	1: An action is dispatched
			//	2: An effect is triggered
			//	3: The effect immediately dispatches a new action
			// The Queue ensures it is processed after its triggering action has completed rather than immediately
			bool wasAlreadyDispatching = QueuedActions.Any();
			if (wasAlreadyDispatching)
			{
				System.Diagnostics.Debug.WriteLine(">" + QueuedActions.Peek().ToString());
			}
			QueuedActions.Enqueue(action);
			if (wasAlreadyDispatching)
				return;

			// HasActivatedStore is set to true when the page finishes loading
			// At which point DequeueActions will be called
			if (!HasActivatedStore)
				return;

			DequeueActions();
		}

		/// <see cref="IStore.AddEffect(IEffectFuncs)"/>
		public void AddEffect(IEffectFuncs effectFuncs)
		{
			if (effectFuncs == null)
				throw new ArgumentNullException(nameof(effectFuncs));
			EffectFuncs.Add(effectFuncs);
		}

		/// <see cref="IStore.AddMiddleware(IMiddleware)"/>
		public void AddMiddleware(IMiddleware middleware)
		{
			Middlewares.Add(middleware);
			ReversedMiddlewares.Insert(0, middleware);
			// Initialize the middleware immediately if the store has already been initialized, otherwise this will be
			// done the first time Dispatch is called
			if (HasActivatedStore)
			{
				middleware.Initialize(this);
				middleware.AfterInitializeAllMiddlewares();
			}
		}

		/// <see cref="IStore.BeginInternalMiddlewareChange"/>
		public IDisposable BeginInternalMiddlewareChange()
		{
			BeginMiddlewareChangeCount++;
			IDisposable[] disposables = Middlewares
				.Select(x => x.BeginInternalMiddlewareChange())
				.ToArray();
			return new DisposableCallback(() =>
			{
				BeginMiddlewareChangeCount--;
				if (BeginMiddlewareChangeCount == 0)
					disposables.ToList().ForEach(x => x.Dispose());
			});
		}

		/// <see cref="IStore.Initialize"/>
		public RenderFragment Initialize()
		{
			return (RenderTreeBuilder renderer) =>
			{
				var scriptsBuilder = new StringBuilder();
				scriptsBuilder.AppendLine("if (window.DotNet) {");
				{
					scriptsBuilder.AppendLine("setTimeout(function() {");
					{
						foreach (IMiddleware middleware in Middlewares)
						{
							string middlewareScript = middleware.GetClientScripts();
							if (middlewareScript != null)
							{
								scriptsBuilder.AppendLine($"// Middleware scripts: {middleware.GetType().FullName}");
								scriptsBuilder.AppendLine($"{middlewareScript}");
							}
						}
						scriptsBuilder.AppendLine("//Fluxor");
						scriptsBuilder.AppendLine(GetClientScripts());
					}
					scriptsBuilder.AppendLine("}, 0);"); // End of setTimeout
				}
				scriptsBuilder.AppendLine("}");

				renderer.OpenElement(1, "script");
				renderer.AddMarkupContent(2, scriptsBuilder.ToString());
				renderer.CloseElement();
			};
		}

		//TODO: PeteM - Should this await?
		private void TriggerEffects(object action)
		{
			System.Diagnostics.Debug.WriteLine("TriggerEffects: " + action.ToString());
			var effectsToTrigger = EffectFuncs.Where(x => x.ShouldReactToAction(action));
			System.Diagnostics.Debug.WriteLine("Found " + effectsToTrigger.Count());
			foreach (var effect in effectsToTrigger)
				effect.HandleAsync(action, this);
		}

		private void InitializeMiddlewares()
		{
			Middlewares.ForEach(x => x.Initialize(this));
			Middlewares.ForEach(x => x.AfterInitializeAllMiddlewares());
		}

		private void ExecuteMiddlewareBeforeDispatch(object actionAboutToBeDispatched)
		{
			foreach (IMiddleware middleWare in Middlewares)
				middleWare.BeforeDispatch(actionAboutToBeDispatched);
		}

		private void ExecuteMiddlewareAfterDispatch(object actionJustDispatched)
		{
			Middlewares.ForEach(x => x.AfterDispatch(actionJustDispatched));
		}

		private void ActivateStore()
		{
			if (HasActivatedStore)
				return;

			HasActivatedStore = true;
			InitializeMiddlewares();
			DequeueActions();
			InitializedCompletionSource.SetResult(true);
		}

		private void DequeueActions()
		{
			while (QueuedActions.Any())
			{
				// We want the next action but we won't dequeue it because we use
				// a non-empty queue as an indication that a Dispatch() loop is already in progress
				object nextActionToDequeue = QueuedActions.Peek();
				System.Diagnostics.Debug.WriteLine("1");
				// Only process the action if no middleware vetos it
				if (Middlewares.All(x => x.MayDispatchAction(nextActionToDequeue)))
				{
					System.Diagnostics.Debug.WriteLine("2");
					ExecuteMiddlewareBeforeDispatch(nextActionToDequeue);

					System.Diagnostics.Debug.WriteLine("3");
					// Notify all features of this action
					foreach (var featureInstance in FeaturesByName.Values)
						NotifyFeatureOfDispatch(featureInstance, nextActionToDequeue);

					System.Diagnostics.Debug.WriteLine("4");
					ExecuteMiddlewareAfterDispatch(nextActionToDequeue);

					System.Diagnostics.Debug.WriteLine("5");
					TriggerEffects(nextActionToDequeue);
					System.Diagnostics.Debug.WriteLine("6");
				}
				// Now remove the processed action from the queue so we can move on to the next (if any)
				QueuedActions.Dequeue();
			}
		}

		private string GetClientScripts()
		{
			return BrowserInteropService.GetClientScripts();
		}

		private void OnPageLoaded(object sender, EventArgs e)
		{
			BrowserInteropService.PageLoaded -= OnPageLoaded;
			ActivateStore();
		}
	}
}
