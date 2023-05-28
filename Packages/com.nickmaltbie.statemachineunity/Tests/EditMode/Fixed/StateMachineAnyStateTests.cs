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
using System.Collections.Concurrent;
using System.Collections.Generic;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.StateMachineUnity.Event;
using nickmaltbie.StateMachineUnity.Fixed;
using nickmaltbie.StateMachineUnity.Tests.EditMode.Event;
using nickmaltbie.StateMachineUnity.Utils;
using nickmaltbie.TestUtilsUnity.Tests.TestCommon;
using NUnit.Framework;
using static nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed.AnyStateTransitionDemo;

namespace nickmaltbie.StateMachineUnity.Tests.EditMode.Fixed
{
    public class PreemptEvent : IEvent
    {

    }

    public class AnyStateTransitionDemo : FixedSM
    {
        public ConcurrentDictionary<Type, int> EntryCount { get; private set; } = new ConcurrentDictionary<Type, int>();
        public ConcurrentDictionary<Type, int> ExitCount { get; private set; } = new ConcurrentDictionary<Type, int>();
        public ConcurrentDictionary<Type, int> StateCounter { get; private set; } = new ConcurrentDictionary<Type, int>();
        public int counter = 0;

        [InitialState]
        [Transition(typeof(DEvent1), typeof(StateD))]
        public class StateA : State { }

        [OnEventDoAction(typeof(TestEvent), nameof(IncrementStateCounter))]
        public class StateB : State { }

        [TransitionFromAnyState(typeof(CEvent))]
        [OnEventDoAction(typeof(TestEvent), nameof(IncrementStateCounter))]
        public class StateC : State { }

        [TransitionFrom(typeof(DEvent2), typeof(StateA))]
        public class StateD : State { }

        [Transition(typeof(PreemptEvent), typeof(StateA))]
        public class PreemptState : State { }

        [Transition(typeof(AEvent), typeof(StateA))]
        [Transition(typeof(BEvent), typeof(StateB))]
        [Transition(typeof(PreemptEvent), typeof(PreemptState))]
        [OnEventDoAction(typeof(TestEvent), nameof(IncrementCounter))]
        [OnEnterState(nameof(OnEnterState))]
        [OnExitState(nameof(OnExitState))]
        public class Any : AnyState { }

        public void IncrementCounter()
        {
            counter++;
        }

        public void IncrementStateCounter()
        {
            StateCounter.AddOrUpdate(CurrentState, 1, (_, v) => v + 1);
        }

        public void OnEnterState()
        {
            EntryCount.AddOrUpdate(CurrentState, 1, (_, v) => v + 1);
        }

        public void OnExitState()
        {
            ExitCount.AddOrUpdate(CurrentState, 1, (_, v) => v + 1);
        }
    }

    [TestFixture]
    public class StateMachineAnyStateTests : TestBase
    {
        private AnyStateTransitionDemo demo;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            demo = new AnyStateTransitionDemo();
        }

        [Test]
        public void VerifyAnyStateTransitions()
        {
            StateMachineTestUtils.SendAndVerifyEventSequence(
                demo,
                (new DEvent1(), typeof(StateD)),
                (new AEvent(), typeof(StateA)),
                (new BEvent(), typeof(StateB)),
                (new CEvent(), typeof(StateC)),
                (new DEvent1(), typeof(StateC)),
                (new AEvent(), typeof(StateA)),
                (new DEvent1(), typeof(StateD)),
                (new DEvent2(), typeof(StateD)),
                (new BEvent(), typeof(StateB)),
                (new DEvent2(), typeof(StateB)));
        }

        [Test]
        public void VerifyAnyStateTransitionCounts()
        {
            StateMachineTestUtils.SendAndVerifyEventSequence(demo, (new AEvent(), typeof(StateA)));
            Assert.IsTrue(demo.EntryCount.TryGetValue(typeof(StateA), out int aEntryCount));
            Assert.IsTrue(demo.ExitCount.TryGetValue(typeof(StateA), out int aExitCount));
            Assert.AreEqual(2, aEntryCount);
            Assert.AreEqual(1, aExitCount);

            StateMachineTestUtils.SendAndVerifyEventSequence(demo, (new BEvent(), typeof(StateB)));
            Assert.IsTrue(demo.EntryCount.TryGetValue(typeof(StateA), out aEntryCount));
            Assert.IsTrue(demo.ExitCount.TryGetValue(typeof(StateA), out aExitCount));
            Assert.IsTrue(demo.EntryCount.TryGetValue(typeof(StateB), out int bEntryCount));
            Assert.IsFalse(demo.ExitCount.TryGetValue(typeof(StateB), out int _));
            Assert.AreEqual(2, aEntryCount);
            Assert.AreEqual(2, aExitCount);
            Assert.AreEqual(1, bEntryCount);
        }

        [Test]
        public void VerifyAnyStatePreemptEvents()
        {
            StateMachineTestUtils.SendAndVerifyEventSequence(demo,
                (new AEvent(), typeof(StateA)),
                (new PreemptEvent(), typeof(PreemptState)),
                (new PreemptEvent(), typeof(StateA)),
                (new PreemptEvent(), typeof(PreemptState)),
                (new PreemptEvent(), typeof(StateA)),
                (new PreemptEvent(), typeof(PreemptState)),
                (new PreemptEvent(), typeof(StateA)),
                (new PreemptEvent(), typeof(PreemptState)),
                (new PreemptEvent(), typeof(StateA)),
                (new PreemptEvent(), typeof(PreemptState)),
                (new PreemptEvent(), typeof(StateA)));
        }

        [Test]
        public void VerifyInvalidTransitionThrowsException()
        {
            ITransition<Type> invalidTransitionTo = new TransitionAttribute(typeof(AEvent), typeof(AnyState));
            ITransition<Type> invalidTransitionFrom = new TransitionFromAttribute(typeof(AEvent), typeof(StateA));
            var lookup = new Dictionary<Tuple<Type, Type>, ITransition<Type>>();

            Assert.Throws<InvalidOperationException>(() => FSMUtils.SetupTransition(typeof(StateA), invalidTransitionTo, lookup));
            Assert.Throws<InvalidOperationException>(() => FSMUtils.SetupTransition(typeof(AnyState), invalidTransitionFrom, lookup));
        }

        [Test]
        public void VerifyAnyStateActions()
        {
            demo.RaiseEvent(new TestEvent());
            Assert.AreEqual(1, demo.counter);
            Assert.IsFalse(demo.StateCounter.TryGetValue(typeof(StateA), out int aCount));
            Assert.IsFalse(demo.StateCounter.TryGetValue(typeof(StateC), out int cCount));
            Assert.AreEqual(0, cCount);
            Assert.AreEqual(0, aCount);

            StateMachineTestUtils.SendAndVerifyEventSequence(demo, (new CEvent(), typeof(StateC)));
            demo.RaiseEvent(new TestEvent());
            Assert.AreEqual(2, demo.counter);
            Assert.IsTrue(demo.StateCounter.TryGetValue(typeof(StateC), out cCount));
            Assert.AreEqual(1, cCount);

            demo.RaiseEvent(new TestEvent());
            Assert.AreEqual(3, demo.counter);
            Assert.IsTrue(demo.StateCounter.TryGetValue(typeof(StateC), out cCount));
            Assert.AreEqual(2, cCount);

            StateMachineTestUtils.SendAndVerifyEventSequence(demo, (new AEvent(), typeof(StateA)));
            demo.RaiseEvent(new TestEvent());
            Assert.AreEqual(4, demo.counter);
            Assert.IsTrue(demo.StateCounter.TryGetValue(typeof(StateC), out cCount));
            Assert.AreEqual(2, cCount);
        }
    }
}
