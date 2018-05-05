using Blazor.Fluxor.Extensions;
using Blazor.Fluxor.ReduxDevTools.CallbackObjects;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.ReduxDevTools
{
	public class ReduxDevToolsMiddleware : Middleware 
	{
		private const string ToJsDispatchId = "fluxorDevTools/dispatch";

		private int SequenceNumberOfCurrentState = -1; // First state is 0, so pre-first state is step -1
		private int SequenceNumberOfLatestState = -1;

		public ReduxDevToolsMiddleware()
		{
			ReduxDevToolsInterop.JumpToState += OnJumpToState;
		}

		public override bool MayDispatchAction(IAction action)
		{
			return SequenceNumberOfCurrentState == SequenceNumberOfLatestState;
		}

		public override void AfterDispatch(IAction action)
		{
			IDictionary<string, object> state = GetState();
			ReduxDevToolsInterop.Invoke<object>(ToJsDispatchId, new ActionInfo(action), state);
			// As actions can only be executed if not in a historical state (yes, "a" historical, pronounce your H!)
			// ensure the latest is incremented, and the current = latest
			SequenceNumberOfLatestState++;
			SequenceNumberOfCurrentState = SequenceNumberOfLatestState;
		}

		private IDictionary<string, object> GetState()
		{
			var state = (IDictionary<string, object>)new ExpandoObject();
			foreach (IFeature feature in Store.Features.OrderBy(x => x.GetName()))
				state[feature.GetName()] = feature.GetState();
			return state;
		}

		private void OnJumpToState(object sender, JumpToStateCallback e)
		{
			SequenceNumberOfCurrentState = e.payload.actionId;
			using (Store.BeginInternalMiddlewareChange())
			{
				Dictionary<string, IFeature> featuresByName = Store.Features.ToDictionary(x => x.GetName());

				var newFeatureStates = (IDictionary<string, object>)JsonUtil.Deserialize<object>(e.state);
				foreach (KeyValuePair<string, object> newFeatureState in newFeatureStates)
				{
					// Get the feature with the given name
					if (!featuresByName.TryGetValue(newFeatureState.Key, out IFeature feature))
						continue;

					// Get the generic method of JsonUtil.Deserialize<> so we have the correct object type for the state
					string deserializeMethodName = nameof(JsonUtil.Deserialize);
					MethodInfo deserializeMethodInfo = typeof(JsonUtil)
						.GetMethod(deserializeMethodName)
						.GetGenericMethodDefinition()
						.MakeGenericMethod(new Type[] { feature.GetStateType() });

					// Get the state we were given as a json string
					string serializedFeatureState = newFeatureState.Value?.ToString();
					// Deserialize that json using the generic method, so we get an object of the correct type
					object stronglyTypedFeatureState =
						string.IsNullOrEmpty(serializedFeatureState)
						? null
						: deserializeMethodInfo.Invoke(null, new object[] { serializedFeatureState });

					// Now set the feature's state to the deserialized object
					feature.RestoreState(stronglyTypedFeatureState);
				}
			}
			//TODO: Force render
		}

		public override string GetClientScripts()
		{
			string assemblyName = typeof(ReduxDevToolsInterop).Assembly.GetName().Name;
			string @namespace = typeof(ReduxDevToolsInterop).GetNamespace();
			string className = typeof(ReduxDevToolsInterop).Name;
			string callbackMethodName = nameof(ReduxDevToolsInterop.DevToolsCallback);

			return $@"
(function() {{
	const reduxDevTools = window.__REDUX_DEVTOOLS_EXTENSION__;
	if (reduxDevTools !== undefined && reduxDevTools !== null) {{
		const fluxorDevToolsCallback = Blazor.platform.findMethod(
			'{assemblyName}',
			'{@namespace}',
			'{className}',
			'{callbackMethodName}'
		);
		
		const fluxorDevTools = reduxDevTools.connect({{ name: 'Blazor-Fluxor' }});
		if (fluxorDevTools !== undefined && fluxorDevTools !== null) {{
			fluxorDevTools.subscribe((message) => {{ 
				const messageAsJson = JSON.stringify(message);
				const messageAsString = Blazor.platform.toDotNetString(messageAsJson);
				Blazor.platform.callMethod(fluxorDevToolsCallback, null, [ messageAsString ]);
			}});

		}}

		Blazor.registerFunction('{ToJsDispatchId}', function(action, state) {{
			fluxorDevTools.send(action, state);
		}});

		// Notify Fluxor of the presence of the browser plugin
		const detectedMessage = {{
			payload: {{
				type: '{ReduxDevToolsInterop.FromJsDevToolsDetectedId}'
			}}
		}};
		const detectedMessageAsJson = JSON.stringify(detectedMessage);
		const detectedMessageAsString = Blazor.platform.toDotNetString(detectedMessageAsJson);
		Blazor.platform.callMethod(fluxorDevToolsCallback, null, [ detectedMessageAsString ]);
	}}
}})();
";
		}
	}
}
