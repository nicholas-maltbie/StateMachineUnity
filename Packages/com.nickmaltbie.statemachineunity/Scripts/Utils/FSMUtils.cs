// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;

namespace nickmaltbie.StateMachineUnity.Utils
{
    /// <summary>
    /// Class with utility functions for state machine.
    /// </summary>
    public static class FSMUtils
    {
        /// <summary>
        /// Map of actions of state machine -> state, attribute) -> action.
        /// </summary>
        internal static ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, MethodInfo>> ActionCache =
            new ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, MethodInfo>>();

        /// <summary>
        /// Map of transitions of state machine -> (state, event) -> state.
        /// </summary>
        internal static ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, ITransition<Type>>> TransitionCache =
            new ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, ITransition<Type>>>();

        /// <summary>
        /// Map of actions of state machine -> (state, event) -> [ actions ]
        /// </summary>
        internal static ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, IEnumerable<MethodInfo>>> EventCache =
            new ConcurrentDictionary<Type, Dictionary<Tuple<Type, Type>, IEnumerable<MethodInfo>>>();

        /// <summary>
        /// Returns the action with the specified name.
        /// </summary>
        /// <param name="actionName">Name of action to search for.</param>
        /// <returns>Method info for the given action.</returns>
        public static MethodInfo GetActionWithName(Type type, string actionName)
        {
            MethodInfo action;

            do
            {
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.FlattenHierarchy;
                action = type.GetMethod(actionName, bindingFlags);
                type = type.BaseType;
            }
            while (action is null && type != null);

            return action;
        }

        /// <summary>
        /// Sets up an action lookup by enumerating the <see cref="State"/> classes
        /// defined within a state machine type and getting all <see cref="nickmaltbie.StateMachineUnity.Attributes.ActionAttribute"/>
        /// decorators defined for each <see cref="State"/> within the class.
        /// </summary>
        /// <param name="stateMachine">State machine to lookup <see cref="State"/> and <see cref="nickmaltbie.StateMachineUnity.Attributes.ActionAttribute"/> for.</param>
        /// <returns>A lookup table mapped as (state, actionType) -> Method</returns>
        public static Dictionary<Tuple<Type, Type>, MethodInfo> CreateActionAttributeCache(Type stateMachine)
        {
            var actionLookup = new Dictionary<Tuple<Type, Type>, MethodInfo>();

            // Find all the supported states for the state machine.
            foreach (Type state in stateMachine.GetNestedTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(State))))
            {
                // Find all action attributes of given state
                foreach (ActionAttribute attr in Attribute.GetCustomAttributes(state, typeof(ActionAttribute)))
                {
                    Type actionType = attr.GetType();
                    Type targetState = state;

                    if (targetState.IsSubclassOf(typeof(AnyState)))
                    {
                        targetState = typeof(AnyState);
                    }

                    actionLookup[new Tuple<Type, Type>(targetState, actionType)] = GetActionWithName(stateMachine, attr.Action);
                }
            }

            return actionLookup;
        }

        /// <summary>
        /// Sets up a transition within a transition lookup.
        /// </summary>
        /// <param name="state">Source state for saving transition.</param>
        /// <param name="transition">Transition to add to lookup table.</param>
        /// <param name="transitionLookup">Lookup to add transition to.</param>
        public static void SetupTransition(Type state, ITransition<Type> transition, Dictionary<Tuple<Type, Type>, ITransition<Type>> transitionLookup)
        {
            Type sourceState = state;
            Type targetState = transition.TargetState;

            if (transition.GetType() == typeof(TransitionFromAttribute) || transition.GetType().IsSubclassOf(typeof(TransitionFromAttribute)))
            {
                sourceState = transition.TargetState;
                targetState = state;
                transition = new TransitionWrapper<Type>(transition.TriggerEvent, state, transition);
            }

            if (sourceState.IsSubclassOf(typeof(AnyState)) || sourceState == typeof(AnyState))
            {
                sourceState = typeof(AnyState);
            }

            if (targetState.IsSubclassOf(typeof(AnyState)) || targetState == typeof(AnyState))
            {
                throw new InvalidOperationException($"Cannot transition to {nameof(AnyState)} as part of a TransitionAttribute for TransitionAttribute:{transition} from state:{state} to state:{transition.TargetState} on event:{transition.TriggerEvent}");
            }

            transitionLookup[new Tuple<Type, Type>(sourceState, transition.TriggerEvent)] = transition;
        }

        /// <summary>
        /// Sets up an transition lookup by enumerating the <see cref="State"/> classes
        /// defined within a state machine type and getting all <see cref="nickmaltbie.StateMachineUnity.Attributes.TransitionAttribute"/>
        /// decorators defined for each <see cref="State"/> within the class.
        /// </summary>
        /// <param name="stateMachine">State machine to lookup <see cref="State"/> and <see cref="nickmaltbie.StateMachineUnity.Attributes.TransitionAttribute"/> for.</param>
        /// <returns>A lookup table mapped as (state, event) -> state</returns>
        public static Dictionary<Tuple<Type, Type>, ITransition<Type>> CreateTransitionAttributeCache(Type stateMachine)
        {
            var transitionLookup = new Dictionary<Tuple<Type, Type>, ITransition<Type>>();

            // Find all the supported states for the state machine.
            foreach (Type state in stateMachine.GetNestedTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(State))))
            {
                // Find all transition attributes of given state
                foreach (ITransition<Type> transition in Attribute.GetCustomAttributes(state, typeof(TransitionAttribute)))
                {
                    SetupTransition(state, transition, transitionLookup);
                }
            }

            return transitionLookup;
        }

        /// <summary>
        /// Sets up an event lookup by enumerating the <see cref="State"/> classes
        /// defined within a state machine type and getting all <see cref="nickmaltbie.StateMachineUnity.Attributes.OnEventDoActionAttribute"/>
        /// decorators defined for each <see cref="State"/> within the class.
        /// </summary>
        /// <param name="stateMachine">State machine to lookup <see cref="State"/> and <see cref="nickmaltbie.StateMachineUnity.Attributes.OnEventDoActionAttribute"/> for.</param>
        /// <returns>A lookup table mapped as (state, event) -> [ methods ]</returns>
        public static Dictionary<Tuple<Type, Type>, IEnumerable<MethodInfo>> CreateEventActionCache(Type stateMachine)
        {
            var eventLookup = new Dictionary<Tuple<Type, Type>, IEnumerable<MethodInfo>>();

            // Find all the supported states for the state machine.
            foreach (Type state in stateMachine.GetNestedTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(State))))
            {
                // Find all OnEventDoAction attributes of given state
                foreach (OnEventDoActionAttribute attr in Attribute.GetCustomAttributes(state, typeof(OnEventDoActionAttribute)))
                {
                    Type evt = attr.Event;
                    MethodInfo action = FSMUtils.GetActionWithName(stateMachine, attr.Action);
                    Type targetState = state;

                    if (state.IsSubclassOf(typeof(AnyState)))
                    {
                        targetState = typeof(AnyState);
                    }

                    var tupleKey = new Tuple<Type, Type>(targetState, evt);

                    if (eventLookup.ContainsKey(tupleKey))
                    {
                        (eventLookup[tupleKey] as List<MethodInfo>).Add(action);
                    }
                    else
                    {
                        eventLookup[tupleKey] = new List<MethodInfo>(new MethodInfo[] { action });
                    }
                }
            }

            return eventLookup;
        }

        /// <summary>
        /// Raise a synchronous event for a given state machine.
        /// <br/>
        /// First checks if this state machine expects any events of this type
        /// for the state machine's CurrentState. These
        /// would follow an attribute of type <see cref="nickmaltbie.StateMachineUnity.Attributes.OnEventDoActionAttribute"/>.
        /// <br/>
        /// If the state machine's CurrentState expects a transition
        /// based on the event, then this will trigger the <see cref="nickmaltbie.StateMachineUnity.Attributes.OnExitStateAttribute"/>
        /// of the nickmaltbie.StateMachineUnity.IStateMachine, change to the next state defined in
        /// the <see cref="nickmaltbie.StateMachineUnity.Attributes.TransitionAttribute"/>, then trigger the
        /// <see cref="nickmaltbie.StateMachineUnity.Attributes.OnEnterStateAttribute"/>
        /// of the next state.
        /// </summary>
        /// <param name="StateMachine">state machine to invoke method of.</param>
        /// <param name="evt">Event to send to this state machine.</param>
        public static void RaiseCachedEvent(IStateMachine<Type> stateMachine, IEvent evt)
        {
            var tupleKey = new Tuple<Type, Type>(stateMachine.CurrentState, evt.GetType());
            var anyStateTupleKey = new Tuple<Type, Type>(typeof(AnyState), evt.GetType());

            EventCache[stateMachine.GetType()].TryGetValue(tupleKey, out IEnumerable<MethodInfo> baseActions);
            EventCache[stateMachine.GetType()].TryGetValue(anyStateTupleKey, out IEnumerable<MethodInfo> anyActions);

            foreach (MethodInfo action in Enumerable.Concat(
                baseActions ?? Enumerable.Empty<MethodInfo>(),
                anyActions ?? Enumerable.Empty<MethodInfo>()))
            {
                ParameterInfo[] actionParams = action.GetParameters();
                if (actionParams.Length == 0)
                {
                    action?.Invoke(stateMachine, new object[0]);
                }
                else
                {
                    action?.Invoke(stateMachine, new object[]{evt});
                }
            }

            // Use short circuit operations to select from target class
            // before an AnyState transition.
            bool hasTransition = TransitionCache[stateMachine.GetType()].TryGetValue(tupleKey, out ITransition<Type> transition) ||
                TransitionCache[stateMachine.GetType()].TryGetValue(anyStateTupleKey, out transition);

            if (hasTransition)
            {
                InvokeAction<OnExitStateAttribute>(stateMachine, stateMachine.CurrentState, evt);
                transition.OnTransition(stateMachine);
                stateMachine.SetStateQuiet(transition.TargetState);
                InvokeAction<OnEnterStateAttribute>(stateMachine, stateMachine.CurrentState, evt);
            }
        }

        /// <summary>
        /// Synchronously invokes an action of a given name.
        /// </summary>
        /// <typeparam name="E">Type of action to invoke.</typeparam>
        /// <param name="stateMachine">state machine to invoke method of.</param>
        /// <param name="state">State to invoke action for, if unspecified or null, will use the CurrentState.</param>
        /// <returns>True if an action was found and invoked, false otherwise.</returns>
        public static bool InvokeAction<E>(IStateMachine<Type> stateMachine, Type state = null, IEvent evt = null) where E : ActionAttribute
        {
            return InvokeAction(stateMachine, typeof(E), state, evt);
        }

        /// <summary>
        /// Synchronously invokes an action of a given name.
        /// </summary>
        /// <param name="stateMachine">state machine to invoke method of.</param>
        /// <param name="actionType">Type of action to invoke.</param>
        /// <param name="state">State to invoke action for.</param>
        /// <param name="evt">Event that triggered state (if any).</param>
        /// <returns>True if an action was found and invoked, false otherwise.</returns>
        public static bool InvokeAction(IStateMachine<Type> stateMachine, Type actionType, Type state, IEvent evt)
        {
            var tupleKey = new Tuple<Type, Type>(state ?? stateMachine.CurrentState, actionType);
            var anyStateTupleKey = new Tuple<Type, Type>(typeof(AnyState), actionType);
            if (ActionCache[stateMachine.GetType()].TryGetValue(tupleKey, out MethodInfo method) ||
                ActionCache[stateMachine.GetType()].TryGetValue(anyStateTupleKey, out method))
            {
                ParameterInfo[] actionParams = method.GetParameters();
                if (actionParams.Length == 0)
                {
                    method?.Invoke(stateMachine, new object[0]);
                }
                else
                {
                    method?.Invoke(stateMachine, new object[]{evt});
                }
                return method != null;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Initialize a state machine with the initialization state and ensure
        /// all the events, transitions, and actions are cached.
        /// </summary>
        /// <param name="stateMachine">state machine to setup.</param>
        public static void InitializeStateMachine(IStateMachine<Type> stateMachine)
        {
            // Ensure the cache is setup if not done so already
            SetupCache(stateMachine.GetType());

            stateMachine.SetStateQuiet(stateMachine.GetType().GetNestedTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(State)))
                .First(type => State.IsInitialState(type)));

            InvokeAction<OnEnterStateAttribute>(stateMachine, stateMachine.CurrentState);
        }

        /// <summary>
        /// Setup the cache for the state machine if it hasn't been done already.
        /// </summary>
        public static void SetupCache(Type stateMachine)
        {
            if (!ActionCache.ContainsKey(stateMachine))
            {
                ActionCache.TryAdd(stateMachine, FSMUtils.CreateActionAttributeCache(stateMachine));
            }

            if (!TransitionCache.ContainsKey(stateMachine))
            {
                TransitionCache.TryAdd(stateMachine, FSMUtils.CreateTransitionAttributeCache(stateMachine));
            }

            if (!EventCache.ContainsKey(stateMachine))
            {
                EventCache.TryAdd(stateMachine, FSMUtils.CreateEventActionCache(stateMachine));
            }
        }
    }
}
