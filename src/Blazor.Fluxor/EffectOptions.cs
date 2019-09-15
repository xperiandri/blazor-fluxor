using System;

namespace Blazor.Fluxor
{
	[Flags]
	public enum EffectOptions
	{
		None = 0,
		HandleDescendants = 1
	}
}
