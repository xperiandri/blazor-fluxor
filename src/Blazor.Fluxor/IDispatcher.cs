using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	/// <summary>
	/// Interface that blazor components/pages should use to dispatch actions
	/// through the store
	/// </summary>
	public interface IDispatcher
	{
		/// <summary>
		/// Dispatches an action to all features added to the store and ensures all effects with a regstered
		/// interest in the action type are notified.
		/// </summary>
		/// <remarks>
		/// The return type is a Task because the store may also dispatch long-running side effects from 
		/// effects (<see cref="IEffect"/>). The caller should await the result of this method.
		/// </remarks>
		/// <param name="action">The action to dispatch to all features</param>
		/// <returns>An awaitable task</returns>
		Task DispatchAsync(IAction action);
	}
}
