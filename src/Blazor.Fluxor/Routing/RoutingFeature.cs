using Microsoft.AspNetCore.Blazor.Services;

namespace Blazor.Fluxor.Routing
{
	/// <summary>
	/// The feature required by <see cref="RoutingMiddleware"/> to store URL state
	/// </summary>
	public class RoutingFeature : Feature<RoutingState>
	{
		private readonly string InitialUrl;

		public override string GetName() => "@url";
		protected override RoutingState GetInitialState() => new RoutingState(InitialUrl);

		public RoutingFeature(IUriHelper uriHelper)
		{
			InitialUrl = uriHelper.GetAbsoluteUri();
		}

	}
}
