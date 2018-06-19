using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	public abstract class Effect<TTriggerAction> : IEffect<TTriggerAction>
	  where TTriggerAction : IAction
	{
		public abstract Task<IAction[]> HandleAsync(TTriggerAction action);

		public bool ShouldReactToAction(IAction action)
		{
			return typeof(TTriggerAction).IsAssignableFrom(action.GetType());
		}

		Task<IAction[]> IEffect.HandleAsync(IAction action)
		{
			return HandleAsync((TTriggerAction)action);
		}
	}
}
