using System;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal static class ReducerMethodValidator
	{
		public static void ValidateMethodInfoStructure(Type implementingType, MethodInfo reducerMethodInfo)
		{
			if (reducerMethodInfo == null)
				throw new ArgumentNullException(nameof(reducerMethodInfo));

			ParameterInfo[] parameterInfos = reducerMethodInfo.GetParameters();

			if (reducerMethodInfo.ReturnType == typeof(void)
				|| parameterInfos.Length != 2
				|| parameterInfos[0].ParameterType != reducerMethodInfo.ReturnType)
			{
				throw new ArgumentException($"{implementingType.Name}.{reducerMethodInfo.Name} has the wrong method signature.\r\n"
					+ $"Methods decorated with [{nameof(ReducerMethodAttribute)}] " +
					"must be in the form `public TState ReducerMethodName(TState state, TAction action)`");
			}
		}
	}
}
