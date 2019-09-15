using System;

namespace Blazor.Fluxor
{
	[Flags]
	public enum ReducerOptions
	{
		None = 0,
		HandleDescendants = 1
	}
}
