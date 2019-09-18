using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class EffectWrapper<TAction> : IEffect
	{
		private delegate Task HandleAsyncHandler(TAction action);
		private readonly HandleAsyncHandler HandleAsync;

		Task IEffect.HandleAsync(object action) => HandleAsync((TAction)action);
		bool IEffect.ShouldReactToAction(object action) => action is TAction;

		public EffectWrapper(IServiceProvider serviceProvider, object effectHostInstance, MethodInfo methodInfo)
		{
			var parametersInfos = methodInfo.GetParameters();
			var commonParameters = new object[parametersInfos.Length];
			for (var i = 0; i < parametersInfos.Length; i++)
			{
				Type parameterType = parametersInfos[i].ParameterType;
				if (parameterType != typeof(TAction))
					commonParameters[i] = serviceProvider.GetService(parameterType);
			}

			Task Handle(TAction action)
			{
				var parameters = new object[parametersInfos.Length];
				commonParameters.CopyTo(parameters, 0);
				var actionParamerIndex = Array.FindIndex(parametersInfos, t => t.ParameterType == typeof(TAction));
				parameters[actionParamerIndex] = action;
				return (Task)methodInfo.Invoke(effectHostInstance, parameters);
			}

			HandleAsync = Handle;
		}
	}
}
