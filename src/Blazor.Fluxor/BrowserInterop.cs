using Blazor.Fluxor.Extensions;
using System;
using System.Linq;

namespace Blazor.Fluxor
{
	public static class BrowserInterop
	{
		public static event EventHandler PageLoaded;

		public static string GetClientScripts()
		{
			string assemblyName = typeof(BrowserInterop).Assembly.GetName().Name;
			string @namespace = typeof(Store).GetNamespace();
			string className = typeof(BrowserInterop).Name;
			string callbackMethodName = nameof(OnPageLoaded);

			return $@"
	(function() {{ 
		//TODO: PeteM - Remove setTimeout
		setTimeout(function() {{
			const fluxorBrowserHooksLoadedCallback = Blazor.platform.findMethod(
				'{assemblyName}',
				'{@namespace}',
				'{className}',
				'{callbackMethodName}'
			);
			Blazor.platform.callMethod(fluxorBrowserHooksLoadedCallback, null, []); 
		}}, 1000);
	}})();
";
		}

		private static void OnPageLoaded()
		{
			PageLoaded?.Invoke(null, EventArgs.Empty);
		}
	}
}
