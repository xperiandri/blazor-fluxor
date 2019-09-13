using System;
using System.Reflection;

namespace Blazor.Fluxor
{
	internal class ReducerMethodWrapper<TState> : IReducer<TState>
	{
		private readonly Type ActionType;
		private readonly Func<TState, object, TState> Reducer;
		private readonly bool HandleDescendantActions;

		public ReducerMethodWrapper(object instance, MethodInfo reducerMethodInfo, ReducerMethodAttributeOptions options)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			ValidateMethodInfoStructure(reducerMethodInfo);

			HandleDescendantActions = (options & ReducerMethodAttributeOptions.HandleDescendantActions) != 0;
			ActionType = reducerMethodInfo.GetParameters()[0].ParameterType;
			Reducer = CreateReducerFunction(instance, reducerMethodInfo);
		}

		public TState Reduce(TState state, object action)
		{
			throw new NotImplementedException();
		}

		public bool ShouldReduceStateForAction(object action)
		{
			if (action == null)
				return false;

			if (!HandleDescendantActions && action.GetType() != ActionType)
				return false;

			return ActionType.IsAssignableFrom(action.GetType());
		}

		private void ValidateMethodInfoStructure(MethodInfo reducerMethodInfo)
		{
			if (reducerMethodInfo == null)
				throw new ArgumentNullException(nameof(reducerMethodInfo));

			ParameterInfo[] parameterInfos = reducerMethodInfo.GetParameters();

			if (parameterInfos[0].ParameterType != typeof(TState)
				|| reducerMethodInfo.ReturnType == typeof(void)
				|| parameterInfos.Length != 2
				|| parameterInfos[0].ParameterType != reducerMethodInfo.ReturnType)
			{
				throw new ArgumentException($"Methods decorated with [{nameof(ReducerMethodAttribute)}] " +
					"must be in the form `public TState ReducerMethodName(TState state, TAction action)`");
			}
		}

		private Func<TState, object, TState> CreateReducerFunction(object instance, MethodInfo reducerMethodInfo)
		{
			throw new NotImplementedException();
		}
	}
}
