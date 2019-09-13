using Blazor.Fluxor.DependencyInjection;
using System;

namespace Blazor.Fluxor
{
	///	<summary>
	///		Identifies a method as a reducer that should be added to a feature
	///		during dependency injection discovery.
	///	<para>
	///		Using this approach means we can avoid writing a class per reducer
	///		<see cref="IReducer{TState}"/> and instead have a single class with
	///		multiple reducers. See https://github.com/mrpmorris/blazor-fluxor/issues/76
	///	</para>
	///	</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ReducerMethodAttribute : Attribute
	{
		public readonly bool HandleDescendants;

		///	<summary>
		///		Creates a new instance
		///	</summary>
		public ReducerMethodAttribute(ReducerMethodAttributeOptions options = ReducerMethodAttributeOptions.None)
		{
			HandleDescendants = (options & ReducerMethodAttributeOptions.HandleDescendantActions) != 0;
		}
	}

	/// <summary>
	/// Options for <see cref="ReducerMethodAttribute"/>
	/// </summary>
	[Flags]
	public enum ReducerMethodAttributeOptions
	{
		/// <summary>
		/// No options
		/// </summary>
		None = 0,
		/// <summary>
		/// Handle the specified action type and also any action types assignable to the specified action type
		/// </summary>
		HandleDescendantActions
	}
}
