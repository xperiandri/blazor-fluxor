using System.Collections;
using System.Collections.Generic;
using FullStackSample.Api.Models;

namespace FullStackSample.Api.Requests
{
	public class ClientCreateResponse : ApiResponse
	{
		public int ClientId { get; set; }
		public ClientCreateDto Client { get; set; }

		public ClientCreateResponse(int clientId, ClientCreateDto client)
		{
			ClientId = clientId;
			Client = client;
		}

		public ClientCreateResponse()
		{
		}

		public ClientCreateResponse(
			string errorMessage,
			params KeyValuePair<string, string>[] validationErrors)
			: base(errorMessage, validationErrors)
		{
		}

	}
}