using Blazor.Fluxor;
using FlightFinder.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class Effects
	{
		private readonly HttpClient HttpClient;

		public Effects(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		[Effect]
		public Task HandleStoreInitializedActionAsync(StoreInitializedAction action, IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new FetchAirportsAction());
			return Task.CompletedTask;
		}

		[Effect]
		public async Task HandleFetchAirportsActionAsync(FetchAirportsAction action, IDispatcher dispatcher)
		{
			Airport[] airports = Array.Empty<Airport>();
			try
			{
				airports = await HttpClient.GetJsonAsync<Airport[]>("api/airports");
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
