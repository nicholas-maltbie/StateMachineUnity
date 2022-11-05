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
    /// Transition attribute to transition to a new state upon completion
    /// of a current animation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TransitionOnAnimationCompleteAttribute : AnimationTransitionAttribute
    {
        /// <summary>
        /// Transition to another state upon completion of the animation for
        /// the current state.
        /// </summary>
        /// <param name="targetState">New state to transition to upon trigger.</param>
        /// <param name="transitionTime">Fixed time to transition to new state.</param>
        /// <param name="fixedTimeTransition">Is this transition in fixed time (true) or normalized time (false).</param>
        public TransitionOnAnimationCompleteAttribute(Type targetState, float transitionTime = 0.0f, bool fixedTimeTransition = false)
            : base(typeof(AnimationCompleteEvent), targetState, transitionTime, fixedTimeTransition)
        {
            
        }
    }
}
