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
    /// Interface to represent state machine with a reference to
    /// an animator.
    /// </summary>
    public interface IAnimStateMachine<E> : IStateMachine<E>
    {
        /// <summary>
        /// Gets the animator associated with this state machine.
        /// </summary>
        /// <returns>Get the animator associated with this state machine.</returns>
        public Animator GetAnimator();

        /// <summary>
        /// Current target animation state for the animator.
        /// </summary>
        public int CurrentAnimationState { get; }

        /// <summary>
        /// Set the animation state of the anim state machine using a normalized transition time.
        /// </summary>
        /// <param name="animState">Description of animation state to transition
        /// anim state machine into.</param>
        /// <param name="layerIdx">Layer to trigger animation in animator.</param>
        public void CrossFade(AnimSMRequest animState, int layerIdx = 0);
    }
}
