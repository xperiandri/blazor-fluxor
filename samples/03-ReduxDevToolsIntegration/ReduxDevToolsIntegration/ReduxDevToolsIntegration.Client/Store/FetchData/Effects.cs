﻿using Blazor.Fluxor;
using Microsoft.AspNetCore.Components;
using ReduxDevToolsIntegration.Shared;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReduxDevToolsIntegration.Client.Store.FetchData
{
	public class Effects
	{
		private readonly HttpClient HttpClient;

		public Effects(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		[Effect]
		public async Task HandleGetForecastDataActionAsync(GetForecastDataAction action, IDispatcher dispatcher)
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
