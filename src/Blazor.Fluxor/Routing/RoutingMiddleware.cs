using Microsoft.AspNetCore.Blazor.Services;

namespace Blazor.Fluxor.Routing
{
	public class RoutingMiddleware : Middleware
	{
		private readonly IUriHelper UriHelper;
		private readonly IFeature<RoutingState> Feature;

		public RoutingMiddleware(IUriHelper uriHelper, IFeature<RoutingState> feature)
		{
			UriHelper = uriHelper;
			Feature = feature;
			UriHelper.OnLocationChanged += OnLocationChanged;
		}

		public override void Initialize(IStore store)
		{
			base.Initialize(store);
			// If the URL changed before we initialized then dispatch an action
			Store.DispatchAsync(new Go(UriHelper.GetAbsoluteUri())).Wait();
		}

		protected override void OnInternalMiddlewareChangeEnding()
		{
			if (Feature.State.Uri != UriHelper.GetAbsoluteUri())
				UriHelper.NavigateTo(Feature.State.Uri);
		}

		private void OnLocationChanged(object sender, string e)
		{
			string fullUri = UriHelper.ToAbsoluteUri(e).ToString();
			if (Store != null && !IsInsideMiddlewareChange && fullUri != Feature.State.Uri)
				Store.DispatchAsync(new Go(e)).Wait();
		}


	}
}
