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
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nickmaltbie.StateMachineUnity.netcode.ExampleAnim
{
    /// <summary>
    /// Example implementation of the ExampleSMAnim.
    /// </summary>
    public class ExampleNetworkSMAnim : NetworkSMAnim
    {
        /// <summary>
        /// Move vector for player animation
        /// </summary>
        private NetworkVariable<Vector2> moveVectorAnim =
            new NetworkVariable<Vector2>(
                readPerm: NetworkVariableReadPermission.Everyone,
                writePerm: NetworkVariableWritePermission.Owner);

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
        /// Idle NetworkState for example NetworkState machine.
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
        /// Walking NetworkState for example NetworkState machine.
        /// </summary>
        [Animation("Walking")]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(CheckWalking))]
        [Transition(typeof(PunchEvent), typeof(PunchingState))]
        [AnimationTransition(typeof(IdleEvent), typeof(IdleState), 0.35f)]
        [AnimationTransition(typeof(JumpEvent), typeof(JumpState), 0.1f, true)]
        public class WalkingState : State { }

        /// <summary>
        /// Jump NetworkState for example NetworkState machine.
        /// </summary>
        [Animation("Jump")]
        [TransitionOnAnimationComplete(typeof(IdleState))]
        public class JumpState : State { }

        /// <summary>
        /// Punching NetworkState
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
        /// Configure actions for the example NetworkState machine with animations.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

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

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            base.AttachedAnimator.SetFloat("MoveX", moveVectorAnim.Value.x);
            base.AttachedAnimator.SetFloat("MoveY", moveVectorAnim.Value.y);
        }

        /// <summary>
        /// Check if the player is walking based on current input values.
        /// </summary>
        public void CheckWalking()
        {
            if (base.IsOwner)
            {
                Vector2 moveValue = moveAction.action.ReadValue<Vector2>();

                transform.position += new Vector3(moveValue.x, 0, moveValue.y) * base.unityService.deltaTime;
                moveVectorAnim.Value = new Vector2(
                    Mathf.Lerp(base.AttachedAnimator.GetFloat("MoveX"), moveValue.x, 5.0f * base.unityService.deltaTime),
                    Mathf.Lerp(base.AttachedAnimator.GetFloat("MoveY"), moveValue.y, 5.0f * base.unityService.deltaTime)
                );

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
