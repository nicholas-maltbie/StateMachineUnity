// // Copyright (C) 2022 Nicholas Maltbie
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// // associated documentation files (the "Software"), to deal in the Software without restriction,
// // including without limitation the rights to use, copy, modify, merge, publish, distribute,
// // sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in all copies or
// // substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// // BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// // CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// // ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// // SOFTWARE.

// using System;
// using System.Collections.Concurrent;
// using Moq;
// using nickmaltbie.StateMachineUnity.Attributes;
// using nickmaltbie.StateMachineUnity.Event;
// using nickmaltbie.StateMachineUnity.Fixed;
// using nickmaltbie.StateMachineUnity.Tests.EditMode.Event;
// using nickmaltbie.TestUtilsUnity;
// using nickmaltbie.TestUtilsUnity.Tests.TestCommon;
// using NUnit.Framework;
// using static nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedStateMachineMonoBehaviour;

// namespace nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed
// {

//     /// <summary>
//     /// Basic tests for <see cref="nickmaltbie.StateMachineUnity.Fixed.FixedSMBehaviour"/> in edit mode.
//     /// </summary>
//     [TestFixture]
//     public class StateMachineBehaviourTests : TestBase
//     {
//         private Mock<IUnityService> unityServiceMock;
//         private DemoFixedStateMachineMonoBehaviour sm;

//         [SetUp]
//         public override void Setup()
//         {
//             base.Setup();
//             sm = CreateGameObject().AddComponent<DemoFixedStateMachineMonoBehaviour>();

//             unityServiceMock = new Mock<IUnityService>();
//             unityServiceMock.Setup(e => e.deltaTime).Returns(1.0f);
//             unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1.0f);

//             sm.unityService = unityServiceMock.Object;

//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnFixedUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnLateUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnGUIAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnEnableAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnDisableAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnAnimatorIKAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState)), t => 0), 0);
//         }

//         [Test]
//         public void TimoutAfterUpdate()
//         {
//             Assert.AreEqual(sm.CurrentState, typeof(StartingState));
//             sm.Update();
//             Assert.AreEqual(sm.CurrentState, typeof(StartingState));

//             unityServiceMock.Setup(e => e.deltaTime).Returns(1000.0f);
//             sm.Update();
//             Assert.AreEqual(sm.CurrentState, typeof(TimeoutState));

//             Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

//             sm.RaiseEvent(new ResetEvent());
//             Assert.AreEqual(sm.CurrentState, typeof(StartingState));
//         }

//         [Test]
//         public void FixedTimoutAfterUpdate()
//         {
//             sm.RaiseEvent(new TestEvent());
//             Assert.AreEqual(sm.CurrentState, typeof(TempFixedTimeState));
//             sm.FixedUpdate();
//             Assert.AreEqual(sm.CurrentState, typeof(TempFixedTimeState));

//             unityServiceMock.Setup(e => e.fixedDeltaTime).Returns(1000.0f);
//             sm.FixedUpdate();
//             Assert.AreEqual(sm.CurrentState, typeof(TimeoutFixedState));

//             Assert.AreEqual(sm.eventCounts[typeof(StateTimeoutEvent)], 1);

//             sm.RaiseEvent(new ResetEvent());
//             Assert.AreEqual(sm.CurrentState, typeof(StartingState));
//         }

//         [Test]
//         public void VerifyUpdateActionCounts()
//         {
//             sm.Update();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.FixedUpdate();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnFixedUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.LateUpdate();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnLateUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.OnGUI();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnGUIAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.OnEnable();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnEnableAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.OnDisable();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnDisableAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.OnAnimatorIK();
//             Assert.AreEqual(sm.actionStateCounts[(typeof(OnAnimatorIKAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StartingState))], 1);

//             sm.RaiseEvent(new AEvent());
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StateA)), t => 0), 0);

//             sm.Update();
//             Assert.AreEqual(sm.actionStateCounts.GetOrAdd((typeof(OnUpdateAttribute), typeof(DemoFixedStateMachineMonoBehaviour.StateA)), t => 0), 1);
//         }
//     }
// }
