namespace FullStackSample.Api.Models
{
	public class ClientSummaryDto
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public ClientSummaryDto() { }

		public ClientSummaryDto(int id, string name) : this()
		{
			Id = id;
			Name = name;
		}
	}
}
