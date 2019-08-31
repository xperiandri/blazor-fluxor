using System;
using System.Collections.Generic;

namespace Blazor.Fluxor
{
	public abstract class MultiActionReducer<TState> : IReducer<TState>
	{
		Dictionary<Type, Func<TState, object, TState>> ReducersByActionType;

		public MultiActionReducer()
		{
			ReducersByActionType = new Dictionary<Type, Func<TState, object, TState>>();
		}

		public void AddActionReducer<TAction>(Func<TState, TAction, TState> reducer)
			=> AddActionReducer(typeof(TAction), reducer);

		public void AddActionReducer(Type actionType, Func<TState, object, TState> reducer)
			=> ReducersByActionType.Add(actionType, reducer);

		public TState Reduce(TState state, object action)
		{
			if (action == null)
				return state;

			if (!ReducersByActionType.TryGetValue(action.GetType(), out Func<TState, object, TState> reducer))
				throw new InvalidOperationException($"Reducer {GetType().Name} cannot reduce action {action.GetType().Name} into state {typeof(TState).Name}.");

			return reducer(state, action);
		}

		public bool ShouldReduceStateForAction(object action)
		{
			if (action == null)
				return false;

			return ReducersByActionType.ContainsKey(action.GetType());
		}
	}
}
