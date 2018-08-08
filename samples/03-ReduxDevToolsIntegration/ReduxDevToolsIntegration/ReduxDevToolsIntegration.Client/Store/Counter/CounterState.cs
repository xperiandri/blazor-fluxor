using System;

namespace ReduxDevToolsIntegration.Client.Store.Counter
{
	public class CounterState
	{
		public int ClickCount { get; set; }  // TODO: Make setter private https://github.com/aspnet/Blazor/issues/705

		[Obsolete("For deserialization purposes only. Use the constructor with parameters")]
		public CounterState() { }

		public CounterState(int clickCount)
		{
			ClickCount = clickCount;
		}
	}
}