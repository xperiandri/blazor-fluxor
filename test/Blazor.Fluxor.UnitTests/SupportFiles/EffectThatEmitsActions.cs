using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
	public class EffectThatEmitsActions<TTriggerAction> : Effect<TTriggerAction>
	{
		public readonly object[] ActionsToEmit;

		public EffectThatEmitsActions(object[] actionsToEmit)
		{
			ActionsToEmit = actionsToEmit ?? Array.Empty<object>();
		}

		public EffectFuncs ToEffectFuncs()
		{
			return new EffectFuncs(
				shouldReactToAction: ShouldReactToAction,
				handleAsync: (action, dispatcher) => HandleAsync((TTriggerAction)action, dispatcher));
		}

		protected override Task HandleAsync(TTriggerAction action, IDispatcher dispatcher)
		{
			foreach (object actionToEmit in ActionsToEmit)
				dispatcher.Dispatch(actionToEmit);
			return Task.CompletedTask;
		}
	}

	public static class EffectThatEmitsActionsExtensions
	{
		public static EffectFuncs ToEffectFuncs(this IEffect effect) =>
			new EffectFuncs(effect.ShouldReactToAction, effect.HandleAsync);
	}
}
