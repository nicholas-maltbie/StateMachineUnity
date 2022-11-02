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
    /// Transition attribute to manage transitions between states for a state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TransitionWithAnimationAttribute : TransitionAttribute
    {
        /// <summary>
        /// Animation to play upon tarnsition.
        /// </summary>
        public string Animation { get; private set; }

        /// <summary>
        /// Transition to another state on a given event.
        /// </summary>
        /// <param name="triggerEvent">Trigger event to cause transition.</param>
        /// <param name="targetState">New state to transition to upon trigger.</param>
        public TransitionWithAnimationAttribute(Type triggerEvent, Type targetState, string animation)
            : base(triggerEvent, targetState)
        {
            Animation = animation;
        }

        /// <summary>
        /// Behaviour to invoke when this transition is triggered.
        /// </summary>
        public override void OnTransition()
        {

        }
    }
}
