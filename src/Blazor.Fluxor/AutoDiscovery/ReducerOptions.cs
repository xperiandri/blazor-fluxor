using System;

namespace Blazor.Fluxor.AutoDiscovery
{
	[Flags]
	public enum ReducerOptions
	{
		None = 0,
		HandleDescendants = 1
	}
}
