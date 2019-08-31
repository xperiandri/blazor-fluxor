using MediatR;
using System;

namespace FullStackSample.Api.Requests
{
	public class ClientIsNameAvailableQuery : IRequest<ClientIsNameAvailableResponse>
	{
		public int? ClientIdToIgnore { get; set; }
		public string Name { get; set; }

		public ClientIsNameAvailableQuery() { }

		public ClientIsNameAvailableQuery(int? clientIdToIgnore, string name) : this()
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			ClientIdToIgnore = clientIdToIgnore;
			Name = name;
		}
	}
}
