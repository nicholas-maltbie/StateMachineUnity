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
using UnityEngine;

namespace nickmaltbie.StateMachineUnity.Attributes
{
    /// <summary>
    /// Transition attribute to manage transitions between states for a state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AnimationTransitionAttribute : TransitionAttribute
    {
        /// <summary>
        /// name of the animation state to play upon transition.
        /// </summary>
        public string StateName { get; private set; }

        /// <summary>
        /// Fixed time to transition to new state.
        /// </summary>
        public float TransitionTime { get; private set; }

        /// <summary>
        /// Transition to another state on a given event.
        /// </summary>
        /// <param name="triggerEvent">Trigger event to cause transition.</param>
        /// <param name="targetState">New state to transition to upon trigger.</param>
        /// <param name="stateName">Name of animation state to play upon transition.</param>
        /// <param name="transitionTime">Fixed time to transition to new state.</param>
        public AnimationTransitionAttribute(Type triggerEvent, Type targetState, string stateName, float transitionTime)
            : base(triggerEvent, targetState)
        {
            StateName = stateName;
            TransitionTime = transitionTime;
        }

        /// <inheritdoc/>
        public override void OnTransition<E>(IStateMachine<E> sm)
        {
            IAnimStateMachine<E> animStateMachine = sm as IAnimStateMachine<E>;
            Animator animator = animStateMachine.GetAnimator();
            animator.CrossFadeInFixedTime(StateName, TransitionTime);
        }
    }
}
