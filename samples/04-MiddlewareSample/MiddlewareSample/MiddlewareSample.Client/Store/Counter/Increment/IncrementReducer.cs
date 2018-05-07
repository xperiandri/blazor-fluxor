using Blazor.Fluxor;

namespace MiddlewareSample.Client.Store.Counter.Increment
{
	public class IncrementReducer : IReducer<CounterState, IncrementAction>
	{
		public CounterState Reduce(CounterState state, IncrementAction action)
		{
			System.Console.WriteLine("State.Value=" + (state.Value + 1));
			return new CounterState(state.Value + 1);
		}
	}
}
