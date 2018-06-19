using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
	public class GenericEffectThatDoesNothing<TTriggerAction> : Effect<TTriggerAction>
		where TTriggerAction : IAction
	{
		public override Task<IAction[]> HandleAsync(TTriggerAction action) => null;
	}
}
