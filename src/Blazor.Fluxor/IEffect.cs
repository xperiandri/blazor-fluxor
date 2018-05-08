using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	/// <summary>
	/// Classes that implement this interface may be registered with the store. Whenever an action is dispatched
	/// the store will execute all effects registered as observers of type of action dispatched
	/// <seealso cref="IStore.AddEffect(System.Type, IEffect)"/>
	/// </summary>
	public interface IEffect
	{
		/// <summary>
		/// This method is executed by the store immediately after an action is dispatched. You must first
		/// indicate which action type this effect is interested in by calling <see cref="IStore.AddEffect(System.Type, IEffect)"/>
		/// </summary>
		/// <param name="action"></param>
		/// <returns>An array of actions that the effect wants the store to process. This can be null or empty.</returns>
		Task<IAction[]> HandleAsync(IAction action);
	}

	/// <summary>
	/// A generic implementation of <see cref="IEffect"/>
	/// </summary>
	/// <typeparam name="TAction">The action type that triggers this effect when the store dispatches</typeparam>
	public interface IEffect<TAction>: IEffect
	  where TAction : IAction
	{
		/// <summary>
		/// This method is executed by the store immediately after an action is dispatched. You must first
		/// indicate which action type this effect is interested in by calling <see cref="IStore.AddEffect(System.Type, IEffect)"/>
		/// </summary>
		/// <param name="action"></param>
		/// <returns>An array of actions that the effect wants the store to process. This can be null or empty.</returns>
		Task<IAction[]> HandleAsync(TAction action);
	}
}
