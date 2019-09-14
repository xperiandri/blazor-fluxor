using Blazor.Fluxor;
using Blazor.Fluxor.DependencyInjection;

namespace MultiActionReducerSample.Client.Store.FetchData
{
	public class FetchDataReducers
	{
		[ReducerMethod]
		public FetchDataState Reduce(FetchDataState state, GetForecastDataAction action) =>
				new FetchDataState(
				isLoading: true,
				errorMessage: null,
				forecasts: null);

		[ReducerMethod]
		public FetchDataState Reduce(FetchDataState state, GetForecastDataFailedAction action) =>
				new FetchDataState(
					isLoading: false,
					errorMessage: action.ErrorMessage,
					forecasts: null);

		[ReducerMethod]
		public FetchDataState Reduce(FetchDataState state, GetForecastDataSuccessAction action) =>
				new FetchDataState(
					isLoading: false,
					errorMessage: null,
					forecasts: action.WeatherForecasts);
	}
}
