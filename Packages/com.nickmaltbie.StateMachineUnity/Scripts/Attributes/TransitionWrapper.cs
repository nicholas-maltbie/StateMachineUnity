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

namespace nickmaltbie.StateMachineUnity.Attributes
{
    /// <summary>
    /// Transition wrapper for creating reversed transitions.
    /// </summary>
    public class TransitionWrapper<E> : ITransition<E>
    {
        public Type TriggerEvent { get; private set; }

        public E TargetState { get; private set; }

        private ITransition<E> transitionBase;

        /// <summary>
        /// Transition to another state on a given event.
        /// </summary>
        /// <param name="triggerEvent">Trigger event to cause transition.</param>
        /// <param name="targetState">New state to transition to upon trigger.</param>
        /// <param name="transitionBase">Transition base to trigger OnTransition event from.</param>
        public TransitionWrapper(Type triggerEvent, E targetState, ITransition<E> transitionBase)
        {
            TriggerEvent = triggerEvent;
            TargetState = targetState;
            this.transitionBase = transitionBase;
        }

        /// <summary>
        /// Behaviour to invoke when this transition is triggered.
        /// </summary>
        /// <param name="sm">State machine being transitioned.</param>
        /// <typeparam name="E">Type of the state machine.</typeparam>
        public void OnTransition(IStateMachine<E> sm)
        {
            transitionBase.OnTransition(sm);
        }
    }
}
