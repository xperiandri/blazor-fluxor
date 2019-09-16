﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Fluxor
{
	///	<summary>
	///		A generic base implementation of a reducer that can handle
	///		multiple actions.
	///	</summary>
	///	<para>
	///		This is an alternative approach to <see cref="Reducer{TState, TAction}"/>. Instead of
	///		creating a reducer class for each action that needs to be reduced into a state, this
	///		class can be used so that a variety of actions can be reduced from a single class.
	///	</para>
	///	<example>
	///		public class SearchReducers : MultiActionReducer&lt;SearchState&gt;
	///		{
	///			public SearchReducers()
	///			{
	///				AddActionReducer&lt;Search&gt;((state, action) =&gt;
	///					new StateState(isSearching: true, searchItems: null));
	///
	///				AddActionReduer&lt;SearchResponse&gt;((state, response) =&gt;
	///					new StateSearch(isSearching: false, searchItems: response.Items));
	///			}
	///		}
	///	</example>
	///	<typeparam name="TState">The state that this reducer works with</typeparam>
	public abstract class MultiActionReducer<TState> : IReducer<TState>
	{
		Dictionary<Type, Func<TState, object, TState>> ReducersByActionType;

		///	<summary>
		///		Creates a new instance. This is where <see cref="AddActionReducer{TAction}(Func{TState, TAction, TState})"/>
		///		should be executed to register reducers.
		///	</summary>
		public MultiActionReducer()
		{
			ReducersByActionType = new Dictionary<Type, Func<TState, object, TState>>();
		}

		/// <summary>
		///		Specifies what code should be executed when the specific action needs to be
		///		reduced into the TState.
		/// </summary>
		/// <typeparam name="TAction">The action type the <paramref name="reducer"/> code responds to</typeparam>
		/// <param name="reducer">
		///		A function or method that accepts <typeparamref name="TState"/> and <typeparamref name="TAction"/>
		///		and returns a <typeparamref name="TState"/>.
		///	</param>
		protected void AddActionReducer<TAction>(Func<TState, TAction, TState> reducer)
			=> AddActionReducer(typeof(TAction), (TState state, object action) => reducer(state, (TAction)action));

		/// <summary>
		///		Specifies what code should be executed when the specific action needs to be
		///		reduced into the TState.
		/// </summary>
		/// <param name="actionType">
		///		The action type this <paramref name="reducer"/> code responds to.
		/// </param>
		/// <param name="reducer">
		///		A function or method that accepts <typeparamref name="TState"/> and an <see cref="object"/> action,
		///		and returns a <typeparamref name="TState"/>.
		///	</param>
		protected void AddActionReducer(Type actionType, Func<TState, object, TState> reducer)
			=> ReducersByActionType.Add(actionType, reducer);

		/// <summary>
		///		Implements the <see cref="IReducer{TState}.Reduce(TState, object)"/> method.
		///		This implementation will identify which reducer function to invoke
		///		based on the action, and then return the result of invoking that function.
		///	<para>
		///		Note that this method will not be called by Fluxor if <see cref="ShouldReduceStateForAction(object)"/>
		///		returns false.
		///	</para>
		/// </summary>
		/// <param name="state">The current state</param>
		/// <param name="action">The action dispatched via the store</param>
		/// <returns>The new state based on the current state + the changes the action should cause</returns>
		/// <exception cref="InvalidOperationException">
		///		Thrown if no reducer function has been added for the <paramref name="action"/> type.
		///	</exception>
		public TState Reduce(TState state, object action)
		{
			if (action == null)
				return state;

			var actionType = action.GetType();
			if (!ReducersByActionType.TryGetValue(actionType, out Func<TState, object, TState> reducer))
			{
				var baseType = ReducersByActionType.Keys.FirstOrDefault(k => k.IsAssignableFrom(actionType));
				if (baseType == null)
					throw new InvalidOperationException($"Reducer {GetType().Name} cannot reduce action {action.GetType().Name} into state {typeof(TState).Name}.");

				reducer = ReducersByActionType[baseType];
			}

			return reducer(state, action);
		}

		/// <summary>
		///		Indicates whether or not a reducer function has been registered with the reducer
		///		that can handle this type of action.
		/// </summary>
		/// <param name="action"></param>
		/// <returns>True if a reducer action/method has been added for this type of action, otherwise false.</returns>
		public bool ShouldReduceStateForAction(object action)
		{
			if (action == null)
				return false;

			return ReducersByActionType.ContainsKey(action.GetType());
		}
	}
}
