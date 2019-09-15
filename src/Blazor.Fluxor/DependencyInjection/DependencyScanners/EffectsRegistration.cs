using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal class EffectsRegistration
	{
		internal static IEnumerable<DiscoveredEffect> DiscoverEffects(IServiceCollection serviceCollection,
			IEnumerable<Type> allCandidateTypes)
		{
			var discoveredEffects = allCandidateTypes
				.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
				.Select(m => new
				{
					MethodInfo = m,
					EffectAttribute = m.GetCustomAttribute<EffectAttribute>(false)
				})
				.Where(x => x.EffectAttribute != null)
				.Select(x => new DiscoveredEffect(
					hostClassType: x.MethodInfo.DeclaringType,
					methodInfo: x.MethodInfo,
					actionType: x.MethodInfo.GetParameters()[0].ParameterType,
					options: x.EffectAttribute.Options));

			IEnumerable<Type> hostClassTypes = discoveredEffects
				.Select(x => x.HostClassType)
				.Where(t => !t.IsAbstract)
				.Distinct();

			foreach (Type hostClassType in hostClassTypes)
				serviceCollection.AddScoped(hostClassType);

			return discoveredEffects;
		}
	}
}
