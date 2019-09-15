using Blazor.Fluxor.AutoDiscovery;

namespace ReduxDevToolsIntegration.Client.Store.Counter
{
	public static class Reducers
	{
		[Reducer]
		public static CounterState Reduce(CounterState state, IncrementCounterAction action) =>
			new CounterState(state.ClickCount + 1);
	}
}
