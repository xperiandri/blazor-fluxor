using Blazor.Fluxor.Extensions;
using Blazor.Fluxor.ReduxDevTools.CallbackObjects;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;

namespace Blazor.Fluxor.ReduxDevTools
{
	internal static class ReduxDevToolsInterop
	{
		internal const string FromJsDevToolsDetectedId = "fluxorDevTools/detected";

		internal static bool DevToolsBrowserPluginDetected { get; private set; }
		internal static event EventHandler<JumpToStateCallback> JumpToState;
		internal static event EventHandler AfterJumpToState;
		internal static event EventHandler Commit;

		private const string ToJsDispatchId = "fluxorDevTools/dispatch";
		private const string ToJsInitId = "fluxorDevTools/init";

		internal static void Init(IDictionary<string, object> state)
		{
			Invoke<object>(ToJsInitId, state);
		}

		internal static void Dispatch(IAction action, IDictionary<string, object> state)
		{
			Invoke<object>(ToJsDispatchId, new ActionInfo(action), state);
		}

		private static void DevToolsCallback(string messageAsJson)
		{
			if (string.IsNullOrWhiteSpace(messageAsJson))
				return;

			var message = JsonUtil.Deserialize<BaseCallbackObject>(messageAsJson);
			switch (message?.payload?.type)
			{
				case FromJsDevToolsDetectedId:
					DevToolsBrowserPluginDetected = true;
					break;

				case "COMMIT":
					Commit?.Invoke(null, EventArgs.Empty);
					break;

				case "JUMP_TO_STATE":
					OnJumpToState(JsonUtil.Deserialize<JumpToStateCallback>(messageAsJson));
					break;
			}
		}

		private static TRes Invoke<TRes>(string identifier, params object[] args)
		{
			if (!DevToolsBrowserPluginDetected)
				return default(TRes);

			return Microsoft.AspNetCore.Blazor.Browser.Interop.RegisteredFunction.Invoke<TRes>(identifier, args);
		}

		private static void OnJumpToState(JumpToStateCallback jumpToStateCallback)
		{
			JumpToState?.Invoke(null, jumpToStateCallback);
			AfterJumpToState?.Invoke(null, EventArgs.Empty);
		}

		internal static string GetClientScripts()
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

		Blazor.registerFunction('{ToJsInitId}', function(state) {{
			fluxorDevTools.init(state);
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
