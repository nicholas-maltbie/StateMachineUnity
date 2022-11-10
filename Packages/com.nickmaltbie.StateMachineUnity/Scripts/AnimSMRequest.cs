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

using UnityEngine;

namespace nickmaltbie.StateMachineUnity
{
    /// <summary>
    /// Animation request for an animation state machine.
    /// </summary>
    public readonly struct AnimSMRequest
    {
        /// <summary>
        /// Target state to transition into.
        /// </summary>
        public readonly int targetStateHash;

        /// <summary>
        /// Time to take between transition.
        /// </summary>
        public readonly float transitionTime;

        /// <summary>
        /// Is the transition in fixed time or normalized time.
        /// </summary>
        public readonly bool fixedTimeTransition;

        /// <summary>
        /// Should the animation be locked for a given number of
        /// seconds before transitioning to a new state.
        /// </summary>
        public readonly float lockAnimationTime;

        public AnimSMRequest(
            string targetStateName,
            float transitionTime = 0.0f,
            bool fixedTimeTransition = false,
            float lockAnimationTime = 0.0f)
        : this(
            Animator.StringToHash(targetStateName),
            transitionTime,
            fixedTimeTransition,
            lockAnimationTime) { }

        public AnimSMRequest(
            int targetStateHash,
            float transitionTime = 0.0f,
            bool fixedTimeTransition = false,
            float lockAnimationTime = 0.0f)
        {
            this.targetStateHash = targetStateHash;
            this.transitionTime = transitionTime;
            this.fixedTimeTransition = fixedTimeTransition;
            this.lockAnimationTime = lockAnimationTime;
        }
    }
}
