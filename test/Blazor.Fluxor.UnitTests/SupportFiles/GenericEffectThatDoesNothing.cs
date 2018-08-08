using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
	public class GenericEffectThatDoesNothing<TTriggerAction> : Effect<TTriggerAction>
		where TTriggerAction : IAction
	{
		protected override Task HandleAsync(TTriggerAction action, IDispatcher dispatcher) => Task.CompletedTask;
	}
}
