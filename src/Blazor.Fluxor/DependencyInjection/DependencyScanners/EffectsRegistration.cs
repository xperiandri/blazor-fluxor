using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor.DependencyInjection.DependencyScanners
{
    internal class EffectsRegistration
    {
		internal static IEnumerable<DiscoveredEffectInfo> DiscoverEffects(
			IServiceCollection serviceCollection, IEnumerable<Type> allCandidateTypes)
		{
			IEnumerable<DiscoveredEffectInfo> discoveredEffectInfos = allCandidateTypes
				.Select(t => new
				{
					ImplementingType = t,
					GenericParameterTypes = TypeHelper.GetGenericParametersForImplementedInterface(t, typeof(IEffect<>))
				})
				.Where(x => x.GenericParameterTypes != null)
				.Select(x => new DiscoveredEffectInfo(
					implementingType: x.ImplementingType,
					actionType: x.GenericParameterTypes[0]
					)
				)
				.ToList();

			foreach (DiscoveredEffectInfo discoveredEffectInfo in discoveredEffectInfos)
			{
				RegisterEffect(serviceCollection, discoveredEffectInfo);
			}

			return discoveredEffectInfos;
		}

		private static void RegisterEffect(IServiceCollection serviceCollection, DiscoveredEffectInfo discoveredEffectInfo)
		{
			// Register the effect class against the generic IEffect<> interface
			serviceCollection.AddSingleton(
				serviceType: discoveredEffectInfo.EffectInterfaceGenericType,
				implementationType: discoveredEffectInfo.ImplementingType);
		}
	}
}
