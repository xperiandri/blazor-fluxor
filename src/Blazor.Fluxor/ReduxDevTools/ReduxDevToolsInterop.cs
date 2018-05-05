using Blazor.Fluxor.ReduxDevTools.CallbackObjects;
using Microsoft.AspNetCore.Blazor;
using System;

namespace Blazor.Fluxor.ReduxDevTools
{
	internal static class ReduxDevToolsInterop
	{
		internal const string FromJsDevToolsDetectedId = "fluxorDevTools/detected";

		internal static bool DevToolsBrowserPluginDetected { get; private set; }
		internal static event EventHandler<JumpToStateCallback> JumpToState;
		internal static event EventHandler AfterJumpToState;

		internal static void DevToolsCallback(string messageAsJson)
		{
			if (string.IsNullOrWhiteSpace(messageAsJson))
				return;

			var message = JsonUtil.Deserialize<BaseCallbackObject>(messageAsJson);
			switch (message?.payload?.type)
			{
				case FromJsDevToolsDetectedId:
					Console.WriteLine("Tools detected");
					DevToolsBrowserPluginDetected = true;
					break;

				case "JUMP_TO_STATE":
					OnJumpToState(JsonUtil.Deserialize<JumpToStateCallback>(messageAsJson));
					break;
			}
		}

		public static TRes Invoke<TRes>(string identifier, params object[] args)
		{
			if (!DevToolsBrowserPluginDetected)
			{
				Console.WriteLine("Plugin not detected");
				return default(TRes);
			}
			Console.WriteLine("Plugin detected: Invoking " + identifier);
			return Microsoft.AspNetCore.Blazor.Browser.Interop.RegisteredFunction.Invoke<TRes>(identifier, args);
		}

		private static void OnJumpToState(JumpToStateCallback jumpToStateCallback)
		{
			JumpToState?.Invoke(null, jumpToStateCallback);
			AfterJumpToState?.Invoke(null, EventArgs.Empty);
		}

	}
}
