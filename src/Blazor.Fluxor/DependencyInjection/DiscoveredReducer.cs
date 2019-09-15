using System;
using System.Reflection;
using Blazor.Fluxor.AutoDiscovery;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class DiscoveredReducer
	{
		public readonly Type HostClassType;
		public readonly MethodInfo MethodInfo;
		public readonly Type StateType;
		public readonly Type ActionType;
		public readonly ReducerOptions Options;

		public DiscoveredReducer(Type hostClassType, MethodInfo methodInfo, Type stateType, Type actionType, ReducerOptions options)
		{
			HostClassType = hostClassType;
			MethodInfo = methodInfo;
			StateType = stateType;
			ActionType = actionType;
			Options = options;
		}
	}
}
