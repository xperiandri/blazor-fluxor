using Blazor.Fluxor.Extensions;
using System;

namespace Blazor.Fluxor
{
	/// <summary>
	/// Provides standard interactions with the browser via Javascript
	/// </summary>
	public static class BrowserInterop
	{
		/// <summary>
		/// Executed when the browser finishes loading the page
		/// </summary>
		public static event EventHandler PageLoaded;

		/// <summary>
		/// Gets Javascripts required to support the features of this class
		/// </summary>
		/// <returns></returns>
		public static string GetClientScripts()
		{
			string assemblyName = typeof(BrowserInterop).Assembly.GetName().Name;
			string @namespace = typeof(Store).GetNamespace();
			string className = typeof(BrowserInterop).Name;
			string callbackMethodName = nameof(OnPageLoaded);

			return $@"
	(function() {{ 
		const fluxorBrowserHooksLoadedCallback = Blazor.platform.findMethod(
			'{assemblyName}',
			'{@namespace}',
			'{className}',
			'{callbackMethodName}'
		);
		Blazor.platform.callMethod(fluxorBrowserHooksLoadedCallback, null, []); 
	}})();
";
		}

		private static void OnPageLoaded()
		{
			PageLoaded?.Invoke(null, EventArgs.Empty);
		}
	}
}
