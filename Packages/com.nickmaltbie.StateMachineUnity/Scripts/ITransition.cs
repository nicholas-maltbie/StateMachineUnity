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

namespace nickmaltbie.StateMachineUnity
{
    /// <summary>
    /// Transition that can be triggered by an event.
    /// </summary>
    /// <typeparam name="TState">Type of the state machine.</typeparam>
    public interface ITransition<TState>
    {
        /// <summary>
        /// Type of event to listen for.
        /// </summary>
        public Type TriggerEvent { get; }

        /// <summary>
        /// Target state upon event trigger.
        /// </summary>
        public TState TargetState { get; }

        /// <summary>
        /// Behaviour to invoke when this transition is triggered.
        /// </summary>
        /// <param name="sm">State machine being transitioned.</param>
        public void OnTransition(IStateMachine<TState> sm);
    }
}
