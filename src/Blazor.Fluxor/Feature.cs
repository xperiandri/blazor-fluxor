using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor
{
	/// <see cref="IFeature{TState}"/>
	public abstract class Feature<TState> : IFeature<TState>
	{
		/// <see cref="IFeature.GetName"/>
		public abstract string GetName();
		/// <see cref="IFeature.GetState"/>
		public virtual object GetState() => State;
		/// <see cref="IFeature.RestoreState(object)"/>
		public virtual void RestoreState(object value) => State = (TState)value;
		/// <see cref="IFeature.GetStateType"/>
		public virtual Type GetStateType() => typeof(TState);

		/// <summary>
		/// Gets the initial state for the feature
		/// </summary>
		/// <returns>The initial state</returns>
		protected abstract TState GetInitialState();
		/// <summary>
		/// A list of reducers registered with this feature
		/// </summary>
		protected readonly List<IReducer<TState>> Reducers = new List<IReducer<TState>>();

		private Dictionary<WeakReference, Action> StateChangedCallbacks = new Dictionary<WeakReference, Action>();

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public Feature()
		{
			State = GetInitialState();
		}

		private TState _State;
		/// <see cref="IFeature{TState}.State"/>
		public virtual TState State
		{
			get => _State;
			protected set
			{
				bool stateHasChanged = !Object.ReferenceEquals(_State, value);
				_State = value;
				if (stateHasChanged)
					TriggerStateChangedCallbacks();
			}
		}

		/// <see cref="IFeature{TState}.AddReducer(IReducer{TState})"/>
		public virtual void AddReducer(IReducer<TState> reducer)
		{
			if (reducer == null)
				throw new ArgumentNullException(nameof(reducer));
			Reducers.Add(reducer);
		}

		/// <see cref="IFeature.ReceiveDispatchNotificationFromStore(IAction)"/>
		public virtual void ReceiveDispatchNotificationFromStore(IAction action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			IEnumerable<IReducer<TState>> applicableReducers = Reducers.Where(x => x.ShouldReduceStateForAction(action));
			TState newState = State;
			foreach (IReducer<TState> currentReducer in applicableReducers)
			{
				newState = currentReducer.Reduce(newState, action);
			}
			State = newState;
		}

		/// <see cref="IFeature.Changed{TObserver}(TObserver, Action)"/>
		public void Changed<TObserver>(TObserver observer, Action callback)
		{
			StateChangedCallbacks[new WeakReference(observer)] = callback;
		}

		private void TriggerStateChangedCallbacks()
		{
			Console.WriteLine($"Calling state change callbacks");
			var callbacks = new List<Action>();
			var observers = new List<object>();
			var newStateChangedCallbacks = new Dictionary<WeakReference, Action>();

			// Keep only weak references that have not expired
			foreach (KeyValuePair<WeakReference, Action> callbackReference in StateChangedCallbacks)
			{
				Object observer = callbackReference.Key.Target;
				if (observer != null)
				{
					// Temporarily keep a reference to the observer so it isn't collected before we can call it back
					observers.Add(observer);
					// Keep a reference to the callback
					Action callback = callbackReference.Value;
					callbacks.Add(callback);
					// Add this observer+callback combination to the replacement dictionary
					newStateChangedCallbacks[callbackReference.Key] = callbackReference.Value;
				}
			}

			StateChangedCallbacks = newStateChangedCallbacks;
			// Now execute the callbacks
			callbacks.ForEach(callback => callback());
		}
	}

}
