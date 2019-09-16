﻿using Blazor.Fluxor.DependencyInjection.DependencyScanners;
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
				MiddlewareClassesDiscovery.FindMiddlewareLocations(allCandidateAssemblies);
			allCandidateTypes = AssemblyScanSettings.Filter(
				types: allCandidateTypes,
				scanExcludeList: scanExcludeList,
				scanIncludeList: scanIncludeList);


			IEnumerable<DiscoveredReducerClass> discoveredReducerClasses =
				ReducerClassessDiscovery.DiscoverReducerClasses(serviceCollection, allNonAbstractCandidateTypes);

			IEnumerable<DiscoveredEffectClass> discoveredEffectClasses =
				EffectClassessDiscovery.DiscoverEffectClasses(serviceCollection, allNonAbstractCandidateTypes);

			IEnumerable<DiscoveredEffectMethod> discoveredEffectMethods =
				EffectMethodsDiscovery.DiscoverEffectMethods(serviceCollection, allCandidateTypes);

			IEnumerable<DiscoveredFeatureClass> discoveredFeatureClasses =
				FeatureClassesDiscovery.DiscoverFeatureClasses(serviceCollection, allNonAbstractCandidateTypes, discoveredReducerClasses);

			RegisterStore(
				serviceCollection,
				discoveredFeatureClasses,
				discoveredEffectClasses,
				discoveredEffectMethods);
		}

		private static void RegisterStore(IServiceCollection serviceCollection, 
			IEnumerable<DiscoveredFeatureClass> discoveredFeatureClasses,
			IEnumerable<DiscoveredEffectClass> discoveredEffectClasses,
			IEnumerable<DiscoveredEffectMethod> discoveredEffectMethods)
		{
			// Register IDispatcher as an alias to IStore
			serviceCollection.AddScoped<IDispatcher>(sp => sp.GetService<IStore>());

			// Register a custom factory for building IStore that will inject all effects
			serviceCollection.AddScoped(typeof(IStore), serviceProvider =>
			{
				var browserInteropService = serviceProvider.GetService<IBrowserInteropService>();
				var store = new Store(browserInteropService);
				foreach(DiscoveredFeatureClass discoveredFeatureInfo in discoveredFeatureClasses)
				{
					var feature = (IFeature)serviceProvider.GetService(discoveredFeatureInfo.FeatureInterfaceGenericType);
					store.AddFeature(feature);
				}

				foreach(DiscoveredEffectClass discoveredEffectInfo in discoveredEffectClasses)
				{
					var effect = (IEffect)serviceProvider.GetService(discoveredEffectInfo.ImplementingType);
					store.AddEffect(effect);
				}

				foreach(DiscoveredEffectMethod discoveredEffectMethod in discoveredEffectMethods)
				{
					IEffect effect = EffectWrapper<object>.Create(serviceProvider, discoveredEffectMethod);
					store.AddEffect(effect);
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
