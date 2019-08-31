namespace FullStackSample.Api.Requests
{
	public class ClientIsRegistrationNumberAvailableResponse : ApiResponse
	{
		public bool Available { get; set; }

		public ClientIsRegistrationNumberAvailableResponse() { }

		public ClientIsRegistrationNumberAvailableResponse(bool available)
		{
			Available = available;
		}
	}
}