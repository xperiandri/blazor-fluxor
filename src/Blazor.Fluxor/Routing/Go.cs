namespace Blazor.Fluxor.Routing
{
	public class Go: IAction
	{
		public string NewUri { get; private set; }

		public Go(string newUri)
		{
			NewUri = newUri;
		}
	}
}
