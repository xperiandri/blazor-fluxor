using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal class ReducersRegistration
	{
		internal static IEnumerable<DiscoveredReducer> DiscoverReducers(IServiceCollection serviceCollection,
			IEnumerable<Type> allCandidateTypes)
		{
			var discoveredReducers = allCandidateTypes
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
				.Select(m => new
				{
					MethodInfo = m,
					ReducerAttribute = m.GetCustomAttribute<ReducerAttribute>(false)
				})
				.Where(x => x.ReducerAttribute != null)
				.Select(x => new DiscoveredReducer(
					hostClassType: x.MethodInfo.DeclaringType,
					methodInfo: x.MethodInfo,
					stateType: x.MethodInfo.GetParameters()[0].ParameterType,
					actionType: x.MethodInfo.GetParameters()[1].ParameterType,
					options: x.ReducerAttribute.Options));

			IEnumerable<Type> hostClassTypes = discoveredReducers
				.Select(x => x.HostClassType)
				.Where(t => !t.IsAbstract)
				.Distinct();

			foreach (Type hostClassType in hostClassTypes)
				serviceCollection.AddScoped(hostClassType);

			return discoveredReducers;
		}
	}
}
