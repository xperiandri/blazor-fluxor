using Blazor.Fluxor.AutoDiscovery;

namespace ReduxDevToolsIntegration.Client.Store.FetchData
{
	public static class Reducers
	{
		[Reducer]
		public static FetchDataState ReduceGetForecastDataAction(FetchDataState state, GetForecastDataAction action) =>
			new FetchDataState(
				isLoading: true,
				errorMessage: null,
				forecasts: null);

		[Reducer]
		public static FetchDataState ReduceGetForecastDataFailedAction(FetchDataState state, GetForecastDataFailedAction action) =>
			new FetchDataState(
				isLoading: false,
				errorMessage: action.ErrorMessage,
				forecasts: null);

		[Reducer]
		public static FetchDataState ReduceGetForecastDataSuccessAction(FetchDataState state, GetForecastDataSuccessAction action) =>
			new FetchDataState(
				isLoading: false,
				errorMessage: null,
				forecasts: action.WeatherForecasts);
	}
}
