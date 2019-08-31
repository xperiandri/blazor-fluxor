using FullStackSample.Api.Models;
using System;
using System.Collections.Generic;

namespace FullStackSample.Api.Requests
{
	public class ClientsSearchResponse : ApiResponse
	{
		public IEnumerable<ClientSummaryDto> Clients { get; set; }

		public ClientsSearchResponse(IEnumerable<ClientSummaryDto> clients)
		{
			Clients = clients;
		}

		public ClientsSearchResponse()
		{
			Clients = Array.Empty<ClientSummaryDto>();
		}

		public ClientsSearchResponse(
			string errorMessage,
			IEnumerable<KeyValuePair<string, string>> validationErrors)
			: base(errorMessage, validationErrors)
		{
		}
	}
}
