using Blazor.Fluxor;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
    public class StoreInitializedEffect : Effect<StoreInitializedAction>
    {
        private readonly HttpClient httpClient;
        private readonly IDispatcher dispatcher;

        public StoreInitializedEffect(HttpClient httpClient, IDispatcher dispatcher)
        {
            this.httpClient = httpClient;
            this.dispatcher = dispatcher;
        }

        protected override Task HandleAsync(StoreInitializedAction action)
        {
            dispatcher.Dispatch(new FetchAirportsAction());
            return Task.CompletedTask;
        }
    }
}
