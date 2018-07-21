using Blazor.Fluxor;
using FlightFinder.Shared;

namespace FlightFinder.Client.Store
{
	public class AddToShortlistAction: IAction
	{
		public readonly Itinerary Itinerary;

		public AddToShortlistAction(Itinerary itinerary)
		{
			Itinerary = itinerary;
		}
	}
}
