using FullStackSample.Api.Models;
using MediatR;
using System;

namespace FullStackSample.Api.Requests
{
	public class ClientCreateCommand : IRequest<ClientCreateResponse>
	{
		public ClientCreateDto Client { get; set; }

		public ClientCreateCommand() { }

		public ClientCreateCommand(ClientCreateDto client)
		{
			if (client == null)
				throw new ArgumentNullException(nameof(client));

			Client = client;
		}
	}
}
