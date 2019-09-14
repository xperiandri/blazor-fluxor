using System;

namespace Blazor.Fluxor.DependencyInjection
{
	/// <summary>
	/// Options for <see cref="ReducerMethodAttribute"/>
	/// </summary>
	[Flags]
	public enum ReducerMethodOptions
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
