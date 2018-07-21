using Blazor.Fluxor;
using FlightFinder.Shared;
using System.Collections.Generic;

namespace FlightFinder.Client.Store
{
	public class RemoveFromShortlistReducer : Reducer<AppState, RemoveFromShortlistAction>
	{
		public override AppState Reduce(AppState state, RemoveFromShortlistAction action)
		{
			var newShortList = new List<Itinerary>(state.Shortlist);
			newShortList.Remove(action.Itinerary);
			return new AppState(
				searchInProgress: state.SearchInProgress,
				searchResults: state.SearchResults,
				shortlist: newShortList,
				airports: state.Airports);
		}
	}
}
