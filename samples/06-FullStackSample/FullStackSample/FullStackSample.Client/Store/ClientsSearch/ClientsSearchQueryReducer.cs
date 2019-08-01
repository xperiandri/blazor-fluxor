using Blazor.Fluxor;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientsSearchQueryReducer : Reducer<ClientsSearchState, Api.Requests.ClientsSearchQuery>
	{
		public override ClientsSearchState Reduce(ClientsSearchState state, Api.Requests.ClientsSearchQuery query) =>
			new ClientsSearchState(
				isSearching: true,
				name: query.Name,
				errorMessage: null,
				clients: null
			);
	}
}
