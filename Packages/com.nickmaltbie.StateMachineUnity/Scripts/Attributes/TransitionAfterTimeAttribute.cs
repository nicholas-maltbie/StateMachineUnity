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
using nickmaltbie.StateMachineUnity.Event;

namespace nickmaltbie.StateMachineUnity.Attributes
{
    /// <summary>
    /// Attribute to transition after a given period of time
    /// from one state to another.
    /// 
    /// Supported by any implementation of the
    /// <see cref="nickmaltbie.StateMachineUnity.Fixed.FixedSMBehaviour"/> that has a sense of how much
    /// time has passed in the update or fixed update time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TransitionAfterTimeAttribute : TransitionAttribute
    {
        /// <summary>
        /// Should fixed delta time be used to determine timeout.
        /// </summary>
        public bool FixedUpdate { get; private set; }

        /// <summary>
        /// Time in seconds to wait before transitioning.
        /// </summary>
        public float TimeToTransition { get; private set; }

        /// <summary>
        /// Transition to another state after a period of time.
        /// </summary>
        /// <param name="targetState">New state to transition to upon trigger.</param>
        /// <param name="timeToTransition">Fixed time to transition to new state.</param>
        /// <param name="fixedUpdate">Should time from fixedDeltaTime or deltaTime be used.
        /// True corresponds to fixedDeltaTime, false corresponds to deltaTime</param>
        public TransitionAfterTimeAttribute(Type targetState, float timeToTransition, bool fixedUpdate = false)
            : base(typeof(StateTimeoutEvent), targetState)
        {
            TimeToTransition = timeToTransition;
            FixedUpdate = fixedUpdate;
        }
    }
}
