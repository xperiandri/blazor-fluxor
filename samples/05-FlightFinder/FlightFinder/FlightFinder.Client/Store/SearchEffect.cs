using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Blazor;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class SearchEffect : Effect<SearchAction>
	{
		private readonly HttpClient HttpClient;

		public SearchEffect(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public override async Task<IAction[]> HandleAsync(SearchAction action)
		{
			try
			{
				Itinerary[] searchResults = await HttpClient.PostJsonAsync<Itinerary[]>("/api/flightsearch", action.SearchCriteria);
				return new IAction[] { new SearchCompleteAction(searchResults) };
			}
			catch
			{
				// Should really dispatch an error action
				return new IAction[] { new SearchCompleteAction(null) };
			}
		}
	}
}
