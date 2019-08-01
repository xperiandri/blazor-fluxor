using MediatR;
using System;

namespace FullStackSample.Api.Requests
{
	public class ClientIsNameTakenQuery : IRequest<ClientIsNameTakenResponse>
	{
		public int? ClientIdToIgnore { get; set; }
		public string Name { get; set; }

		public ClientIsNameTakenQuery() { }

		public ClientIsNameTakenQuery(int? clientIdToIgnore, string name) : this()
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			ClientIdToIgnore = clientIdToIgnore;
			Name = name;
		}
	}
}
