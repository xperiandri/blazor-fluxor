namespace FullStackSample.Api.Requests
{
	public class ClientIsNameTakenResponse
	{
		public bool IsTaken { get; set; }

		public ClientIsNameTakenResponse() { }

		public ClientIsNameTakenResponse(bool isTaken)
		{
			IsTaken = isTaken;
		}
	}
}