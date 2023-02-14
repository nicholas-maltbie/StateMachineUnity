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
using Moq;
using nickmaltbie.StateMachineUnity;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.TestUtilsUnity;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.TestHelpers.Runtime;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;
using static nickmaltbie.StateMachineUnity.netcode.Tests.PlayMode.DemoNetworkSMAnim;

namespace nickmaltbie.StateMachineUnity.netcode.Tests.PlayMode
{
    /// <summary>
    /// Demo state machine for 
    /// </summary>
    public class DemoNetworkSMAnim : NetworkSMAnim
    {
        // indexed by [object, machine]
        public static DemoNetworkSMAnim[,] Objects = new DemoNetworkSMAnim[3, 3];
        public static int CurrentlySpawning = 0;

        public const string AnimA = "animA";
        public const string AnimB = "animB";
        public const string AnimC = "animC";
        public const string AnimD = "animD";

        public int CrossFadeCount = 0;

        public Mock<IUnityService> unityServiceMock;

        [InitialState]
        [Animation(AnimA)]
        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(CEvent), typeof(StateC))]
        [AnimationTransition(typeof(DEvent1), typeof(StateD), fixedTimeTransition: false)]
        [AnimationTransition(typeof(DEvent2), typeof(StateD), fixedTimeTransition: true)]
        [TransitionOnAnimationComplete(typeof(TimeoutState))]
        public class StateA : State { }

        [Animation(AnimB)]
        [AnimationTransition(typeof(AEvent), typeof(StateA), 0.5f, false)]
        [Transition(typeof(CEvent), typeof(StateC))]
        public class StateB : State { }

        [Animation(AnimC)]
        [AnimationTransition(typeof(AEvent), typeof(StateA), 0.5f, true)]
        [Transition(typeof(BEvent), typeof(StateB))]
        public class StateC : State { }

        [Animation(AnimD)]
        [AnimationTransition(typeof(AEvent), typeof(StateA))]
        public class StateD : State { }

        public class TimeoutState : State { }

        public void Awake()
        {
            unityServiceMock = new Mock<IUnityService>();
            unityServiceMock.Setup(e => e.deltaTime).Returns(0.1f);
            unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(0.1f);

            var controller = new AnimatorController();

            // Add State Machines
            controller.AddLayer("base");
            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

            // Add States
            AnimatorState stateA = rootStateMachine.AddState(AnimA);
            AnimatorState stateB = rootStateMachine.AddState(AnimB);
            AnimatorState stateC = rootStateMachine.AddState(AnimC);

            // Add animation
            var clipA = new AnimationClip();
            stateA.motion = clipA;

            Animator anim = GetComponent<Animator>();
            anim.runtimeAnimatorController = controller;
            unityService = unityServiceMock.Object;
        }

        public override void OnNetworkSpawn()
        {
            Objects[CurrentlySpawning, NetworkManager.LocalClientId] = GetComponent<DemoNetworkSMAnim>();
            Debug.Log($"Object index ({CurrentlySpawning}) spawned on client {NetworkManager.LocalClientId}");
        }

        public override void CrossFade(AnimSMRequest req, int layerIdx = 0)
        {
            base.CrossFade(req, layerIdx);
            if (req.fixedTimeTransition)
            {
                CrossFadeCount++;
            }
            else
            {
                CrossFadeCount++;
            }
        }
    }

    /// <summary>
    /// Simple tests meant to be run in PlayMode
    /// </summary>
    [TestFixture]
    public class NetworkSMAnimTests : NetcodeIntegrationTest
    {
        protected override int NumberOfClients => 2;

        private GameObject m_PrefabToSpawn;

        private Mock<IUnityService> unityServiceMock;

        protected override void OnServerAndClientsCreated()
        {
            m_PrefabToSpawn = CreateNetworkObjectPrefab("DemoNetworkSMBehaviour");
            m_PrefabToSpawn.AddComponent<Animator>();
            m_PrefabToSpawn.AddComponent<DemoNetworkSMAnim>();
        }

        [UnitySetUp]
        public IEnumerator SetupTest()
        {
            unityServiceMock = new Mock<IUnityService>();

            // create 3 objects
            for (int objectIndex = 0; objectIndex < 3; objectIndex++)
            {
                DemoNetworkSMAnim.CurrentlySpawning = objectIndex;

                NetworkManager ownerManager = m_ServerNetworkManager;
                if (objectIndex != 0)
                {
                    ownerManager = m_ClientNetworkManagers[objectIndex - 1];
                }

                DemoNetworkSMAnim demoBehaviour = SpawnObject(m_PrefabToSpawn, ownerManager).GetComponent<DemoNetworkSMAnim>();
                demoBehaviour.unityService = unityServiceMock.Object;

                // wait for each object to spawn on each client
                for (int clientIndex = 0; clientIndex < 3; clientIndex++)
                {
                    while (DemoNetworkSMAnim.Objects[objectIndex, clientIndex] == null)
                    {
                        yield return new WaitForSeconds(0.0f);
                    }
                }
            }
        }

        private void InternalTestHelper(Action<DemoNetworkSMAnim, Animator> testAction)
        {
            for (int i = 0; i < 3; i++)
            {
                unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
                DemoNetworkSMAnim sm = DemoNetworkSMAnim.Objects[i, i];
                sm.SetStateQuiet(typeof(StateA));
                sm.Start();
                sm.unityService = unityServiceMock.Object;
                Animator anim = sm.GetComponent<Animator>();
                testAction(sm, anim);
            }
        }

        [Test]
        public void TimeoutAfterUpdate()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(typeof(StateA), sm.CurrentState);
                Assert.AreEqual(Animator.StringToHash(AnimA), sm.CurrentAnimationState);
                Assert.AreEqual(Animator.StringToHash(AnimA), anim.GetCurrentAnimatorStateInfo(0).shortNameHash);

                unityServiceMock.Setup(e => e.deltaTime).Returns(10.0f);
                anim.Play(AnimA, 0, 0.0f);
                anim.Update(1.0f);
                sm.Update();
                Assert.AreEqual(typeof(StateA), sm.CurrentState);

                anim.Play(AnimA, 0, 1.0f);
                anim.Update(1.0f);
                sm.Update();
                Assert.AreEqual(typeof(TimeoutState), sm.CurrentState);
            });
        }

        [Test]
        public void TestAnimationLockPending()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                // Manually cross fade with a lock to anim C for 5 seconds
                unityServiceMock.Setup(e => e.time).Returns(0.0f);
                sm.CrossFade(new AnimSMRequest(AnimC, lockAnimationTime: 5.0f));

                // Assert that we are now in the AnimC animation.
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimC));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimC));
                Assert.AreEqual(null, sm.PendingReq);

                // Update the SM and assert that we don't save a new pending state
                sm.Update();
                Assert.AreEqual(null, sm.PendingReq);
            });
        }

        [Test]
        public void TestAnimationTransitionWithLocks()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                // Manually cross fade with a lock to anim C for 5 seconds
                unityServiceMock.Setup(e => e.time).Returns(0.0f);
                sm.CrossFade(new AnimSMRequest(AnimC, lockAnimationTime: 5.0f));

                // Assert that we are now in the AnimC animation.
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimC));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimC));

                // If we attempt to transition to AnimB, verify it is saved as pending request
                var bRequest = new AnimSMRequest(AnimB);
                sm.CrossFade(bRequest);
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimC));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimC));
                Assert.AreEqual(bRequest, sm.PendingReq);

                // If we wait for time to expire, assert that we read the pending
                // animation instead of the current state
                unityServiceMock.Setup(e => e.time).Returns(10);
                sm.Update();
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimB));
                Assert.AreEqual(null, sm.PendingReq);

                // If we update again, it should read the animation from state A
                // Since there is no longer any pending animation.
                sm.Update();
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));
            });
        }

        [Test]
        public void VerifyTransitionOnTimeout()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                unityServiceMock.Setup(e => e.deltaTime).Returns(10.0f);
                anim.Play(AnimA, 0, 0.0f);
                anim.Update(1.0f);
                sm.Update();
                Assert.AreEqual(typeof(StateA), sm.CurrentState);

                anim.Play(AnimA, 0, 1.0f);
                anim.Update(1.0f);
                sm.Update();
                Assert.AreEqual(typeof(TimeoutState), sm.CurrentState);
            });
        }

        [Test]
        public void VerifyAnimationGetAnimator()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.GetAnimator(), anim);
            });
        }

        [Test]
        public void VerifyAnimationTransitions()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                sm.RaiseEvent(new BEvent());
                Assert.AreEqual(sm.CurrentState, typeof(StateB));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                sm.Update();
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
                anim.Update(1.0f);
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimB));

                sm.RaiseEvent(new CEvent());
                Assert.AreEqual(sm.CurrentState, typeof(StateC));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
                sm.Update();
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimC));
                anim.Update(1.0f);
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimC));

                sm.RaiseEvent(new AEvent());
                anim.Update(1.0f);
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));
            });
        }

        [Test]
        public void VerifyAnimationTransitionStateCrossFade()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                sm.RaiseEvent(new BEvent());
                sm.CrossFadeCount = 0;
                sm.RaiseEvent(new AEvent());
                Assert.AreEqual(sm.CrossFadeCount, 1);
            });
        }

        [UnityTest]
        public IEnumerator VerifyAnimationTransitionStateCrossFadeFixed()
        {
            yield return null;
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                sm.RaiseEvent(new CEvent());
                sm.CrossFadeCount = 0;
                sm.RaiseEvent(new AEvent());
                Assert.AreEqual(sm.CrossFadeCount, 1);
            });
        }

        [UnityTest]
        public IEnumerator VerifyTransitionToUnknownAnimationStateCrossFade()
        {
            yield return null;
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                sm.CrossFadeCount = 0;
                sm.RaiseEvent(new DEvent1());
                Assert.AreEqual(sm.CrossFadeCount, 1);

                UnityEngine.TestTools.LogAssert.Expect(
                    LogType.Error,
                    $"Warning, did not find expected stateId:{Animator.StringToHash(AnimD)} in layer:0 for animator:{anim.name}");
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimD));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));
            });
        }

        [UnityTest]
        public IEnumerator VerifyTransitionOnClients()
        {
            for (int i = 0; i < 3; i++)
            {
                unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
                DemoNetworkSMAnim sm = DemoNetworkSMAnim.Objects[i, i];
                sm.SetStateQuiet(typeof(StateA));
                sm.Start();
                sm.unityService = unityServiceMock.Object;
                Animator anim = sm.GetComponent<Animator>();

                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));

                for (int clientIdx = 0; clientIdx < 3; clientIdx++)
                {
                    DemoNetworkSMAnim sm2 = DemoNetworkSMAnim.Objects[i, clientIdx];
                    while (!(sm2.CurrentState == typeof(StateA) &&
                        sm2.CurrentAnimationState == Animator.StringToHash(AnimA)))
                    {
                        yield return new WaitForSeconds(0.0f);
                    }
                }

                sm.RaiseEvent(new BEvent());
                Assert.AreEqual(sm.CurrentState, typeof(StateB));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                sm.Update();
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
                anim.Update(1.0f);
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimB));

                for (int clientIdx = 0; clientIdx < 3; clientIdx++)
                {
                    DemoNetworkSMAnim sm2 = DemoNetworkSMAnim.Objects[i, clientIdx];
                    UnityEngine.Debug.Log($"Query: clientIdx:{clientIdx}, sm2.CurrentAnimationState:{sm2.CurrentAnimationState}");
                    while (sm2.CurrentAnimationState != Animator.StringToHash(AnimB))
                    {
                        UnityEngine.Debug.Log($"Pending: clientIdx:{clientIdx}, expected:{Animator.StringToHash(AnimB)} sm2.CurrentAnimationState:{sm2.CurrentAnimationState}");
                        yield return null;
                    }

                    UnityEngine.Debug.Log($"Resolved: clientIdx:{clientIdx}, expected:{Animator.StringToHash(AnimB)} sm2.CurrentAnimationState:{sm2.CurrentAnimationState}");
                }
            }
        }

        [UnityTest]
        public IEnumerator VerifyCrossFadeNotOwner()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
                    DemoNetworkSMAnim sm = DemoNetworkSMAnim.Objects[i, j];
                    sm.Start();
                    sm.unityService = unityServiceMock.Object;
                    Animator anim = sm.GetComponent<Animator>();

                    while (sm.CurrentState != typeof(StateA))
                    {
                        yield return null;
                    }

                    sm.CrossFade(new AnimSMRequest(AnimB));

                    Assert.AreEqual(sm.CurrentState, typeof(StateA));
                    Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                    Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));
                }
            }
        }

        [Test]
        public void VerifyTransitionToUnknownAnimationStateCrossFadeFixed()
        {
            InternalTestHelper((DemoNetworkSMAnim sm, Animator anim) =>
            {
                Assert.AreEqual(sm.CurrentState, typeof(StateA));
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

                sm.CrossFadeCount = 0;
                sm.RaiseEvent(new DEvent2());
                Assert.AreEqual(sm.CrossFadeCount, 1);

                UnityEngine.TestTools.LogAssert.Expect(
                    LogType.Error,
                    $"Warning, did not find expected stateId:{Animator.StringToHash(AnimD)} in layer:0 for animator:{anim.name}");
                Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimD));
                Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));
            });
        }
    }
}
