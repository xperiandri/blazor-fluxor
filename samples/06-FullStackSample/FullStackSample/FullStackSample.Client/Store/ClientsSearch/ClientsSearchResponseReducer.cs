using Blazor.Fluxor;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientsSearchResponseReducer : Reducer<ClientsSearchState, Api.Requests.ClientsSearchResponse>
	{
		public override ClientsSearchState Reduce(ClientsSearchState state, Api.Requests.ClientsSearchResponse response) =>
				new ClientsSearchState(
					isSearching: false,
					name: state.Name,
					errorMessage: response.ErrorMessage,
					clients: response.Clients
				);
	}
}
