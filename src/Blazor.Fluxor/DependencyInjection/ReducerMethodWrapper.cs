using System;
using System.Reflection;

namespace Blazor.Fluxor.DependencyInjection
{
	internal class ReducerMethodWrapper<TState> : IReducer<TState>
	{
		private readonly Type ActionType;
		private readonly Func<TState, object, TState> ReducerFunction;
		private readonly bool HandleDescendantActions;

		public TState Reduce(TState state, object action) => ReducerFunction(state, action);

		public bool ShouldReduceStateForAction(object action)
		{
			if (action == null)
				return false;

			if (!HandleDescendantActions && action.GetType() != ActionType)
				return false;

			return ActionType.IsAssignableFrom(action.GetType());
		}

		//TODO: Create IReducer non-generic interface
		public static object Create(
			object reducerMethodHost,
			DiscoveredReducerMethodInfo discoveredReducerMethodInfo)
		{
			if (reducerMethodHost == null)
				throw new ArgumentNullException(nameof(reducerMethodHost));
			if (discoveredReducerMethodInfo == null)
				throw new ArgumentNullException(nameof(discoveredReducerMethodInfo));

			var reducerFunction = (Func<object, TState, object, TState>)
				Delegate.CreateDelegate(reducerMethodHost.GetType(), discoveredReducerMethodInfo.ReducerMethodInfo);

			var constructorArgs = new ConstructorArgs(
				actionType: discoveredReducerMethodInfo.ActionType,
				reducer: (state, action) => reducerFunction(reducerMethodHost, state, action),
				handleDescendantActions: (discoveredReducerMethodInfo.Options & ReducerMethodOptions.HandleDescendantActions) != 0);

			Type genericWrapperType = typeof(ReducerMethodWrapper<>).MakeGenericType(discoveredReducerMethodInfo.StateType);
			var result = Activator.CreateInstance(
				type: genericWrapperType,
				bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
				binder: null,
				args: new object[] { constructorArgs },
				culture: null);
			return (ReducerMethodWrapper<TState>)result;
		}

		private ReducerMethodWrapper(ConstructorArgs args)
		{
			ActionType = args.ActionType;
			ReducerFunction = args.Reducer;
			HandleDescendantActions = args.HandleDescendantActions;
		}

		private class ConstructorArgs
		{
			public readonly Type ActionType;
			public readonly Func<TState, object, TState> Reducer;
			public readonly bool HandleDescendantActions;

			public ConstructorArgs(
				Type actionType,
				Func<TState, object, TState> reducer,
				bool handleDescendantActions)
			{
				ActionType = actionType;
				Reducer = reducer;
				HandleDescendantActions = handleDescendantActions;
			}
		}
	}
}
