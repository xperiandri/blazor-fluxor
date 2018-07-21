using System;
using Blazor.Fluxor;
using FlightFinder.Shared;

namespace FlightFinder.Client.Store
{
	public class SearchCompleteAction : IAction
	{
		public readonly Itinerary[] SearchResults;

		public SearchCompleteAction(Itinerary[] searchResults)
		{
			SearchResults = searchResults ?? Array.Empty<Itinerary>();
		}
	}
}
