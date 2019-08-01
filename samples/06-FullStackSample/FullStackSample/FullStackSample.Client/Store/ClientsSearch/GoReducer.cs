using Blazor.Fluxor;
using Blazor.Fluxor.Routing;
using System;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class GoReducer : Reducer<ClientsSearchState, Go>
	{
		public override ClientsSearchState Reduce(ClientsSearchState state, Go action)
		{
			string uri = new Uri(action.NewUri ?? "").AbsolutePath.ToLowerInvariant();
			if (uri.StartsWith("/clients"))
				return state;
			return ClientsSearchState.Default;
		}
	}
}
