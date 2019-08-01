using System.Threading.Tasks;
using Blazor.Fluxor;
using Blazor.Fluxor.Routing;
using FullStackSample.Api.Models;
using FullStackSample.Client.Services;
using FullStackSample.Client.Store.EntityStateEvents;
using FullStackSample.Client.Store.Main;

namespace FullStackSample.Client.Store.ClientCreate
{
	public class ClientCreateCommandEffect : Effect<Api.Requests.ClientCreateCommand>
	{
		private readonly IApiService ApiService;

		public ClientCreateCommandEffect(IApiService apiService)
		{
			ApiService = apiService;
		}

		protected async override Task HandleAsync(Api.Requests.ClientCreateCommand action, IDispatcher dispatcher)
		{
			try
			{
				var response = 
					await ApiService.Execute<Api.Requests.ClientCreateCommand, Api.Requests.ClientCreateResponse>(action);

				dispatcher.Dispatch(response);
				if (response.Successful)
				{
					NotifyStateChanged(dispatcher, response.Client);
					dispatcher.Dispatch(new Go("/clients/search/"));
				}
			}
			catch
			{
				dispatcher.Dispatch(new Api.Requests.ClientCreateResponse());
				dispatcher.Dispatch(new NotifyUnexpectedServerErrorStatusChanged(true));
			}
		}

		private void NotifyStateChanged(IDispatcher dispatcher, ClientCreateOrUpdate client)
		{
			var clientStateChangeNotification = new ClientStateNotification(
				stateUpdateKind: StateUpdateKind.Created,
				id: client.Id,
				name: client.Name);
			dispatcher.Dispatch(clientStateChangeNotification);
		}
	}
}
