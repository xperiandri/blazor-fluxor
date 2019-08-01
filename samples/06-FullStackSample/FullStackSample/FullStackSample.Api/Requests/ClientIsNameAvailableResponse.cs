namespace FullStackSample.Api.Requests
{
	public class ClientIsNameAvailableResponse : ApiResponse
	{
		public bool Available { get; set; }

		public ClientIsNameAvailableResponse() { }

		public ClientIsNameAvailableResponse(bool available)
		{
			Available = available;
		}
	}
}