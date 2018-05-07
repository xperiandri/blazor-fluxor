using Blazor.Fluxor;

namespace MiddlewareSample.Client.Store.Counter
{
	public class CounterFeature : Feature<CounterState>
	{
		public override string GetName() => "Home";
		protected override CounterState GetInitialState() => new CounterState(0);
	}
}
