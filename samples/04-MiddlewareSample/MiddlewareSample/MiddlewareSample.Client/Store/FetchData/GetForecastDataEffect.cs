using Blazor.Fluxor;
using Blazor.Fluxor.AutoDiscovery;
using Microsoft.AspNetCore.Components;
using MiddlewareSample.Shared;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiddlewareSample.Client.Store.FetchData
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
