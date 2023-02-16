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
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using nickmaltbie.StateMachineUnity.netcode.Utils;
using nickmaltbie.StateMachineUnity.Utils;
using nickmaltbie.TestUtilsUnity;
using Unity.Netcode;

namespace nickmaltbie.StateMachineUnity.netcode
{
    /// <summary>
    /// Abstract state machine to manage a set of given states
    /// and transitions. Supports basic unity events for
    /// unity updates and features.
    /// </summary>
    public abstract class NetworkSMBehaviour : NetworkBehaviour, IStateMachine<Type>
    {
        /// <summary>
        /// Unity service for managing getting static unity values
        /// in a testable manner.
        /// </summary>
        public IUnityService unityService = UnityService.Instance;

        /// <summary>
        /// Delta time from the unity update function in the current state.
        /// </summary>
        protected float deltaTimeInCurrentState;

        /// <summary>
        /// Fixed delta time from the unity fixed update function in the current state.
        /// </summary>
        protected float fixedDeltaTimeInCurrentState;

        /// <summary>
        /// Has this network sm behaviour been initialized.
        /// </summary>
        protected bool initialized = false;

        /// <summary>
        /// Networked state for the state machine synced over the network communication.
        /// </summary>
        protected NetworkVariable<int> networkedState = new NetworkVariable<int>(
            value: -1,
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Owner);

        /// <summary>
        /// Current state of the state machine.
        /// </summary>
        public Type CurrentState
        {
            get => NetworkSMUtils.LookupIndexState(GetType(), networkedState.Value);
            private set => networkedState.Value = NetworkSMUtils.LookupStateIndex(GetType(), value);
        }

        /// <summary>
        /// Initializes a state machine
        /// and will set the initial
        /// state to the state defined under this class with a
        /// (InitialStateAttribute)[xref:nickmaltbie.StateMachineUnity.Attributes.InitialStateAttribute].
        /// </summary>
        public NetworkSMBehaviour()
        {
            FSMUtils.SetupCache(GetType());
            NetworkSMUtils.SetupNetworkCache(GetType());
            deltaTimeInCurrentState = 0.0f;
            fixedDeltaTimeInCurrentState = 0.0f;
        }

        /// <summary>
        /// Initialize the network SM Behaviour.
        /// </summary>
        public virtual void Start()
        {
            initialized = true;
            if (IsOwner)
            {
                FSMUtils.InitializeStateMachine(this);
            }
        }

        /// <summary>
        /// Raise a synchronous event for a given state machine.
        /// <br/>
        /// First checks if this state machine expects any events of this type
        /// for the state machine's <see cref="CurrentState"/>. These
        /// would follow an attribute of type
        /// [OnEventDoActionAttribute](xref:nickmaltbie.StateMachineUnity.Attributes.OnEventDoActionAttribute).
        /// <br/>
        /// If the state machine's <see cref="CurrentState"/> expects a transition
        /// based on the event, then this will trigger the
        /// [OnExitStateAttribute](xref:nickmaltbie.StateMachineUnity.Attributes.OnExitStateAttribute).
        /// of the <see cref="CurrentState"/>, change to the next state defined in
        /// the
        /// [OnEnterStateAttribute](xref:nickmaltbie.StateMachineUnity.Attributes.OnEnterStateAttribute).
        /// of the next state.
        /// </summary>
        /// <param name="evt">Event to send to this state machine.</param>
        public virtual void RaiseEvent(IEvent evt)
        {
            if (initialized)
            {
                FSMUtils.RaiseCachedEvent(this, evt);
            }
        }

        /// <inheritdoc/>
        public void SetStateQuiet(Type newState)
        {
            if (initialized)
            {
                deltaTimeInCurrentState = 0.0f;
                fixedDeltaTimeInCurrentState = 0.0f;
                CurrentState = newState;
            }
        }

        /// <summary>
        /// Update the <see cref="nickmaltbie.StateMachineUnity.netcode.NetworkSMBehaviour.deltaTimeInCurrentState"/>
        /// based on the [IUnityService.deltaTime](xref:nickmaltbie.TestUtilsUnity.IUnityService.deltaTime)
        /// as well as raising an instance of the [OnUpdateEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnUpdateEvent)
        /// for this state machine.
        /// </summary>
        public virtual void Update()
        {
            deltaTimeInCurrentState += unityService.deltaTime;
            if (Attribute.GetCustomAttribute(CurrentState, typeof(TransitionAfterTimeAttribute))
                    is TransitionAfterTimeAttribute timeoutAttr &&
                !timeoutAttr.FixedUpdate)
            {
                if (base.IsOwner && timeoutAttr.TimeToTransition <= deltaTimeInCurrentState)
                {
                    RaiseEvent(StateTimeoutEvent.Instance);
                }
            }

            RaiseEvent(OnUpdateEvent.Instance);
        }

        /// <summary>
        /// Update the <see cref="nickmaltbie.StateMachineUnity.netcode.NetworkSMBehaviour.fixedDeltaTimeInCurrentState"/>
        /// based on the [IUnityService.fixedDeltaTime](xref:nickmaltbie.TestUtilsUnity.IUnityService.fixedDeltaTime)
        /// as well as raising an instance of the [OnFixedUpdateEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnFixedUpdateEvent)
        /// for this state machine.
        /// </summary>
        public virtual void FixedUpdate()
        {
            fixedDeltaTimeInCurrentState += unityService.fixedDeltaTime;
            if (Attribute.GetCustomAttribute(CurrentState, typeof(TransitionAfterTimeAttribute))
                    is TransitionAfterTimeAttribute timeoutAttr &&
                timeoutAttr.FixedUpdate)
            {
                if (timeoutAttr.TimeToTransition <= fixedDeltaTimeInCurrentState)
                {
                    RaiseEvent(StateTimeoutEvent.Instance);
                }
            }

            RaiseEvent(OnFixedUpdateEvent.Instance);
        }

        /// <summary>
        /// Raises an instance of the [OnLateUpdateEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnLateUpdateEvent)
        /// for this state machine.
        /// </summary>
        public virtual void LateUpdate()
        {
            RaiseEvent(OnLateUpdateEvent.Instance);
        }

        /// <summary>
        /// Raises an instance of the [OnGUIEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnGUIEvent)
        /// for this state machine.
        /// </summary>
        public virtual void OnGUI()
        {
            RaiseEvent(OnGUIEvent.Instance);
        }

        /// <summary>
        /// Raises an instance of the [OnEnableEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnEnableEvent)
        /// for this state machine.
        /// </summary>
        public virtual void OnEnable()
        {
            RaiseEvent(OnEnableEvent.Instance);
        }

        /// <summary>
        /// Raises an instance of the [OnDisableEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnDisableEvent)
        /// for this state machine.
        /// </summary>
        public virtual void OnDisable()
        {
            RaiseEvent(OnDisableEvent.Instance);
        }

        /// <summary>
        /// Raises an instance of the [OnAnimatorIKEvent](xref:nickmaltbie.StateMachineUnity.Attributes.OnAnimatorIKEvent)
        /// for this state machine.
        /// </summary>
        public virtual void OnAnimatorIK()
        {
            RaiseEvent(OnAnimatorIKEvent.Instance);
        }
    }
}
