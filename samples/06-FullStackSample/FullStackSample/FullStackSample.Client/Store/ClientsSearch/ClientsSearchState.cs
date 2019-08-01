using System;
using System.Collections.Generic;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientsSearchState
	{
		public bool IsSearching { get; private set; }
		public string ErrorMessage { get; private set; }
		public string Name { get; set; }
		public IEnumerable<Api.Models.ClientSummary> Clients { get; private set; }
		public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

		public ClientsSearchState(
			bool isSearching,
			string errorMessage,
			string name,
			IEnumerable<Api.Models.ClientSummary> clients)
		{
			IsSearching = isSearching;
			ErrorMessage = errorMessage;
			Name = name;
			Clients = clients ?? Array.Empty<Api.Models.ClientSummary>();
		}

		public static readonly ClientsSearchState Default = new ClientsSearchState(
			isSearching: false,
			name: null,
			errorMessage: null,
			clients: null);
	}
}
