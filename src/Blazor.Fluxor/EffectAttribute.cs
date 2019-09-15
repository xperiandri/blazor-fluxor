using System;

namespace Blazor.Fluxor
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class EffectAttribute : Attribute
	{
		public readonly EffectOptions Options;

		public EffectAttribute(EffectOptions options = EffectOptions.None)
		{
			Options = options;
		}
	}
}
