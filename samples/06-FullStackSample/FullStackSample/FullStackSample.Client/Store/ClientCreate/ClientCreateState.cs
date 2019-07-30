using FullStackSample.Api.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FullStackSample.Client.Store.ClientCreate
{
	public class ClientCreateState
	{
		public bool IsExecutingApi { get; }
		public string ErrorMessage { get; }
		public ClientCreateOrUpdate Client { get; }
		public IEnumerable<KeyValuePair<string, string>> ValidationErrors { get; }

		public ClientCreateState(
			ClientCreateOrUpdate client,
			bool isExecutingApi,
			string errorMessage,
			IEnumerable<KeyValuePair<string, string>> validationErrors)
		{
			Client = client ?? new ClientCreateOrUpdate();
			IsExecutingApi = isExecutingApi;
			ErrorMessage = errorMessage;
			ValidationErrors = validationErrors ?? Array.Empty<KeyValuePair<string, string>>();
		}
	}
}
