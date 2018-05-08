namespace Blazor.Fluxor
{
	/// <summary>
	/// Identifies a class that is used to update state based on the execution of a specific action
	/// </summary>
	/// <typeparam name="TState">The class type of the state this reducer operates on</typeparam>
	/// <typeparam name="TAction">The class type of the action this reducer will react to</typeparam>
	public interface IReducer<TState, TAction>
	{
		/// <summary>
		/// Takes the current state and the action dispatched and returns a new state.
		/// </summary>
		/// <param name="state">The current state</param>
		/// <param name="action">The action dispatched via the store</param>
		/// <returns>The new state based on the current state + the changes the action should cause</returns>
		TState Reduce(TState state, TAction action);
	}
}
