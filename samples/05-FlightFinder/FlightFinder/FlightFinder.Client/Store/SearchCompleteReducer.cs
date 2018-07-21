using Blazor.Fluxor;
using FlightFinder.Shared;
using System.Collections.Generic;

namespace FlightFinder.Client.Store
{
	public class SearchCompleteReducer : Reducer<AppState, SearchCompleteAction>
	{
		public override AppState Reduce(AppState state, SearchCompleteAction action)
		{
			return new AppState(
				searchInProgress: false,
				searchResults: action.SearchResults,
				shortlist: state.Shortlist,
				airports: state.Airports);
		}
	}
}
