namespace Blazor.Fluxor
{
	/// <summary>
	/// An interface that is injected into Blazor Components / pages for accessing
	/// the state of an <see cref="IFeature{TState}"/>
	/// </summary>
	/// <typeparam name="TState">The type of the state</typeparam>
	public interface IState<TState>
	{
		/// <summary>
		/// Returns the current state of the feature
		/// </summary>
		TState Current { get; }
	}
}
