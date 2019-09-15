using Blazor.Fluxor.AutoDiscovery;
using Blazor.Fluxor.DependencyInjection.DependencyScanners;
using Blazor.Fluxor.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Fluxor.DependencyInjection
{
	internal static class DependencyScanner
	{
		internal static void Scan(this IServiceCollection serviceCollection,
			IEnumerable<AssemblyScanSettings> assembliesToScan, IEnumerable<AssemblyScanSettings> scanIncludeList)
		{
			if (assembliesToScan == null || assembliesToScan.Count() == 0)
				throw new ArgumentNullException(nameof(assembliesToScan));
			scanIncludeList = scanIncludeList ?? new List<AssemblyScanSettings>();

			IEnumerable<Type> allCandidateTypes = assembliesToScan.SelectMany(x => x.Assembly.GetTypes())
				.Union(scanIncludeList.SelectMany(x => x.Assembly.GetTypes()))
				.Distinct();
			IEnumerable<Assembly> allCandidateAssemblies = assembliesToScan.Select(x => x.Assembly)
				.Union(scanIncludeList.Select(x => x.Assembly))
				.Distinct();

			IEnumerable<AssemblyScanSettings> scanExcludeList =
				MiddlewareScanner.FindMiddlewareLocations(allCandidateAssemblies);
			allCandidateTypes = AssemblyScanSettings.Filter(
				types: allCandidateTypes,
				scanExcludeList: scanExcludeList,
				scanIncludeList: scanIncludeList);

			IEnumerable<DiscoveredEffect> discoveredEffects =
				EffectsRegistration.DiscoverEffects(serviceCollection, allCandidateTypes);

			IEnumerable<DiscoveredReducerInfo> discoveredReducerInfos =
				ReducersRegistration.DiscoverReducers(serviceCollection, allCandidateTypes);

			IEnumerable<DiscoveredFeatureInfo> discoveredFeatureInfos =
				FeaturesRegistration.DiscoverFeatures(serviceCollection, allCandidateTypes, discoveredReducerInfos);

			RegisterStore(serviceCollection, discoveredFeatureInfos, discoveredEffects);
		}

		private static void RegisterStore(IServiceCollection serviceCollection,
			IEnumerable<DiscoveredFeatureInfo> discoveredFeatureInfos,
			IEnumerable<DiscoveredEffect> discoveredEffects)
		{
			// Register IDispatcher as an alias to IStore
			serviceCollection.AddScoped<IDispatcher>(sp => sp.GetService<IStore>());

			// Register a custom factory for building IStore that will inject all effects
			serviceCollection.AddScoped(typeof(IStore), serviceProvider =>
			{
				var browserInteropService = serviceProvider.GetService<IBrowserInteropService>();
				var store = new Store(browserInteropService);
				foreach (DiscoveredFeatureInfo discoveredFeatureInfo in discoveredFeatureInfos)
				{
					var feature = (IFeature)serviceProvider.GetService(discoveredFeatureInfo.FeatureInterfaceGenericType);
					store.AddFeature(feature);
				}

				foreach (DiscoveredEffect discoveredEffect in discoveredEffects)
				{
					IEffectFuncs effectFuncs = TypedEffectFuncs<bool>.Create(
						serviceProvider,
						discoveredEffect.MethodInfo,
						discoveredEffect.Options);
					store.AddEffect(effectFuncs);
				}

				foreach (Type middlewareType in Options.MiddlewareTypes)
				{
					var middleware = (IMiddleware)serviceProvider.GetService(middlewareType);
					store.AddMiddleware(middleware);
				}

				return store;
			});
		}
	}
}
