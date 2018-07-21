using Blazor.Fluxor;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class StoreInitializedEffect : Effect<StoreInitializedAction>
	{
		private readonly HttpClient HttpClient;

		public StoreInitializedEffect(HttpClient httpClient)
		{
			HttpClient = httpClient;
		}

		public override Task<IAction[]> HandleAsync(StoreInitializedAction action)
		{
			var fetchAirports = new FetchAirportsAction();
			return Task.FromResult(new IAction[] { fetchAirports });
		}
	}
}
