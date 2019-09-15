using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	public interface IEffectFuncs
	{
		bool ShouldReactToAction(object action);
		Task HandleAsync(object action, IDispatcher dispatcher);
	}

	internal sealed class ReflectedEffectFuncs<TAction> : IEffectFuncs
	{
		private delegate Task HandleAsyncHandler(TAction action, IDispatcher dispatcher);
		private delegate bool ShouldReactToActionHandler(object action);
		private readonly HandleAsyncHandler HandleAsync;
		private readonly ShouldReactToActionHandler ShouldReactToAction;

		public static IEffectFuncs Create(IServiceProvider serviceProvider, MethodInfo methodInfo, EffectOptions options)
		{
			ValidateMethod(methodInfo);
			Type actionType = methodInfo.GetParameters()[0].ParameterType;

			Type hostClassType = methodInfo.DeclaringType;
			object effectHostInstance = methodInfo.IsStatic
				? null
				: serviceProvider.GetService(hostClassType);

			Type classGenericType = typeof(ReflectedEffectFuncs<>).MakeGenericType(actionType);
			var result = (IEffectFuncs)Activator.CreateInstance(classGenericType, effectHostInstance, methodInfo, options);
			return result;
		}

		public ReflectedEffectFuncs(object effectHostInstance, MethodInfo methodInfo, EffectOptions options)
		{
			if (effectHostInstance == null)
			{
				// Static method
				HandleAsync = (HandleAsyncHandler)
					Delegate.CreateDelegate(
						type: typeof(HandleAsyncHandler),
						method: methodInfo);
			}
			else
			{
				// Instance method
				HandleAsync = (HandleAsyncHandler)
					Delegate.CreateDelegate(
						type: typeof(HandleAsyncHandler),
						firstArgument: effectHostInstance,
						method: methodInfo);
			}

			if (!options.HasFlag(EffectOptions.HandleDescendants))
				ShouldReactToAction = (action) => action.GetType() == typeof(TAction);
			else
				ShouldReactToAction = (action) => typeof(TAction).IsAssignableFrom(action.GetType());
		}

		bool IEffectFuncs.ShouldReactToAction(object action) =>
			ShouldReactToAction(action);

		Task IEffectFuncs.HandleAsync(object action, IDispatcher dispatcher) =>
			HandleAsync((TAction)action, dispatcher);


		private static bool ValidateMethod(MethodInfo methodInfo)
		{
			if (methodInfo == null)
				throw new ArgumentNullException(nameof(methodInfo));

			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 2
				|| !typeof(IDispatcher).IsAssignableFrom(parameters[1].ParameterType)
				|| methodInfo.ReturnType != typeof(Task))
			{
				throw new InvalidOperationException(
					"EffectAttribute can only decorate methods in the format\r\n" +
					"public Task {NameOfMethod}({TypeOfAction} action, IDispatcher dispatcher)");
			}
			return true;
		}

	}
}
