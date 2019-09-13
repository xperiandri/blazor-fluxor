using System;

namespace Blazor.Fluxor
{
	///	<summary>
	///		Identifies a method as an effect handler that should be added to a feature
	///		during dependency injection discovery.
	///	<para>
	///		Using this approach means we can avoid writing a class per reducer
	///		<see cref="IReducer{TState}"/> and instead have a single class with
	///		effect handlers. See https://github.com/mrpmorris/blazor-fluxor/issues/76
	///	</para>
	///	</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class EffectHandlerMethodAttribute : Attribute
	{
		public readonly bool HandleDescendants;

		///	<summary>
		///		Creates a new instance
		///	</summary>
		public EffectHandlerMethodAttribute(EffectHandlerMethodAttributeOptions options = EffectHandlerMethodAttributeOptions.None)
		{
			HandleDescendants = (options & EffectHandlerMethodAttributeOptions.HandleDescendantActions) != 0;
		}
	}

	/// <summary>
	/// Options for <see cref="EffectHandlerMethodAttribute"/>
	/// </summary>
	[Flags]
	public enum EffectHandlerMethodAttributeOptions
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
