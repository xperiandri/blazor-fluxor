using FlightFinder.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightFinder.Client.Store
{
	public class AppState
	{
		public bool SearchInProgress { get; set; }
		public Itinerary[] SearchResults { get; set; }
		public Itinerary[] Shortlist { get; set; }
		public Airport[] Airports { get; set; }

		public AppState()
		{
			SearchResults = Array.Empty<Itinerary>();
			Shortlist = Array.Empty<Itinerary>();
			Airports = Array.Empty<Airport>();
		}

		public AppState(bool searchInProgress, IEnumerable<Itinerary> searchResults, IEnumerable<Itinerary> shortlist, IEnumerable<Airport> airports)
		{
			SearchInProgress = searchInProgress;
			SearchResults = searchResults?.ToArray() ?? Array.Empty<Itinerary>();
			Shortlist = shortlist?.ToArray() ?? Array.Empty<Itinerary>();
			Airports = airports?.ToArray() ?? Array.Empty<Airport>();
		}
	}
}
