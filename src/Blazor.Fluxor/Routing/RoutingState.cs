using System;

namespace Blazor.Fluxor.Routing
{
	/// <summary>
	/// State used by <see cref="RoutingMiddleware"/> and <see cref="RoutingFeature"/>.
	/// </summary>
	public class RoutingState
	{
		/// <summary>
		/// The browser address
		/// </summary>
		public string Uri { get; set; } // TODO: Make setter private https://github.com/aspnet/Blazor/issues/705

		[Obsolete("For deserialization purposes only. Use the constructor with parameters")]
		public RoutingState() { } // Required by DevTools to recreate historic state

		public RoutingState(string uri)
		{
			Uri = uri;
		}
	}
}
