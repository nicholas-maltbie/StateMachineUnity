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
using System.Collections.Generic;
using nickmaltbie.StateMachineUnity.Event;
using NUnit.Framework;

namespace nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed
{
    public static class StateMachineTestUtils
    {
        /// <summary>
        /// Send an event sequence to a state machine and verify the given transitions.
        /// </summary>
        /// <param name="transitions">Sequence of events and transitions
        /// to send to the state machine and expect changes.</param>
        public static void SendAndVerifyEventSequence(IStateMachine<Type> sm, params (IEvent, Type)[] transitions)
        {
            SendAndVerifyEventSequence(sm, transitions, true);
        }

        /// <summary>
        /// Send an event sequence to a state machine and verify the given transitions.
        /// </summary>
        /// <param name="transitions">Sequence of events and transitions
        /// to send to the state machine and expect changes.</param>
        public static void SendAndVerifyEventSequence(IStateMachine<Type> sm, IEnumerable<(IEvent, Type)> transitions, bool log = true)
        {
            foreach ((IEvent, Type) tuple in transitions)
            {
                if (log)
                {
                    UnityEngine.Debug.Log($"Sending event {tuple.Item1.GetType()} to state machine");
                }

                sm.RaiseEvent(tuple.Item1);

                if (log)
                {
                    UnityEngine.Debug.Log($"State machine is now in state {sm.CurrentState}");
                }

                Assert.AreEqual(sm.CurrentState, tuple.Item2);
            }
        }
    }
}
