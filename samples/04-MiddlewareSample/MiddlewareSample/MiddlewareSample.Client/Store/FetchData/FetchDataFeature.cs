using Blazor.Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareSample.Client.Store.FetchData
{
	public class FetchDataFeature: Feature<FetchDataState>
	{
		public override string GetName() => "FetchData";
		protected override FetchDataState GetInitialState() => new FetchDataState(
			isLoading: false,
			errorMessage: null,
			forecasts: null);
	}
}
