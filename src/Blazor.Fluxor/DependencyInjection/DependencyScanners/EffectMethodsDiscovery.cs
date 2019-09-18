using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal static class EffectMethodsDiscovery
	{
		internal static IEnumerable<DiscoveredEffectMethod> DiscoverEffectMethods(IServiceCollection serviceCollection,
			IEnumerable<Type> allCandidateTypes)
		{
			var discoveredEffects =
				from method in allCandidateTypes
				.SelectMany(t => t.GetMethods(BindingFlags.Public
				                            | BindingFlags.NonPublic
											| BindingFlags.Instance
											| BindingFlags.Static))
				let effectAttribute = method.GetCustomAttribute<EffectMethodAttribute>(false)
				where effectAttribute != null
				let actionType = effectAttribute.ActionType
					?? method.GetParameters().First(
						p => p.ParameterType.FullName.LastIndexOf("Action") > -1).ParameterType
					?? throw new InvalidOperationException($"Reducer decorated with {nameof(EffectMethodAttribute)} must either specify action type within attribute property or has parameter with full type name containing \"Action\" string.")
				select new DiscoveredEffectMethod(
					hostClassType: method.DeclaringType,
					methodInfo: method,
					actionType: actionType);

			IEnumerable<Type> hostClassTypes = discoveredEffects
				.Select(x => x.HostClassType)
				.Where(t => !t.IsAbstract)
				.Distinct();

			foreach (Type hostClassType in hostClassTypes)
				if (!hostClassType.IsAbstract)
					serviceCollection.AddScoped(hostClassType);

			return discoveredEffects;
		}
	}
}
