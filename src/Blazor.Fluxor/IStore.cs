using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	public interface IStore
	{
		void AddEffect(Type actionType, IEffect effect);
		void AddFeature(IFeature feature);
		void AddMiddleware(IMiddleware middleware);
		IDisposable BeginInternalMiddlewareChange();
		Task DispatchAsync(IAction action);
		IEnumerable<IFeature> Features { get; }
		RenderFragment Initialize();
	}
}
