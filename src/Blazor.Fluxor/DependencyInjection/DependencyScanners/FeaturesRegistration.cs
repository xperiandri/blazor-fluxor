using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class FeaturesRegistration
	{
		internal static IEnumerable<DiscoveredFeatureInfo> DiscoverFeatures(IServiceCollection serviceCollection, 
			IEnumerable<Type> allCandidateTypes,
			IEnumerable<DiscoveredReducerInfo> discoveredReducerInfos,
			IEnumerable<DiscoveredReducerMethodInfo> discoveredReducerMethodInfos)
		{
			Dictionary<Type, IGrouping<Type, DiscoveredReducerInfo>> discoveredReducerInfosByStateType =
				discoveredReducerInfos
				.GroupBy(x => x.StateType)
				.ToDictionary(x => x.Key);

			Dictionary<Type, IGrouping<Type, DiscoveredReducerMethodInfo>> discoveredReducerMethodInfosByStateType =
				discoveredReducerMethodInfos
				.GroupBy(x => x.StateType)
				.ToDictionary(x => x.Key);

			IEnumerable<DiscoveredFeatureInfo> discoveredFeatureInfos = allCandidateTypes
				.Select(t => new
				{
					ImplementingType = t,
					GenericParameterTypes = TypeHelper.GetGenericParametersForImplementedInterface(t, typeof(IFeature<>))
				})
				.Where(x => x.GenericParameterTypes != null)
				.Select(x => new DiscoveredFeatureInfo(
					implementingType: x.ImplementingType,
					stateType: x.GenericParameterTypes[0]
					)
				)
				.ToList();

			foreach (DiscoveredFeatureInfo discoveredFeatureInfo in discoveredFeatureInfos)
			{
				discoveredReducerInfosByStateType.TryGetValue(
					discoveredFeatureInfo.StateType,
					out IGrouping<Type, DiscoveredReducerInfo> discoveredReducerInfosForFeatureState);

				discoveredReducerMethodInfosByStateType.TryGetValue(
					discoveredFeatureInfo.StateType,
					out IGrouping<Type, DiscoveredReducerMethodInfo> discoveredReducerMethoInfosForFeatureState);

				RegisterFeature(
					serviceCollection,
					discoveredFeatureInfo: discoveredFeatureInfo,
					discoveredReducerInfosForFeatureState: discoveredReducerInfosForFeatureState,
					discoveredReducerMethodInfosForFeatureState: discoveredReducerMethoInfosForFeatureState);
			}

			return discoveredFeatureInfos;
		}

		private static void RegisterFeature(IServiceCollection serviceCollection,
			DiscoveredFeatureInfo discoveredFeatureInfo,
			IEnumerable<DiscoveredReducerInfo> discoveredReducerInfosForFeatureState,
			IEnumerable<DiscoveredReducerMethodInfo> discoveredReducerMethodInfosForFeatureState)
		{
			string addReducerMethodName = nameof(IFeature<object>.AddReducer);

			// Register the implementing type so we can get an instance from the service provider
			serviceCollection.AddScoped(discoveredFeatureInfo.ImplementingType);

			// Register a factory for creating instance of this feature type when requested via the generic IFeature interface
			serviceCollection.AddScoped(discoveredFeatureInfo.FeatureInterfaceGenericType, serviceProvider =>
			{
				// Create an instance of the implementing type
				IFeature featureInstance = (IFeature)serviceProvider.GetService(discoveredFeatureInfo.ImplementingType);

				MethodInfo featureAddReducerMethod =
					discoveredFeatureInfo.ImplementingType.GetMethod(addReducerMethodName);

				if (discoveredReducerInfosForFeatureState != null)
				{
					foreach (DiscoveredReducerInfo reducerInfo in discoveredReducerInfosForFeatureState)
					{
						object reducerInstance = serviceProvider.GetService(reducerInfo.ImplementingType);
						featureAddReducerMethod.Invoke(featureInstance, new object[] { reducerInstance });
					}
				}

				if (discoveredReducerMethodInfosForFeatureState != null)
				{
					foreach(DiscoveredReducerMethodInfo reducerMethodInfo in discoveredReducerMethodInfosForFeatureState)
					{
						System.Diagnostics.Debug.WriteLine("Found one");
						object reducerMethodHost = serviceProvider.GetService(reducerMethodInfo.ImplementingType);
						var reducer = ReducerMethodWrapper<object>.Create(
							reducerMethodHost: reducerMethodHost,
							discoveredReducerMethodInfo: reducerMethodInfo);
					}
				}

				return featureInstance;
			});
		}

	}
}
