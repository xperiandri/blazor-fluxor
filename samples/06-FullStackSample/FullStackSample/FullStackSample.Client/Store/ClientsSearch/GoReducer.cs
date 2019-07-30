using Blazor.Fluxor;
using Blazor.Fluxor.Routing;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class GoReducer : Reducer<ClientsSearchState, Go>
	{
		public override ClientsSearchState Reduce(ClientsSearchState state, Go action)
		{
			string uri = (action.NewUri ?? "").ToLowerInvariant();
			if (uri.StartsWith("/clients/search"))
				return state;
			return ClientsSearchState.Default;
		}
	}
}
