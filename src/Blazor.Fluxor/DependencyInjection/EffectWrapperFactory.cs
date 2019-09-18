using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Fluxor.DependencyInjection
{
	internal static class EffectWrapperFactory
	{
		internal static IEffect Create(IServiceProvider serviceProvider, DiscoveredEffectMethod discoveredEffectMethod)
		{
			Type actionType = discoveredEffectMethod.ActionType;
			//ValidateMethod(actionType, discoveredEffectMethod.MethodInfo);

			Type hostClassType = discoveredEffectMethod.HostClassType;
			Type classGenericType = typeof(EffectWrapper<>).MakeGenericType(actionType);
			object effectHostInstance = discoveredEffectMethod.MethodInfo.IsStatic
									? null
									: serviceProvider.GetService(hostClassType);
			return (IEffect)Activator.CreateInstance(
							classGenericType,
							serviceProvider, effectHostInstance, discoveredEffectMethod.MethodInfo);
		}

		private static bool ValidateMethod(Type actionType, MethodInfo methodInfo)
		{
			if (methodInfo == null)
				throw new ArgumentNullException(nameof(methodInfo));

			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (!parameters.Any(p => p.ParameterType == actionType))
				throw new InvalidOperationException($"Method {methodInfo.Name} must declare parameter of type {actionType} to be used with {nameof(EffectMethodAttribute)}");

			if (methodInfo.ReturnType != typeof(Task)/* || methodInfo.ReturnType != typeof(ValueTask)*/)
				throw new InvalidOperationException($"Method {methodInfo.Name} must return Task to be used with {nameof(EffectMethodAttribute)}");
			//throw new InvalidOperationException($"Method {methodInfo.Name} must return either Task or ValueTask to be used with {nameof(EffectMethodAttribute)}");

			return true;
		}
	}
}
