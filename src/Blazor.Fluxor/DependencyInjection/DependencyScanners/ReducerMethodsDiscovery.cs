using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class ReducerMethodsDiscovery
	{
		internal static IEnumerable<DiscoveredReducerMethod> DiscoverReducerMethods(
			IServiceCollection serviceCollection,
			IEnumerable<Type> allCandidateTypes)
		{
			var discoveredReducers =
				from method in allCandidateTypes
				.SelectMany(t => t.GetMethods(BindingFlags.Public
											| BindingFlags.NonPublic
											| BindingFlags.Instance
											| BindingFlags.Static))
				let reducerAttribute = method.GetCustomAttribute<ReducerMethodAttribute>(false)
				where reducerAttribute != null
				let parameters = method.GetParameters()
				let error = parameters.Length <= 2
					      ? false
						  : throw new NotSupportedException($"Reducer decorated with {nameof(ReducerMethodAttribute)} must either require state and action parameters or just single Action parameter and has no other parameters.")
				let stateType = reducerAttribute.StateType
					?? parameters.FirstOrDefault(
						p => p.ParameterType.FullName.LastIndexOf("State") > -1).ParameterType
					?? throw new InvalidOperationException($"Reducer decorated with {nameof(ReducerMethodAttribute)} must either specify state type within attribute property or has parameter with full type name containing \"State\" string.")
				let actionType = reducerAttribute.ActionType
					?? parameters.First(
						p => p.ParameterType.FullName.LastIndexOf("Action") > -1).ParameterType
					?? throw new InvalidOperationException($"Reducer decorated with {nameof(ReducerMethodAttribute)} must either specify action type within attribute property or has parameter with full type name containing \"Action\" string.")
				let returnTypeMatches = stateType.IsAssignableFrom(method.ReturnType)
								      ? true
									  : throw new NotSupportedException($"Reducer must return instance of state type or type derived from it. But state type is {stateType} and return type is {method.ReturnType}.")
				select new DiscoveredReducerMethod(
					hostClassType: method.DeclaringType,
					methodInfo: method,
					stateType: stateType,
					actionType: actionType);

			IEnumerable<Type> hostClassTypes = discoveredReducers
				.Select(x => x.HostClassType)
				.Where(t => !t.IsAbstract)
				.Distinct();

			foreach (Type hostClassType in hostClassTypes)
				if (!hostClassType.IsAbstract)
					serviceCollection.AddScoped(hostClassType);

			return discoveredReducers;
		}
	}
}
