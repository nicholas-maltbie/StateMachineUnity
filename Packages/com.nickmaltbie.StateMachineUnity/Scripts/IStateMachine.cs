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
    /// Abstract state machine to manage a set of given states
    /// and transitions.
    /// </summary>
    public interface IStateMachine<E>
    {
        /// <summary>
        /// Current state of the state machine.
        /// </summary>
        public E CurrentState { get; }

        /// <summary>
        /// Raise an event to the current state machine.
        /// </summary>
        /// <param name="evt">Event to send to the state machine.</param>
        public void RaiseEvent(IEvent evt);

        /// <summary>
        /// Internal method to set the current state of the state machine without
        /// invoking the <see cref="Attributes.OnEnterStateAttribute"/>
        /// or <see cref="Attributes.OnExitStateAttribute"/>
        /// </summary>
        /// <param name="newState">New state to set for the state machine.</param>
        public void SetStateQuiet(Type newState);
    }
}
