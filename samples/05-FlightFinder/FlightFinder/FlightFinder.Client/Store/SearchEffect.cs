using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
    public class SearchEffect : Effect<SearchAction>
    {
        private readonly HttpClient httpClient;
        private readonly IDispatcher dispatcher;

        public SearchEffect(HttpClient httpClient, IDispatcher dispatcher)
        {
            this.httpClient = httpClient;
            this.dispatcher = dispatcher;
        }

        protected async override Task HandleAsync(SearchAction action)
        {
            try
            {
                Itinerary[] searchResults = await httpClient.PostJsonAsync<Itinerary[]>("api/flightsearch", action.SearchCriteria);
                dispatcher.Dispatch(new SearchCompleteAction(searchResults));
            }
            catch
            {
                // Should really dispatch an error action
                dispatcher.Dispatch(new SearchCompleteAction(null));
            }
        }
    }
}
