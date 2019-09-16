using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal class EffectsRegistration
	{
		internal static IEnumerable<DiscoveredEffectClass> DiscoverEffects(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredEffectClass> discoveredEffectInfos = allCandidateTypes
				.Where(t => typeof(IEffect).IsAssignableFrom(t))
				.Select(t => new DiscoveredEffectClass(implementingType: t))
				.ToList();

			foreach (DiscoveredEffectClass discoveredEffectInfo in discoveredEffectInfos)
			{
				RegisterEffect(serviceCollection, discoveredEffectInfo);
			}

			return discoveredEffectInfos;
		}

		private static void RegisterEffect(IServiceCollection serviceCollection, DiscoveredEffectClass discoveredEffectInfo)
		{
			// Register the effect class
			serviceCollection.AddScoped(discoveredEffectInfo.ImplementingType);
		}
	}
}
