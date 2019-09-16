using Blazor.Fluxor.DependencyInjection.DependencyScanners;
using Blazor.Fluxor.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
			IEnumerable<Type> allNonAbstractCandidateTypes = allCandidateTypes.Where(t => !t.IsAbstract);
			IEnumerable<Assembly> allCandidateAssemblies = assembliesToScan.Select(x => x.Assembly)
				.Union(scanIncludeList.Select(x => x.Assembly))
				.Distinct();

			IEnumerable<AssemblyScanSettings> scanExcludeList =
				MiddlewareScanner.FindMiddlewareLocations(allCandidateAssemblies);
			allCandidateTypes = AssemblyScanSettings.Filter(
				types: allCandidateTypes,
				scanExcludeList: scanExcludeList,
				scanIncludeList: scanIncludeList);


			IEnumerable<DiscoveredReducerClass> discoveredReducerClasses =
				ReducersRegistration.DiscoverReducers(serviceCollection, allNonAbstractCandidateTypes);

			IEnumerable<DiscoveredEffectClass> discoveredEffectClasses =
				EffectsRegistration.DiscoverEffects(serviceCollection, allNonAbstractCandidateTypes);

			IEnumerable<DiscoveredFeatureClass> discoveredFeatureClasses =
				FeaturesRegistration.DiscoverFeatures(serviceCollection, allNonAbstractCandidateTypes, discoveredReducerClasses);

			RegisterStore(serviceCollection, discoveredFeatureClasses, discoveredEffectClasses);
		}

		private static void RegisterStore(IServiceCollection serviceCollection, 
			IEnumerable<DiscoveredFeatureClass> discoveredFeatureInfos,
			IEnumerable<DiscoveredEffectClass> discoveredEffectInfos)
		{
			// Register IDispatcher as an alias to IStore
			serviceCollection.AddScoped<IDispatcher>(sp => sp.GetService<IStore>());

			// Register a custom factory for building IStore that will inject all effects
			serviceCollection.AddScoped(typeof(IStore), serviceProvider =>
			{
				var browserInteropService = serviceProvider.GetService<IBrowserInteropService>();
				var store = new Store(browserInteropService);
				foreach(DiscoveredFeatureClass discoveredFeatureInfo in discoveredFeatureInfos)
				{
					IFeature feature = (IFeature)serviceProvider.GetService(discoveredFeatureInfo.FeatureInterfaceGenericType);
					store.AddFeature(feature);
				}

				foreach(DiscoveredEffectClass discoveredEffectInfo in discoveredEffectInfos)
				{
					IEffect effect = (IEffect)serviceProvider.GetService(discoveredEffectInfo.ImplementingType);
					store.AddEffect(effect);
				}

				foreach (Type middlewareType in Options.MiddlewareTypes)
				{
					IMiddleware middleware = (IMiddleware)serviceProvider.GetService(middlewareType);
					store.AddMiddleware(middleware);
				}

				return store;
			});
		}
	}
}
