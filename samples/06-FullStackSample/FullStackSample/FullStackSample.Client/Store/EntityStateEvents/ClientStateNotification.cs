namespace FullStackSample.Client.Store.EntityStateEvents
{
	public class ClientStateNotification
	{
		public StateUpdateKind StateUpdateKind { get; set; }
		public int Id { get; set; }
		public PropertyUpdate<string> Name { get; set; }

		public ClientStateNotification(
			StateUpdateKind stateUpdateKind,
			int id,
			PropertyUpdate<string> name)
		{
			StateUpdateKind = stateUpdateKind;
			Id = id;
			Name = name;
		}
	}
}
