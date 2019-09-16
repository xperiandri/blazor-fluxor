using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class EffectWrapper<TAction> : IEffect
	{
		private delegate Task HandleAsyncHandler(TAction action, IDispatcher dispatcher);
		private readonly HandleAsyncHandler HandleAsync;

		Task IEffect.HandleAsync(object action, IDispatcher dispatcher) => HandleAsync((TAction)action, dispatcher);
		bool IEffect.ShouldReactToAction(object action) => action is TAction;

		public static IEffect Create(IServiceProvider serviceProvider, DiscoveredEffectMethod discoveredEffectMethod)
		{
			ValidateMethod(discoveredEffectMethod.MethodInfo);
			Type actionType = discoveredEffectMethod.ActionType;

			Type hostClassType = discoveredEffectMethod.HostClassType;
			object effectHostInstance = discoveredEffectMethod.MethodInfo.IsStatic
				? null
				: serviceProvider.GetService(hostClassType);

			Type classGenericType = typeof(EffectWrapper<>).MakeGenericType(actionType);
			var result = (IEffect)Activator.CreateInstance(
				classGenericType,
				effectHostInstance,
				discoveredEffectMethod.MethodInfo);
			return result;
		}

		public EffectWrapper(object effectHostInstance, MethodInfo methodInfo)
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
		}

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
