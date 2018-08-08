using ReduxDevToolsIntegration.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReduxDevToolsIntegration.Client.Store.FetchData
{
	public class FetchDataState
	{
		public bool IsLoading { get; set; }  // TODO: Make setter private https://github.com/aspnet/Blazor/issues/705
		public string ErrorMessage { get; set; }  // TODO: Make setter private https://github.com/aspnet/Blazor/issues/705
		public WeatherForecast[] Forecasts { get; set; }  // TODO: Make setter private https://github.com/aspnet/Blazor/issues/705

		[Obsolete("For deserialization purposes only. Use the constructor with parameters")]
		public FetchDataState() { }

		public FetchDataState(bool isLoading, string errorMessage, IEnumerable<WeatherForecast> forecasts)
		{
			IsLoading = isLoading;
			ErrorMessage = errorMessage;
			Forecasts = forecasts == null ? null : forecasts.ToArray();
		}
	}
}