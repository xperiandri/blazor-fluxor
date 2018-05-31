using Blazor.Fluxor;

namespace MiddlewareSample.Client.Store.Counter.Increment
{
	public class IncrementCounterReducer : IReducer<CounterState, IncrementCounterAction>
	{
		public CounterState Reduce(CounterState state, IncrementCounterAction action)
		{
			System.Console.WriteLine("State.Value=" + (state.ClickCount + 1));
			return new CounterState(state.ClickCount + 1);
		}
	}
}
