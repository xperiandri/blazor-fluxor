using System;

namespace Blazor.Fluxor.DependencyInjection
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class EffectHandlerMethodAttribute : Attribute
	{
		public readonly EffectHandlerMethodOptions Options;

		///	<summary>
		///		Creates a new instance
		///	</summary>
		public EffectHandlerMethodAttribute(EffectHandlerMethodOptions options = EffectHandlerMethodOptions.None)
		{
			Options = options;
		}
	}
}
