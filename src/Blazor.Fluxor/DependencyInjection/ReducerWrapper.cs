using System;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class ReducerWrapper<TState, TAction> : IReducer<TState>
	{
		private delegate TState ReduceHandler(TState state, TAction action);
		private readonly ReduceHandler Reduce;

		TState IReducer<TState>.Reduce(TState state, object action) => Reduce(state, (TAction)action);
		bool IReducer<TState>.ShouldReduceStateForAction(object action) => action is TAction;

		public ReducerWrapper(IServiceProvider serviceProvider, object reducerHostInstance, MethodInfo methodInfo)
		{
			var parametersInfos = methodInfo.GetParameters();
			//var commonParameters = new object[parametersInfos.Length];
			//for (var i = 0; i < parametersInfos.Length; i++)
			//{
			//	Type parameterType = parametersInfos[i].ParameterType;
			//	if (parameterType != typeof(TAction) && parameterType != typeof(TState))
			//		commonParameters[i] = serviceProvider.GetService(parameterType);
			//}

			TState Reducer(TState state, TAction action)
			{
				var parameters = new object[parametersInfos.Length];
				//commonParameters.CopyTo(parameters, 0);
				var actionParamerIndex = Array.FindIndex(parametersInfos, t => t.ParameterType == typeof(TAction));
				parameters[actionParamerIndex] = action;
				var stateParamerIndex = Array.FindIndex(parametersInfos, t => t.ParameterType == typeof(TState));
				if (stateParamerIndex >= 0)
					parameters[stateParamerIndex] = state;
				return (TState)methodInfo.Invoke(reducerHostInstance, parameters);
			}

			Reduce = Reducer;
		}
	}
}
