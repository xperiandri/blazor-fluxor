using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
	public class EffectThatEmitsActions<TTriggerAction> : Effect<TTriggerAction>
		where TTriggerAction: IAction
	{
		public readonly IAction[] ActionsToEmit;

		public EffectThatEmitsActions(IAction[] actionsToEmit)
		{
			ActionsToEmit = actionsToEmit ?? Array.Empty<IAction>();
		}
		protected override Task HandleAsync(TTriggerAction action, IDispatcher dispatcher)
		{
			foreach (IAction actionToEmit in ActionsToEmit)
				dispatcher.Dispatch(actionToEmit);
			return Task.CompletedTask;
		}
	}
}
