using Blazor.Fluxor.AutoDiscovery;
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

	internal sealed class TypedEffectFuncs<TAction> : IEffectFuncs
	{
		internal delegate Task HandleAsyncHandler(TAction action, IDispatcher dispatcher);
		internal delegate bool ShouldReactToActionHandler(object action);

		private readonly HandleAsyncHandler HandleAsync;
		private readonly ShouldReactToActionHandler ShouldReactToAction;

		public static IEffectFuncs Create(IServiceProvider serviceProvider, MethodInfo methodInfo, EffectOptions options)
		{
			ValidateMethod(methodInfo);
			Type actionType = methodInfo.GetParameters()[0].ParameterType;

			Type hostClassType = methodInfo.DeclaringType;
			object effectHostInstance = methodInfo.IsAbstract
				? null
				: serviceProvider.GetService(hostClassType);

			Type classGenericType = typeof(TypedEffectFuncs<>).MakeGenericType(actionType);
			var result = (IEffectFuncs)Activator.CreateInstance(classGenericType, effectHostInstance, methodInfo, options);
			return result;
		}

		public TypedEffectFuncs(object effectHostInstance, MethodInfo methodInfo, EffectOptions options)
		{
			if ((options & EffectOptions.HandleDescendants) == 0)
				ShouldReactToAction = (action) =>
				{
					System.Diagnostics.Debug.WriteLine("1");
					return action.GetType() == typeof(TAction);
				};
			else
				ShouldReactToAction = (action) =>
				{
					System.Diagnostics.Debug.WriteLine("1");
					return typeof(TAction).IsAssignableFrom(action.GetType());
				};

			HandleAsync = (HandleAsyncHandler)
				Delegate.CreateDelegate(
					type: typeof(HandleAsyncHandler),
					firstArgument: effectHostInstance,
					method: methodInfo);
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

		bool IEffectFuncs.ShouldReactToAction(object action) =>
			ShouldReactToAction((TAction)action);

		Task IEffectFuncs.HandleAsync(object action, IDispatcher dispatcher) =>
			HandleAsync((TAction)action, dispatcher);
	}
}
