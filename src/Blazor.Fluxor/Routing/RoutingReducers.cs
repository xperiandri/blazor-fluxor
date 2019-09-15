namespace Blazor.Fluxor.Routing
{
	public static class RoutingReducers
	{
		[Reducer]
		public static RoutingState ReduceGo(RoutingState state, Go action) =>
			new RoutingState(action.NewUri ?? "");
	}
}
