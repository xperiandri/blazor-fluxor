using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
    public class FetchAirportsEffect : Effect<FetchAirportsAction>
    {
        private readonly HttpClient httpClient;
        private readonly IDispatcher dispatcher;

        public FetchAirportsEffect(HttpClient httpClient, IDispatcher dispatcher)
        {
            this.httpClient = httpClient;
            this.dispatcher = dispatcher;
        }

        protected async override Task HandleAsync(FetchAirportsAction action)
        {
            Airport[] airports = Array.Empty<Airport>();
            try
            {
                airports = await httpClient.GetJsonAsync<Airport[]>("api/airports");
            }
            catch
            {
                // Should really dispatch an error action
            }
            var completeAction = new FetchAirportsCompleteAction(airports);
            dispatcher.Dispatch(completeAction);
        }
    }
}
