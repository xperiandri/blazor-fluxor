using Blazor.Fluxor.AutoDiscovery;

namespace CounterSample.Client.Store.Counter
{
	public static class IncrementCounterReducer
	{
		[Reducer]
		public static CounterState Reduce(CounterState state, IncrementCounterAction action) =>
			new CounterState(state.ClickCount + 1);
	}
}
