using MediatR;
using System;

namespace FullStackSample.Api.Requests
{
	public class ClientIsRegistrationNumberAvailableQuery : IRequest<ClientIsRegistrationNumberAvailableResponse>
	{
		public int? ClientIdToIgnore { get; set; }
		public int RegistrationNumber { get; set; }

		public ClientIsRegistrationNumberAvailableQuery() { }

		public ClientIsRegistrationNumberAvailableQuery(int? clientIdToIgnore, int registrationNumber)
			: this()
		{
			if (registrationNumber == 0)
				throw new ArgumentOutOfRangeException(nameof(registrationNumber), "Cannot be 0.");

			ClientIdToIgnore = clientIdToIgnore;
			RegistrationNumber = registrationNumber;
		}
	}
}
