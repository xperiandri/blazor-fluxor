using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
	public class EffectThatEmitsActions<TTriggerAction> : Effect<TTriggerAction>
	public class EffectThatEmitsActions<TTriggerAction> : Effect<TTriggerAction>
		where TTriggerAction: IAction
	{
		public readonly IAction[] ActionsToEmit;

		public EffectThatEmitsActions(IAction[] actionsToEmit)
		{
			ActionsToEmit = actionsToEmit ?? new IAction[0];
		}
		public override Task<IAction[]> HandleAsync(TTriggerAction action)
		{
			return Task.FromResult(ActionsToEmit);
		}
	}
}
