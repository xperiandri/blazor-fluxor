using Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using MultiActionReducerSample.Shared;
using Blazor.Fluxor.AutoDiscovery;

namespace MultiActionReducerSample.Client.Store.FetchData
{
	public class GetForecastDataEffect
	{
		private readonly HttpClient HttpClient;

		public GetForecastDataEffect(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		[Effect]
		public async Task HandleAsync(GetForecastDataAction action, IDispatcher dispatcher)
		{
			try
			{
				WeatherForecast[] forecasts =
					await HttpClient.GetJsonAsync<WeatherForecast[]>("api/SampleData/WeatherForecasts");
				dispatcher.Dispatch(new GetForecastDataSuccessAction(forecasts));
			}
			catch (Exception e)
			{
				dispatcher.Dispatch(new GetForecastDataFailedAction(errorMessage: e.Message));
			}
		}
	}
}
