using Blazor.Fluxor;
using FlightFinder.Shared;
using System.Collections.Generic;

namespace FlightFinder.Client.Store
{
	public class AddToShortlistReducer : Reducer<AppState, AddToShortlistAction>
	{
		public override AppState Reduce(AppState state, AddToShortlistAction action)
		{
			var newShortlist = new List<Itinerary>(state.Shortlist);
			newShortlist.Add(action.Itinerary);
			return new AppState(
				searchInProgress: state.SearchInProgress,
				searchResults: state.SearchResults,
				shortlist: newShortlist,
				airports: state.Airports);
		}
	}
}
