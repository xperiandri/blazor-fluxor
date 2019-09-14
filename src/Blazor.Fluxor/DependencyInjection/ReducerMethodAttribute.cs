using System;

namespace Blazor.Fluxor.DependencyInjection
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
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ReducerMethodAttribute : Attribute
	{
		public readonly ReducerMethodOptions Options;

		///	<summary>
		///		Creates a new instance
		///	</summary>
		public ReducerMethodAttribute(ReducerMethodOptions options = ReducerMethodOptions.None)
		{
			Options = options;
		}
	}
}
