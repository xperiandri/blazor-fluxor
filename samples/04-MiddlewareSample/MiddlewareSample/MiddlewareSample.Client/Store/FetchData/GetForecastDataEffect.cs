using Blazor.Fluxor;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Components;
using MiddlewareSample.Shared;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiddlewareSample.Client.Store.FetchData
{
    public class GetForecastDataEffect : Effect<GetForecastDataAction>
    {
        private readonly HttpClient httpClient;
        private readonly IDispatcher dispatcher;

        public GetForecastDataEffect(HttpClient httpClient, IDispatcher dispatcher)
        {
            this.httpClient = httpClient;
            this.dispatcher = dispatcher;
        }

        protected async override Task HandleAsync(GetForecastDataAction action)
        {
            try
            {
                WeatherForecast[] forecasts =
                    await httpClient.GetJsonAsync<WeatherForecast[]>("api/SampleData/WeatherForecasts");
                dispatcher.Dispatch(new GetForecastDataSuccessAction(forecasts));
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(new GetForecastDataFailedAction(errorMessage: e.Message));
            }
        }
    }
}
