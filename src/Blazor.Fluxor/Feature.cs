using Microsoft.AspNetCore.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor
{
	public abstract class Feature<TState> : IFeature<TState>
	{
		public abstract string GetName();
		public virtual TState State { get; protected set; }
		public virtual object GetState() => State;
		public virtual void RestoreState(object value) => State = (TState)value;
		public virtual Type GetStateType() => typeof(TState);

		protected abstract TState GetInitialState();
		protected readonly List<IReducer<TState>> Reducers = new List<IReducer<TState>>();

		public Feature()
		{
			State = GetInitialState();
		}

		public virtual void AddReducer(IReducer<TState> reducer)
		{
			if (reducer == null)
				throw new ArgumentNullException(nameof(reducer));
			Reducers.Add(reducer);
		}

		public virtual void ReceiveDispatchNotificationFromStore(IAction action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			IEnumerable<IReducer<TState>> applicableReducers = Reducers.Where(x => x.ShouldReduceStateForAction(action));
			TState newState = State;
			foreach (IReducer<TState> currentReducer in applicableReducers)
			{
				newState = currentReducer.Reduce(newState, action);
			}
			State = newState;
		}

	}

}
