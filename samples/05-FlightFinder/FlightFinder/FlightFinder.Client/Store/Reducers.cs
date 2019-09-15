using Blazor.Fluxor;
using FlightFinder.Shared;
using System.Collections.Generic;

namespace FlightFinder.Client.Store
{
	public static class Reducers
	{
		[Reducer]
		public static AppState ReduceAddToShortlistAction(AppState state, AddToShortlistAction action)
		{
			var newShortlist = new List<Itinerary>(state.Shortlist);
			newShortlist.Add(action.Itinerary);
			return new AppState(
				searchInProgress: state.SearchInProgress,
				searchResults: state.SearchResults,
				shortlist: newShortlist,
				airports: state.Airports);
		}

		[Reducer]
		public static AppState ReduceFetchAirportsCompleteAction(AppState state, FetchAirportsCompleteAction action) =>
			new AppState(
				searchInProgress: state.SearchInProgress,
				searchResults: state.SearchResults,
				shortlist: state.Shortlist,
				airports: action.Airports);

		[Reducer]
		public static AppState ReduceRemoveFromShortlistAction(AppState state, RemoveFromShortlistAction action)
		{
			var newShortList = new List<Itinerary>(state.Shortlist);
			newShortList.Remove(action.Itinerary);
			return new AppState(
				searchInProgress: state.SearchInProgress,
				searchResults: state.SearchResults,
				shortlist: newShortList,
				airports: state.Airports);
		}

		[Reducer]
		public static AppState ReduceSearchCompleteAction(AppState state, SearchCompleteAction action) =>
			new AppState(
				searchInProgress: false,
				searchResults: action.SearchResults,
				shortlist: state.Shortlist,
				airports: state.Airports);

		[Reducer]
		public static AppState ReduceSearchAction(AppState state, SearchAction action) =>
			new AppState(
				searchInProgress: true,
				searchResults: new List<Shared.Itinerary>(),
				shortlist: state.Shortlist,
				airports: state.Airports);
	}
}
