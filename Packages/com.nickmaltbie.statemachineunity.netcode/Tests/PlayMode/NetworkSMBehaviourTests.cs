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
using System.Collections;
using System.Collections.Concurrent;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using nickmaltbie.TestUtilsUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static nickmaltbie.StateMachineUnity.netcode.Tests.PlayMode.DemoNetworkSMBehaviour;

namespace nickmaltbie.StateMachineUnity.netcode.Tests.PlayMode
{
    /// <summary>
    /// Demo state machine for 
    /// </summary>
    public class DemoNetworkSMBehaviour : NetworkSMBehaviour
    {
        /// <summary>
        /// Counts of actions for each combination of (Action, State).
        /// </summary>
        public ConcurrentDictionary<(Type, Type), int> actionStateCounts = new ConcurrentDictionary<(Type, Type), int>();

        /// <summary>
        /// Times that a state is entered.
        /// </summary>
        public ConcurrentDictionary<Type, int> onEntryCount = new ConcurrentDictionary<Type, int>();

        /// <summary>
        /// Times that a state is exited.
        /// </summary>
        public ConcurrentDictionary<Type, int> onExitCount = new ConcurrentDictionary<Type, int>();

        /// <summary>
        /// Times that specific events have been raised
        /// </summary>
        public ConcurrentDictionary<Type, int> eventCounts = new ConcurrentDictionary<Type, int>();

        [InitialState]
        [Transition(typeof(AEvent), typeof(StateA))]
        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [Transition(typeof(TestEvent), typeof(TempFixedTimeState))]
        [TransitionAfterTime(typeof(TimeoutState), 3.0f)]
        [OnEnterState(nameof(OnEnterStartingState))]
        [OnExitState(nameof(OnExitStartingState))]
        [OnUpdate(nameof(OnUpdateStartingState))]
        [OnFixedUpdate(nameof(OnFixedUpdateStartingState))]
        [OnLateUpdate(nameof(OnLateUpdateStartingState))]
        [OnGUI(nameof(OnGUIStartingState))]
        [OnEnable(nameof(OnEnableStartingState))]
        [OnDisable(nameof(OnDisableStartingState))]
        [OnAnimatorIK(nameof(OnAnimatorIKStartingState))]
        public class StartingState : State { }

        [TransitionAfterTime(typeof(TimeoutFixedState), 3.0f, true)]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TempFixedTimeState : State { }

        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        [OnUpdate(nameof(OnUpdateStateA))]
        [OnEventDoAction(typeof(AEvent), nameof(DoNothing))]
        [OnEventDoAction(typeof(OnUpdateEvent), nameof(DoNothing))]
        public class StateA : State { }

        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class StateB : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class StateC : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TimeoutState : State { }

        [Transition(typeof(ResetEvent), typeof(StartingState))]
        [OnEnterState(nameof(IncrementStateEntryCount))]
        public class TimeoutFixedState : State { }

        public void OnUpdateStateA()
        {
            actionStateCounts.AddOrUpdate((typeof(OnUpdateAttribute), typeof(StateA)), 1, (_, v) => v + 1);
        }

        public void DoNothing()
        {

        }

        public override void RaiseEvent(IEvent evt)
        {
            base.RaiseEvent(evt);
            eventCounts.AddOrUpdate(evt.GetType(), 1, (Type _, int v) => v + 1);
        }

        public void OnUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnFixedUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnFixedUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnLateUpdateStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnLateUpdateAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnGUIStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnGUIAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnEnableStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnEnableAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnDisableStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnDisableAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnAnimatorIKStartingState()
        {
            actionStateCounts.AddOrUpdate((typeof(OnAnimatorIKAttribute), typeof(StartingState)), 1, (_, v) => v + 1);
        }

        public void OnEnterStartingState()
        {
            UnityEngine.Debug.Log("On Enter Starting State Invoked");
            onEntryCount.AddOrUpdate(typeof(StartingState), 1, (Type t, int v) => v + 1);
        }

        public void OnExitStartingState()
        {
            onExitCount.AddOrUpdate(typeof(StartingState), 1, (Type t, int v) => v + 1);
        }

        public void IncrementStateEntryCount()
        {
            onEntryCount.AddOrUpdate(CurrentState, 1, (Type t, int v) => v + 1);
        }
    }

    /// <summary>
    /// Simple tests meant to be run in PlayMode
    /// </summary>
    [TestFixture]
    public class NetworkSMBehaviourTests : NetcodeRuntimeTest<DemoNetworkSMBehaviour>
    {
        protected override int NumberOfClients => 2;

        private MockUnityService unityServiceMock;

        public override void SetupPrefab(GameObject go)
        {
            go.GetComponent<DemoNetworkSMBehaviour>().unityService = unityServiceMock;
        }

        [UnitySetUp]
        public override IEnumerator UnitySetUp()
        {
            unityServiceMock = new MockUnityService();
            yield return base.UnitySetUp();
            yield return WaitForSMReady();

            ForEachOwner((anim, i) => anim.unityService = unityServiceMock);
        }

        [UnityTest]
        public IEnumerator TimeoutAfterUpdate()
        {
            yield return WaitForSMReady();
            base.ForEachOwner((sm, idx) =>
            {
                unityServiceMock.deltaTime = 1.0f;
                sm.SetStateQuiet(typeof(StartingState));
                sm.unityService = unityServiceMock;

                Assert.AreEqual(typeof(StartingState), sm.CurrentState);
                sm.Update();
                Assert.AreEqual(typeof(StartingState), sm.CurrentState);

                unityServiceMock.deltaTime = 1000.0f;
                sm.Update();
                Assert.AreEqual(typeof(TimeoutState), sm.CurrentState);

                Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

                sm.RaiseEvent(new ResetEvent());
                Assert.AreEqual(typeof(StartingState), sm.CurrentState);
            });
        }

        [UnityTest]
        public IEnumerator FixedTimeoutAfterUpdate()
        {
            yield return WaitForSMReady();
            base.ForEachOwner((sm, idx) =>
            {
                unityServiceMock.fixedDeltaTime = 1.0f;
                sm.SetStateQuiet(typeof(StartingState));

                sm.RaiseEvent(new TestEvent());
                Assert.AreEqual(typeof(TempFixedTimeState), sm.CurrentState);
                sm.FixedUpdate();
                Assert.AreEqual(typeof(TempFixedTimeState), sm.CurrentState);

                unityServiceMock.fixedDeltaTime = 1000.0f;
                sm.FixedUpdate();
                Assert.AreEqual(typeof(TimeoutFixedState), sm.CurrentState);

                Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

                sm.RaiseEvent(new ResetEvent());
                Assert.AreEqual(typeof(StartingState), sm.CurrentState);
            });
        }

        [UnityTest]
        public IEnumerator VerifyUpdateActionCounts()
        {
            yield return WaitForSMReady();
            base.ForEachOwner((sm, idx) =>
            {
                unityServiceMock.fixedDeltaTime = 1.0f;
                sm.unityService = unityServiceMock;
                sm.SetStateQuiet(typeof(StartingState));

                sm.Update();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnUpdateAttribute), typeof(StartingState))], 1);

                sm.FixedUpdate();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnFixedUpdateAttribute), typeof(StartingState))], 1);

                sm.LateUpdate();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnLateUpdateAttribute), typeof(StartingState))], 1);

                sm.OnGUI();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnGUIAttribute), typeof(StartingState))], 1);

                sm.OnEnable();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnEnableAttribute), typeof(StartingState))], 1);

                sm.OnDisable();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnDisableAttribute), typeof(StartingState))], 1);

                sm.OnAnimatorIK();
                Assert.GreaterOrEqual(sm.actionStateCounts[(typeof(OnAnimatorIKAttribute), typeof(StartingState))], 1);

                sm.RaiseEvent(new AEvent());
                Assert.GreaterOrEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(StateA)), t => 0), 0);

                sm.Update();
                Assert.GreaterOrEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(StateA)), t => 0), 1);
            });
        }

        protected IEnumerator WaitForSMReady()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    DemoNetworkSMBehaviour sm = GetAttachedNetworkBehaviour(i, j);
                    sm.Start();
                    sm.OnNetworkSpawn();
                    int k = 0;
                    while (k < 1000 && sm.CurrentState != typeof(StartingState))
                    {
                        yield return new WaitForSeconds(0.0f);
                        k++;
                    }

                    Assert.AreEqual(typeof(StartingState), sm.CurrentState);
                }
            }
        }
    }
}
