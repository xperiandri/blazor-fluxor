using FullStackSample.Api.Models;

namespace FullStackSample.Client.Store.ClientCreate
{
	public class ClientCreateState
	{
		public bool IsExecutingApi { get; }
		public string ErrorMessage { get; }
		public ClientCreateOrUpdate Client { get; }

		public ClientCreateState(
			ClientCreateOrUpdate client,
			bool isExecutingApi,
			string errorMessage)
		{
			Client = client ?? new ClientCreateOrUpdate();
			IsExecutingApi = isExecutingApi;
			ErrorMessage = errorMessage;
		}
	}
}
