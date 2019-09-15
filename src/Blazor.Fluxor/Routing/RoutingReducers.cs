using Blazor.Fluxor.AutoDiscovery;

namespace Blazor.Fluxor.Routing
{
	public static class RoutingReducers
	{
		[Reducer]
		public static RoutingState Reduce(RoutingState state, Go action) =>
			new RoutingState(action.NewUri ?? "");
	}
}
