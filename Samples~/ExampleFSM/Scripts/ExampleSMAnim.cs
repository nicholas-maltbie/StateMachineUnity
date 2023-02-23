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

using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using nickmaltbie.StateMachineUnity.Fixed;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nickmaltbie.StateMachineUnity.ExampleFSM
{
    /// <summary>
    /// Example implementation of the ExampleSMAnim.
    /// </summary>
    public class ExampleSMAnim : FixedSMAnim
    {
        /// <summary>
        /// Movement action for the player.
        /// </summary>
        public InputActionReference moveAction;

        /// <summary>
        /// Jump action for the player.
        /// </summary>
        public InputActionReference jumpAction;

        /// <summary>
        /// Punch action for the player.
        /// </summary>
        public InputActionReference punchAction;

        /// <summary>
        /// Idle state for example state machine.
        /// </summary>
        [InitialState]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Animation("Idle", 0.1f)]
        [Transition(typeof(JumpEvent), typeof(JumpState))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [AnimationTransition(typeof(MoveEvent), typeof(WalkingState), 0.1f, true)]
        [TransitionAfterTime(typeof(YawnState), 10.0f, false)]
        public class IdleState : State { }

        /// <summary>
        /// Yawn animation to play after the player stands still for too long.
        /// </summary>
        [Animation("Yawn", 0.25f, true)]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [AnimationTransition(typeof(MoveEvent), typeof(WalkingState), 0.5f, true)]
        [AnimationTransition(typeof(JumpEvent), typeof(JumpState), 0.5f, true)]
        [TransitionOnAnimationComplete(typeof(IdleState), 0.35f, true)]
        public class YawnState : State { }

        /// <summary>
        /// Walking state for example state machine.
        /// </summary>
        [Animation("Walking")]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [AnimationTransition(typeof(IdleEvent), typeof(IdleState), 0.35f)]
        [AnimationTransition(typeof(JumpEvent), typeof(JumpState), 0.1f, true)]
        public class WalkingState : State { }

        /// <summary>
        /// Jump state for example state machine.
        /// </summary>
        [Animation("Jump")]
        [TransitionOnAnimationComplete(typeof(IdleState))]
        public class JumpState : State { }

        /// <summary>
        /// Punching state
        /// </summary>
        [Animation("Punching", 0.35f, true, 0.75f)]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [TransitionOnAnimationComplete(typeof(IdleState), 0.35f)]
        [AnimationTransition(typeof(JumpEvent), typeof(JumpState), 0.35f, true)]
        [AnimationTransition(typeof(MoveEvent), typeof(WalkingState), 0.35f, true)]
        public class PunchingState : State { }

        /// <summary>
        /// Event to start moving the player.
        /// </summary>
        public class MoveEvent : IEvent { }

        /// <summary>
        /// Event to stop moving the player.
        /// </summary>
        public class IdleEvent : IEvent { }

        /// <summary>
        /// Event to perform the jump action.
        /// </summary>
        public class JumpEvent : IEvent { }

        /// <summary>
        /// Event to perform the punching action.
        /// </summary>
        public class PunchEvent : IEvent { }

        /// <summary>
        /// Configure actions for the example state machine with animations.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            jumpAction.action.performed += _ => RaiseEvent(new JumpEvent());
            punchAction.action.performed += _ => RaiseEvent(new PunchEvent());
        }

        /// <summary>
        /// Check if the player is walking based on current input values.
        /// </summary>
        public void CheckWalking()
        {
            Vector2 moveValue = moveAction.action.ReadValue<Vector2>();
            if (moveValue.magnitude > 0.001f)
            {
                RaiseEvent(new MoveEvent());
                base.AttachedAnimator.SetFloat("MoveX", Mathf.Lerp(base.AttachedAnimator.GetFloat("MoveX"), moveValue.x, 5.0f * base.unityService.deltaTime));
                base.AttachedAnimator.SetFloat("MoveY", Mathf.Lerp(base.AttachedAnimator.GetFloat("MoveY"), moveValue.y, 5.0f * base.unityService.deltaTime));
            }
            else
            {
                RaiseEvent(new IdleEvent());
            }
        }
    }
}
