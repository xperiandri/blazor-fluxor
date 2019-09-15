using Blazor.Fluxor;

namespace ReduxDevToolsIntegration.Client.Store.Counter
{
	public static class Reducers
	{
		[Reducer]
		public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action) =>
			new CounterState(state.ClickCount + 1);
	}
}
