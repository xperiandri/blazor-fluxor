using System;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class DiscoveredReducerMethodInfo
	{
		public readonly Type ImplementingType;
		public readonly Type StateType;
		public readonly Type ActionType;
		public readonly MethodInfo ReducerMethodInfo;
		public readonly ReducerMethodOptions Options;

		public DiscoveredReducerMethodInfo(
			Type implementingType,
			MethodInfo reducerMethodInfo,
			ReducerMethodOptions options)
		{
			ReducerMethodValidator.ValidateMethodInfoStructure(implementingType, reducerMethodInfo);
			ImplementingType = implementingType;
			ParameterInfo[] parameterInfos = reducerMethodInfo.GetParameters();
			StateType = parameterInfos[0].ParameterType;
			ActionType = parameterInfos[1].ParameterType;
			ReducerMethodInfo = reducerMethodInfo;
			Options = options;
		}
	}
}
