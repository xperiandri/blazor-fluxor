﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
	internal class EffectClassessDiscovery
	{
		internal static IEnumerable<DiscoveredEffectClass> DiscoverEffectClasses(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredEffectClass> discoveredEffectInfos = allCandidateTypes
				.Where(t => typeof(IEffect).IsAssignableFrom(t))
				.Select(t => new DiscoveredEffectClass(implementingType: t))
				.ToList();

			foreach (DiscoveredEffectClass discoveredEffectInfo in discoveredEffectInfos)
				serviceCollection.AddScoped(discoveredEffectInfo.ImplementingType);

			return discoveredEffectInfos;
		}
	}
}
