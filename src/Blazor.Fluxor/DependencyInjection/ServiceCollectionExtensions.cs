using Blazor.Fluxor.DependencyInjection;
using Blazor.Fluxor.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddFluxor(this IServiceCollection serviceCollection, Action<Options> configure)
		{
			if (configure == null)
				throw new ArgumentNullException(nameof(configure));

			// We only use an instance so middleware can create extensions to the Options
			var options = new Options();
			configure(options);

			// Register all middleware types with dependency injection
			foreach (Type middlewareType in Options.MiddlewareTypes)
				serviceCollection.AddScoped(middlewareType);

			IEnumerable<AssemblyScanSettings> scanWhitelist = Options.MiddlewareTypes
				.Select(t => new AssemblyScanSettings(t.Assembly, t.GetNamespace()));

			// Scan for features and effects
			if (Options.DependencyInjectionEnabled)
				DependencyScanner.Scan(
					serviceCollection: serviceCollection,
					assembliesToScan: Options.DependencyInjectionAssembliesToScan,
					scanWhitelist: scanWhitelist);

			return serviceCollection;
		}

	}
}
