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
using nickmaltbie.TestUtilsUnity.Tests.TestCommon;
using NUnit.Framework;
using UnityEngine;
using static nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.DemoFixedSMPassEvents;

namespace nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed
{
    public class DemoFixedSMPassEvents : FixedSMAnim
    {
        public class DataEvent : IEvent
        {
            public string data;
        }

        public class DataEvent2 : IEvent
        {
            public string data;
        }

        public class ResetEvent : IEvent
        {
            public string data;
        }

        public string PassedData { get; private set; }

        [InitialState]
        [OnEnterState(nameof(ClearData))]
        [Transition(typeof(DataEvent), typeof(DataState))]
        [Transition(typeof(DataEvent2), typeof(NoDataState))]
        public class StartState : State { }

        [OnEnterState(nameof(PersistData))]
        [Transition(typeof(ResetEvent), typeof(StartState))]
        public class DataState : State { }

        [OnEnterState(nameof(ClearData))]
        [OnEventDoAction(typeof(DataEvent), nameof(PersistData))]
        [Transition(typeof(ResetEvent), typeof(StartState))]
        public class NoDataState : State { }

        public void PersistData(IEvent evt)
        {
            if (evt is DataEvent dataEvent)
            {
                PassedData = dataEvent.data;
            }
        }

        public void ClearData()
        {
            PassedData = "";
        }
    }

    [TestFixture]
    public class StateMachinePassEventsTests : TestBase
    {
        public DemoFixedSMPassEvents sm;
        public Animator anim;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            GameObject go = CreateGameObject();
            sm = go.AddComponent<DemoFixedSMPassEvents>();
            sm.Awake();
        }

        [Test]
        public void Validate_PersistDataPassed()
        {
            Assert.AreEqual(sm.CurrentState, typeof(StartState));
            Assert.AreEqual(sm.PassedData, string.Empty);

            var evt1 = new DataEvent { data = "helloworld" };
            sm.RaiseEvent(evt1);

            Assert.AreEqual(sm.CurrentState, typeof(DataState));
            Assert.AreEqual(sm.PassedData, evt1.data);

            sm.RaiseEvent(new ResetEvent());
            Assert.AreEqual(sm.CurrentState, typeof(StartState));
            Assert.AreEqual(sm.PassedData, string.Empty);

            var evt2 = new DataEvent2 { data = "notHelloworld" };
            sm.RaiseEvent(evt2);
            Assert.AreEqual(sm.CurrentState, typeof(NoDataState));
            Assert.AreEqual(sm.PassedData, string.Empty);

            sm.RaiseEvent(evt1);
            Assert.AreEqual(sm.CurrentState, typeof(NoDataState));
            Assert.AreEqual(sm.PassedData, evt1.data);

            sm.RaiseEvent(new ResetEvent());
            Assert.AreEqual(sm.CurrentState, typeof(StartState));
            Assert.AreEqual(sm.PassedData, string.Empty);
        }
    }
}
