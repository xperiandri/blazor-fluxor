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
			IEnumerable<Type> allCandidateTypes, IEnumerable<DiscoveredReducer> discoveredReducers)
		{
			Dictionary<Type, IGrouping<Type, DiscoveredReducer>> discoveredReducerInfosByStateType =
				discoveredReducers
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
					out IGrouping<Type, DiscoveredReducer> discoveredFeaturesForFeatureState);

				RegisterFeature(
					serviceCollection,
					discoveredFeatureInfo: discoveredFeatureInfo,
					discoveredReducersForFeatureState: discoveredFeaturesForFeatureState);
			}

			return discoveredFeatureInfos;
		}

		private static void RegisterFeature(IServiceCollection serviceCollection,
			DiscoveredFeatureInfo discoveredFeatureInfo,
			IEnumerable<DiscoveredReducer> discoveredReducersForFeatureState)
		{
			string addReducerMethodName = nameof(IFeature<object>.AddReducer);

			// Register the implementing type so we can get an instance from the service provider
			serviceCollection.AddScoped(discoveredFeatureInfo.ImplementingType);

			// Register a factory for creating instance of this feature type when requested via the generic IFeature interface
			serviceCollection.AddScoped(discoveredFeatureInfo.FeatureInterfaceGenericType, serviceProvider =>
			{
				// Create an instance of the implementing type
				var featureInstance = (IFeature)serviceProvider.GetService(discoveredFeatureInfo.ImplementingType);

				//TODO: PeteM - Make this a delegate
				MethodInfo featureAddReducerMethod =
					discoveredFeatureInfo.ImplementingType.GetMethod(addReducerMethodName);

				if (discoveredReducersForFeatureState != null)
				{
					foreach (DiscoveredReducer discoveredReducer in discoveredReducersForFeatureState)
					{
						System.Diagnostics.Debug.WriteLine("Reducer name: " + discoveredReducer.MethodInfo.Name);
						IReducerFuncs reducerFuncs = ReflectedReducerFuncs<object, object>.Create(
							serviceProvider,
							discoveredReducer.MethodInfo,
							discoveredReducer.Options);
						featureAddReducerMethod.Invoke(featureInstance, new object[] { reducerFuncs });
					}
				}
				return featureInstance;
			});
		}

	}
}
