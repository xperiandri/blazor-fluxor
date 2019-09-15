using Blazor.Fluxor.AutoDiscovery;
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

			foreach (DiscoveredEffect discoveredEffect in discoveredEffects)
				serviceCollection.AddScoped(discoveredEffect.HostClassType);

			return discoveredEffects;
		}

	}
}
