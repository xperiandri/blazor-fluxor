using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.Routing
{
	internal class GoEffect : Effect<Go>
	{
		private readonly IUriHelper UriHelper;

		public GoEffect(IUriHelper uriHelper)
		{
			UriHelper = uriHelper;
		}

		public override Task<IAction[]> HandleAsync(Go action)
		{
			Uri fullUri = UriHelper.ToAbsoluteUri(action.NewUri);
			if (fullUri.ToString() != UriHelper.GetAbsoluteUri())
			{
				// Only navigate if we are not already at the URI specified
				UriHelper.NavigateTo(action.NewUri);
			}
			return Task.FromResult(new IAction[0]);
		}
	}
}
