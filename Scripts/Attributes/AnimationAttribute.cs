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
    /// Animation attribute to control animations for a state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AnimationAttribute : Attribute
    {
        /// <summary>
        /// Animation hash associated with the state this is attached to.
        /// </summary>
        public int AnimationHash { get; private set; }

        /// <summary>
        /// Default transition time when transitioning to this animation.
        /// </summary>
        public float DefaultTransitionTime { get; private set; }

        /// <summary>
        /// Associate an animation with a given state.
        /// </summary>
        /// <param name="stateName">String name of the state.</param>
        /// <param name="defaultTransitionTime">Default transition time when transitioning to this animation.</param>
        public AnimationAttribute(string stateName, float defaultTransitionTime = 0.0f)
            : this(Animator.StringToHash(stateName), defaultTransitionTime) { }

        /// <summary>
        /// Associate an animation with a given state.
        /// </summary>
        /// <param name="animationHash">Hash of the animation state</param>
        /// <param name="defaultTransitionTime">Default transition time when transitioning to this animation.</param>
        public AnimationAttribute(int animationHash, float defaultTransitionTime = 0.0f)
        {
            AnimationHash = animationHash;
            DefaultTransitionTime = defaultTransitionTime;
        }

        /// <summary>
        /// Gets the animation associated with a given state.
        /// </summary>
        /// <param name="state">State to search attribute for.</param>
        /// <returns></returns>
        public static int? GetStateAnimation(Type state)
        {
            var animAttribute = GetCustomAttribute(state, typeof(AnimationAttribute)) as AnimationAttribute;
            return animAttribute?.AnimationHash;
        }
    }
}
