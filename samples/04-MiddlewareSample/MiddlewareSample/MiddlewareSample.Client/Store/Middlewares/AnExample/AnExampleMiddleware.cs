using Blazor.Fluxor;
using System;

namespace MiddlewareSample.Client.Store.Middlewares.AnExample
{
	public class AnExampleMiddleware: Middleware
	{
		public override string GetClientScripts()
		{
			return $"console.log('This message is from Javascript injected by {GetType().FullName}');";
		}

		public override void Initialize(IStore store)
		{
			base.Initialize(store);
			Console.WriteLine("The Middleware has just been initialized, we now have a reference to the store");
		}

		public override void AfterInitializeAllMiddlewares()
		{
			Console.WriteLine("Initialize() has been executed on all enabled middlewares");
		}

		public override bool MayDispatchAction(IAction action)
		{
			bool allow = new Random().Next(4) > 1;
			string allowResult = allow ? "This one has been allowed" : "This one has been prohibited";
			Console.WriteLine("Middleware can prevent actions from being dispatched: " + allowResult);
			return allow;
		}

		public override void BeforeDispatch(IAction action)
		{
			Console.WriteLine("Middleware has detected action about to be dispatched: " + action.GetType().FullName);
		}

		public override void AfterDispatch(IAction action)
		{
			Console.WriteLine("Middleware has been notified that an action was dispatched: " + action.GetType().FullName);
		}
	}
}
