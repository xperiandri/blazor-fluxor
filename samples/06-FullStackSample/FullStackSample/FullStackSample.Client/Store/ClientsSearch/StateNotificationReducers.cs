using Blazor.Fluxor;
using FullStackSample.Client.Store.EntityStateEvents;
using System.Linq;
using FullStackSample.Client.Extensions;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class StateNotificationReducers : MultiActionReducer<ClientsSearchState>
	{
		public StateNotificationReducers()
		{
			AddActionReducer<ClientStateNotification>((state, action) =>
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
			});
		}
	}
}
