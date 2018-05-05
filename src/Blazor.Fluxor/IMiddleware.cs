using System;

namespace Blazor.Fluxor
{
    public interface IMiddleware
    {
		string GetClientScripts();
		void Initialize(IStore store);
		void AfterInitializeAllMiddlewares();
		bool MayDispatchAction(IAction action);
		void BeforeDispatch(IAction action);
		void AfterDispatch(IAction action);
		IDisposable BeginInternalMiddlewareChange();
	}
}
