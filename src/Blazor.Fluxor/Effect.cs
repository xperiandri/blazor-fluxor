using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	/// <summary>
	/// A generic class that can be used as a base for effects.
	/// </summary>
	/// <typeparam name="TTriggerAction"></typeparam>
	public abstract class Effect<TTriggerAction> : IEffect
	  where TTriggerAction : IAction
	{
		/// <summary>
		/// <see cref="IEffect.HandleAsync(IAction)"/>
		/// </summary>
		public abstract Task<IAction[]> HandleAsync(TTriggerAction action);
		/// <summary>
		/// <see cref="IEffect.ShouldReactToAction(IAction)"/>
		/// </summary>
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
