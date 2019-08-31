using Blazor.Fluxor;
using Blazor.Fluxor.Routing;
using System;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientsSearchReducers : MultiActionReducer<ClientsSearchState>
	{
		public ClientsSearchReducers()
		{
			AddActionReducer<Go>((state, action) =>
			{
				string uri = new Uri(action.NewUri ?? "").AbsolutePath.ToLowerInvariant();
				if (uri.StartsWith("/clients"))
					return state;
				return ClientsSearchState.Default;
			});

			AddActionReducer<Api.Requests.ClientsSearchQuery>((state, query) =>
				new ClientsSearchState(
					isSearching: true,
					name: query.Name,
					errorMessage: null,
					clients: null
				));

			AddActionReducer<Api.Requests.ClientsSearchResponse>((state, response) =>
				new ClientsSearchState(
						isSearching: false,
						name: state.Name,
						errorMessage: response.ErrorMessage,
						clients: response.Clients));
		}
	}
}
