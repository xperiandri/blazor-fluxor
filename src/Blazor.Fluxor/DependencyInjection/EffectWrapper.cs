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

		public EffectWrapper(Store store, IServiceProvider serviceProvider, object effectHostInstance, MethodInfo methodInfo)
		{
			var parametersInfos = methodInfo.GetParameters();
			var parametersCache = new object[parametersInfos.Length];
			for (var i = 0; i < parametersInfos.Length; i++)
			{
				Type parameterType = parametersInfos[i].ParameterType;
				if (parameterType == typeof(TAction))
					continue;
				if (parameterType.IsAssignableFrom(store.GetType()))
					parametersCache[i] = store;
				else
					parametersCache[i] = serviceProvider.GetService(parameterType);
			}

			Task Handle(TAction action)
			{
				var parameters = new object[parametersInfos.Length];
				parametersCache.CopyTo(parameters, 0);
				for (var i = 0; i < parametersInfos.Length; i++)
				{
					Type parameterType = parametersInfos[i].ParameterType;
					if (parameterType == typeof(TAction))
					{
						parameters[i] = action;
						break;
					}
				}
				return (Task)methodInfo.Invoke(effectHostInstance, parameters);
			}

			HandleAsync = Handle;
		}
	}
}
