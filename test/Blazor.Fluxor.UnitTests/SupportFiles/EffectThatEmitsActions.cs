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
		public override Task<IAction[]> HandleAsync(TTriggerAction action)
		{
			return Task.FromResult(ActionsToEmit);
		}
	}
}
