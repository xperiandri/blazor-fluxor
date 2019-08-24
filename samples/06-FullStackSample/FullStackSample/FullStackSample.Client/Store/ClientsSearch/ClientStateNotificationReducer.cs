using Blazor.Fluxor;
using FullStackSample.Api.Models;
using FullStackSample.Client.Store.EntityStateEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using FullStackSample.Client.Extensions;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientStateNotificationReducer : Reducer<ClientsSearchState, ClientStateNotification>
	{
		public override ClientsSearchState Reduce(ClientsSearchState state, ClientStateNotification action)
		{
			var clients = state.Clients.UpdateState(action);
			if (state.Name != null)
			{
				string searchName = state.Name.ToLowerInvariant();
				clients = clients
					.Where(x => (x.Name ?? "").ToLowerInvariant().Contains(searchName));
			}
			return new ClientsSearchState(
				isSearching: state.IsSearching,
				name: state.Name,
				errorMessage: state.ErrorMessage,
				clients: clients);
		}
	}
}
