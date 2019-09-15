using System;
using System.Reflection;

namespace Blazor.Fluxor
{
	public interface IReducerFuncs
	{
		bool ShouldReduceStateForAction(object action);
		object Reduce(object state, object action);
	}

	internal sealed class ReflectedReducerFuncs<TState, TAction> : IReducerFuncs
	{
		private delegate TState ReduceHandler(TState state, TAction action);
		private delegate bool ShouldReduceStateForActionHandler(object action);
		private readonly ReduceHandler Reduce;
		private readonly ShouldReduceStateForActionHandler ShouldReduceStateForAction;

		public static IReducerFuncs Create(IServiceProvider serviceProvider, MethodInfo methodInfo, ReducerOptions options)
		{
			ValidateMethod(methodInfo);
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();
			Type stateType = parameterInfos[0].ParameterType;
			Type actionType = parameterInfos[1].ParameterType;

			Type hostClassType = methodInfo.DeclaringType;
			object effectHostInstance = methodInfo.IsStatic
				? null
				: serviceProvider.GetService(hostClassType);

			Type classGenericType = typeof(ReflectedReducerFuncs<,>).MakeGenericType(stateType, actionType);
			var result = (IReducerFuncs)Activator.CreateInstance(classGenericType, effectHostInstance, methodInfo, options);
			return result;
		}

		public ReflectedReducerFuncs(object effectHostInstance, MethodInfo methodInfo, ReducerOptions options)
		{
			if (effectHostInstance == null)
			{
				// Static method
				Reduce = (ReduceHandler)
					Delegate.CreateDelegate(
						type: typeof(ReduceHandler),
						method: methodInfo);
			}
			else
			{
				// Instance method
				Reduce = (ReduceHandler)
					Delegate.CreateDelegate(
						type: typeof(ReduceHandler),
						firstArgument: effectHostInstance,
						method: methodInfo);
			}

			if (!options.HasFlag(ReducerOptions.HandleDescendants))
				ShouldReduceStateForAction = (action) => action.GetType() == typeof(TAction);
			else
				ShouldReduceStateForAction = (action) => typeof(TAction).IsAssignableFrom(action.GetType());
		}

		bool IReducerFuncs.ShouldReduceStateForAction(object action) =>
			ShouldReduceStateForAction(action);

		object IReducerFuncs.Reduce(object state, object action) =>
			Reduce((TState)state, (TAction)action);

		private static bool ValidateMethod(MethodInfo methodInfo)
		{
			if (methodInfo == null)
				throw new ArgumentNullException(nameof(methodInfo));

			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 2
				|| parameters[0].ParameterType != methodInfo.ReturnType)
			{
				throw new InvalidOperationException(
					"ReducerAttribute can only decorate methods in the format\r\n" +
					"public {TypeOfState} {NameOfMethod}({TypeOfState} state, {TypeOfAction} action)");
			}
			return true;
		}
	}
}
