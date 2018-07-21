using System;

namespace Blazor.Fluxor
{
	/// <summary>
	/// An interface that is injected into Blazor Components / pages for accessing
	/// the state of an <see cref="IFeature{TState}"/>
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Executed when the state changes
		/// </summary>
		/// <param name="observer">The observer to call back</param>
		/// <param name="callback">The callback to execute</param>
		void Changed<TObserver>(TObserver observer, Action callback);
	}

	/// <summary>
	/// An interface that is injected into Blazor Components / pages for accessing
	/// the state of an <see cref="IFeature{TState}"/>
	/// </summary>
	/// <typeparam name="TState">The type of the state</typeparam>
	public interface IState<TState> : IState
	{
		/// <summary>
		/// Returns the current state of the feature
		/// </summary>
		TState Value { get; }
	}
}
