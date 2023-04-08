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
    /// Dynamic animation attribute to control animations for a state machine
    /// via a function instead of a simple value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DynamicAnimationAttribute : AnimationAttribute
    {
        /// <summary>
        /// Evaluation function for the animation state.
        /// </summary>
        protected string AnimationStateFn { get; set; }

        /// <summary>
        /// Cached previous state name.
        /// </summary>
        private string previousState = null;

        /// <summary>
        /// Cached previous state hash value.
        /// </summary>
        private int cachedValue = -1;

        /// <inheritdoc/>
        public override int AnimationHash
        {
            get
            {
                if (previousState == StateName)
                {
                    return cachedValue;
                }

                previousState = StateName;
                return cachedValue = Animator.StringToHash(StateName);
            }
        }

        /// <summary>
        /// Update the state of this animation attribute based on the current
        /// instance configuration.
        /// </summary>
        /// <param name="instance">Instance to modify</param>
        public void UpdateState(object instance)
        {
            Type type = instance.GetType();
            StateName = type.GetMethod(AnimationStateFn)?.Invoke(instance, new object[0]) as string
                ?? type.GetField(AnimationStateFn)?.GetValue(instance) as string
                ?? type.GetProperty(AnimationStateFn)?.GetValue(instance) as string;
        }

        /// <summary>
        /// Initializes an instance of Dynamic animation attribute.
        /// </summary>
        /// <param name="animationStateFn">Animation state function for evaluating current animation state.</param>
        /// <param name="defaultTransitionTime">Default transition time when transitioning to this animation.</param>
        /// <param name="fixedTimeTransition">Is this transition in fixed time (true) or normalized time (false).</param>
        /// <param name="animationLockTime">Time to lock animation during transition.</param>
        public DynamicAnimationAttribute(
            string animationStateFn,
            float defaultTransitionTime = 0.0f,
            bool fixedTimeTransition = false,
            float animationLockTime = 0.0f)
            : base(string.Empty, defaultTransitionTime, fixedTimeTransition, animationLockTime)
        {
            AnimationStateFn = animationStateFn;
            DefaultTransitionTime = defaultTransitionTime;
        }
    }
}
