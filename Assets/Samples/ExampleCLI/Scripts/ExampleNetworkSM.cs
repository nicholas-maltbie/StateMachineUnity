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

using nickmaltbie.StateMachineUnity;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nickmaltbie.StateMachineUnity.netcode.Example
{
    /// <summary>
    /// Example implementation of the ExampleSMAnim.
    /// </summary>
    public class ExampleNetworkSM : NetworkSMBehaviour
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
        [Transition(typeof(JumpEvent), typeof(JumpState))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [Transition(typeof(MoveEvent), typeof(WalkingState))]
        [TransitionAfterTime(typeof(YawnState), 10.0f, false)]
        public class IdleState : State { }

        /// <summary>
        /// Yawn animation to play after the player stands still for too long.
        /// </summary>
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [Transition(typeof(MoveEvent), typeof(WalkingState))]
        [Transition(typeof(JumpEvent), typeof(JumpState))]
        [TransitionAfterTime(typeof(IdleState), 3.0f)]
        public class YawnState : State { }

        /// <summary>
        /// Walking state for example state machine.
        /// </summary>
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [Transition(typeof(IdleEvent), typeof(IdleState))]
        [Transition(typeof(JumpEvent), typeof(JumpState))]
        public class WalkingState : State { }

        /// <summary>
        /// Jump state for example state machine.
        /// </summary>
        [TransitionAfterTime(typeof(IdleState), 3.0f)]
        public class JumpState : State { }

        /// <summary>
        /// Punching state
        /// </summary>
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [TransitionAfterTime(typeof(IdleState), 1.0f)]
        [Transition(typeof(JumpEvent), typeof(JumpState))]
        [Transition(typeof(MoveEvent), typeof(WalkingState))]
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
        public void Awake()
        {
            jumpAction.action.performed += _ =>
            {
                if (base.IsOwner)
                {
                    RaiseEvent(new JumpEvent());
                }
            };

            punchAction.action.performed += _ =>
            {
                if (base.IsOwner)
                {
                    RaiseEvent(new PunchEvent());
                }
            };
        }

        /// <summary>
        /// Check if the player is walking based on current input values.
        /// </summary>
        public void CheckWalking()
        {
            if (base.IsOwner)
            {
                Vector2 moveValue = moveAction.action.ReadValue<Vector2>();
                if (moveValue.magnitude > 0.001f)
                {
                    RaiseEvent(new MoveEvent());
                }
                else
                {
                    RaiseEvent(new IdleEvent());
                }
            }
        }
    }
}
