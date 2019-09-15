using Blazor.Fluxor;
using Blazor.Fluxor.AutoDiscovery;
using System.Threading.Tasks;

namespace FlightFinder.Client.Store
{
	public class StoreInitializedEffect
	{
		[Effect]
		public Task HandleAsync(StoreInitializedAction action, IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new FetchAirportsAction());
			return Task.CompletedTask;
		}
	}
}
