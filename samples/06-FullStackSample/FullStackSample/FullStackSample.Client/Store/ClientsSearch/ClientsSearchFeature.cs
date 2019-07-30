using Blazor.Fluxor;

namespace FullStackSample.Client.Store.ClientsSearch
{
	public class ClientsSearchFeature : Feature<ClientsSearchState>
	{
		public override string GetName() => "ClientsSearch";
		protected override ClientsSearchState GetInitialState() => ClientsSearchState.Default;
	}
}
