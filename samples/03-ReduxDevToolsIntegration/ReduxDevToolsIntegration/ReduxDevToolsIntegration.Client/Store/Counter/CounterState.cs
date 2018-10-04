using System;

namespace ReduxDevToolsIntegration.Client.Store.Counter
{
	public class CounterState
	{
		public int ClickCount { get; private set; } 

		// Required for deserialisation
		private CounterState() { }

		public CounterState(int clickCount)
		{
			ClickCount = clickCount;
		}
	}
}