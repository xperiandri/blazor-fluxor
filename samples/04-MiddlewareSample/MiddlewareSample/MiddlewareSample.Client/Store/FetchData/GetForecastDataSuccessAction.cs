using Blazor.Fluxor;
using MiddlewareSample.Shared;

namespace MiddlewareSample.Client.Store.FetchData
{
	public class GetForecastDataSuccessAction : IAction
	{
		public WeatherForecast[] WeatherForecasts { get; private set; }

		public GetForecastDataSuccessAction(WeatherForecast[] weatherForecasts)
		{
			WeatherForecasts = weatherForecasts;
		}
	}
}
