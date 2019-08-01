namespace FullStackSample.Api.Models
{
	public class ClientSummary
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public ClientSummary() { }

		public ClientSummary(int id, string name) : this()
		{
			Id = id;
			Name = name;
		}
	}
}
