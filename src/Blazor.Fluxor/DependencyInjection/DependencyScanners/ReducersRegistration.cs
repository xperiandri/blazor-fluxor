using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class ReducersRegistration
	{
		internal static IEnumerable<DiscoveredReducerClass> DiscoverReducers(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredReducerClass> discoveredReducerInfos = allCandidateTypes
				.Select(t => new
				{
					ImplementingType = t,
					GenericParameterTypes = TypeHelper.GetGenericParametersForImplementedInterface(t, typeof(IReducer<>))
				})
				.Where(x => x.GenericParameterTypes != null)
				.Select(x => new DiscoveredReducerClass(
					implementingType: x.ImplementingType,
					stateType: x.GenericParameterTypes[0]))
				.ToList();

			foreach (DiscoveredReducerClass discoveredReducerInfo in discoveredReducerInfos)
			{
				RegisterReducer(serviceCollection, discoveredReducerInfo);
			}

			return discoveredReducerInfos;
		}

		private static void RegisterReducer(IServiceCollection serviceCollection, DiscoveredReducerClass discoveredReducerInfo)
		{
			// Register the feature class
			serviceCollection.AddScoped(serviceType: discoveredReducerInfo.ImplementingType);
		}
	}
}
