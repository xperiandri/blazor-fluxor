using System;

namespace Blazor.Fluxor
{
	public abstract class Middleware : IMiddleware
	{
		private int BeginMiddlewareChangeCount;

		protected IStore Store { get; private set; }
		protected bool IsInsideMiddlewareChange => BeginMiddlewareChangeCount > 0;
		protected virtual void OnInternalMiddlewareChangeEnding() { }

		public virtual string GetClientScripts() => null;
		public virtual void AfterInitializeAllMiddlewares() { }
		public virtual bool MayDispatchAction(IAction action) => true;
		public virtual void BeforeDispatch(IAction action) { }
		public virtual void AfterDispatch(IAction action) { }

		public virtual void Initialize(IStore store)
		{
			Store = store;
		}

		public virtual IDisposable BeginInternalMiddlewareChange()
		{
			BeginMiddlewareChangeCount++;
			return new DisposableCallback(() =>
			{
				if (BeginMiddlewareChangeCount == 1)
					OnInternalMiddlewareChangeEnding();
				BeginMiddlewareChangeCount--;
			});
		}
	}
}
