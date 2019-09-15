using System;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class DiscoveredEffect
	{
		public readonly Type HostClassType;
		public readonly MethodInfo MethodInfo;
		public readonly Type ActionType;
		public readonly EffectOptions Options;

		public DiscoveredEffect(Type hostClassType, MethodInfo methodInfo, Type actionType, EffectOptions options)
		{
			HostClassType = hostClassType;
			MethodInfo = methodInfo;
			ActionType = actionType;
			Options = options;
		}
	}
}
