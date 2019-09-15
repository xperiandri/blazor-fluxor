using Blazor.Fluxor;

namespace CounterSample.Client.Store.Counter
{
	public static class IncrementCounterReducer
	{
		[Reducer]
		public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action) =>
			new CounterState(state.ClickCount + 1);
	}
}
