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

using System.Linq;
using Moq;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Fixed;
using nickmaltbie.StateMachineUnity.Tests.EditMode.Event;
using nickmaltbie.TestUtilsUnity;
using nickmaltbie.TestUtilsUnity.Tests.TestCommon;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;
using static nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedSMDynamicAim;

namespace nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed
{
    public class DemoFixedSMDynamicAim : FixedSMAnim
    {
        public int CrossFadeCount = 0;
        public int CrossFadeFixedCount = 0;

        public string AnimState { get; set; } = AnimA;

        public const string AnimA = "animA";
        public const string AnimB = "animB";
        public const string AnimC = "animC";
        public const string AnimD = "animD";

        [InitialState]
        [DynamicAnimation(nameof(GetAnimState))]
        public class StateA : State { }

        [DynamicAnimation(nameof(AnimState))]
        public class StateB : State { }

        public string GetAnimState()
        {
            return AnimState;
        }
    }

    [TestFixture]
    public class StateMachineDynamicAnimTests : TestBase
    {
        public DemoFixedSMDynamicAim sm;
        public Animator anim;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            GameObject go = CreateGameObject();
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

            anim = go.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;
            sm = go.AddComponent<DemoFixedSMDynamicAim>();
            sm.Awake();
        }

        [Test]
        public void Verify_DynamicStateMachineAnimSwaps()
        {
            Assert.AreEqual(sm.CurrentState, typeof(StateA));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

            sm.Update();
            anim.Update(1.0f);

            Assert.AreEqual(sm.CurrentState, typeof(StateA));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

            sm.AnimState = AnimB;

            sm.Update();
            anim.Update(1.0f);

            Assert.AreEqual(sm.CurrentState, typeof(StateA));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimB));
        }

        [Test]
        public void Verify_DynamicStateMachineAnimSwaps_Field()
        {
            sm.SetStateQuiet(typeof(StateB));
            Assert.AreEqual(sm.CurrentState, typeof(StateB));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

            sm.Update();
            anim.Update(1.0f);

            Assert.AreEqual(sm.CurrentState, typeof(StateB));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimA));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimA));

            sm.AnimState = AnimB;

            sm.Update();
            anim.Update(1.0f);

            Assert.AreEqual(sm.CurrentState, typeof(StateB));
            Assert.AreEqual(sm.CurrentAnimationState, Animator.StringToHash(AnimB));
            Assert.AreEqual(anim.GetCurrentAnimatorStateInfo(0).shortNameHash, Animator.StringToHash(AnimB));
        }
    }
}
