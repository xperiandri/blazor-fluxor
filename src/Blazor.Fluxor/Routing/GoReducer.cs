namespace Blazor.Fluxor.Routing
{
	internal class GoReducer : IReducer<RoutingState, Go>
	{
		public RoutingState Reduce(RoutingState state, Go action)
		{
			return new RoutingState(action.NewUri);
		}
	}
}
