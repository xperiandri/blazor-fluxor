using Blazor.Fluxor;
using Blazor.Fluxor.Routing;
using System;

namespace FullStackSample.Client.Store.ClientCreate
{
	public class GoReducer : Reducer<ClientCreateState, Go>
	{
		public override ClientCreateState Reduce(ClientCreateState state, Go action)
		{
			string uri = new Uri(action.NewUri ?? "").AbsolutePath.ToLowerInvariant();
			if (uri.StartsWith("/clients/create"))
				return state;
			return new ClientCreateState(
				client: null,
				isExecutingApi: false,
				errorMessage: null,
				validationErrors: null);
		}
	}
}
