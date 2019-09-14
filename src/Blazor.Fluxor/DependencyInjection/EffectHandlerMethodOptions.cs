using System;

namespace Blazor.Fluxor.DependencyInjection
{
	/// <summary>
	/// Options for <see cref="EffectHandlerMethodAttribute"/>
	/// </summary>
	[Flags]
	public enum EffectHandlerMethodOptions
	{
		/// <summary>
		/// No options
		/// </summary>
		None = 0,
		/// <summary>
		/// React to the specified action type and also any action types assignable to the specified action type
		/// </summary>
		HandleDescendantActions
	}
}
