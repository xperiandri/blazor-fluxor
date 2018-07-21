using Blazor.Fluxor;
using System.Collections.Generic;

namespace FlightFinder.Client.Store
{
	public class SearchReducer : Reducer<AppState, SearchAction>
	{
		public override AppState Reduce(AppState state, SearchAction action)
		{
			return new AppState(
				searchInProgress: true,
				searchResults: new List<Shared.Itinerary>(),
				shortlist: state.Shortlist,
				airports: state.Airports);
		}
	}
}
