using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Blazor;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class FetchAirportsEffect : Effect<FetchAirportsAction>
	{
		private readonly HttpClient HttpClient;

		public FetchAirportsEffect(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public override async Task<IAction[]> HandleAsync(FetchAirportsAction action)
		{
			Airport[] airports = Array.Empty<Airport>();
			try
			{
				airports = await HttpClient.GetJsonAsync<Airport[]>("/api/airports");
			}
			catch
			{
				// Should really dispatch an error action
			}
			var completeAction = new FetchAirportsCompleteAction(airports);
			return new IAction[] { completeAction };
		}
	}
}
