using Blazor.Fluxor;

namespace MiddlewareSample.Client.Store.Counter.Increment
{
	public class IncrementCounterReducer : Reducer<CounterState, IncrementCounterAction>
	{
		public override CounterState Reduce(CounterState state, IncrementCounterAction action)
		{
			System.Console.WriteLine("State.Value=" + (state.ClickCount + 1));
			return new CounterState(state.ClickCount + 1);
		}
	}
}
