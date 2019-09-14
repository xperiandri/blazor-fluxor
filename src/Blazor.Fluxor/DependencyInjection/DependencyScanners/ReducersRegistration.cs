using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class ReducersRegistration
	{
		internal static IEnumerable<DiscoveredReducerInfo> DiscoverReducers(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredReducerInfo> discoveredReducerInfos = allCandidateTypes
				.Select(t => new
				{
					ImplementingType = t,
					GenericParameterTypes = TypeHelper.GetGenericParametersForImplementedInterface(t, typeof(IReducer<>))
				})
				.Where(x => x.GenericParameterTypes != null)
				.Select(x => new DiscoveredReducerInfo(
					implementingType: x.ImplementingType,
					stateType: x.GenericParameterTypes[0]))
				.ToList();

			foreach (DiscoveredReducerInfo discoveredReducerInfo in discoveredReducerInfos)
				serviceCollection.AddScoped(discoveredReducerInfo.ImplementingType);

			return discoveredReducerInfos;
		}

		internal static IEnumerable<DiscoveredReducerMethodInfo> DiscoverReducerMethods(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredReducerMethodInfo> discoveredReducerMethods = allCandidateTypes
				.SelectMany(t =>
					t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Select(m => new
					{
						ImplementingType = t,
						Method = m,
						Attribute = m.GetCustomAttribute<ReducerMethodAttribute>()
					})
				)
				.Where(x => x.Attribute != null)
				.Select(x => new DiscoveredReducerMethodInfo(
					implementingType: x.ImplementingType,
					reducerMethodInfo: x.Method,
					options: x.Attribute.Options));

			var implementingTypes = discoveredReducerMethods
				.Select(x => x.ImplementingType)
				.Distinct();

			foreach (Type implementingType in implementingTypes)
				serviceCollection.AddScoped(implementingType);

			return discoveredReducerMethods;
		}

	}
}
