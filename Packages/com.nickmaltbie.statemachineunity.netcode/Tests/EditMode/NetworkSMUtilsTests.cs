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

using nickmaltbie.StateMachineUnity.netcode.Utils;
using nickmaltbie.StateMachineUnity;
using nickmaltbie.StateMachineUnity.Attributes;
using nickmaltbie.TestUtilsUnity.Tests.TestCommon;
using NUnit.Framework;

namespace nickmaltbie.StateMachineUnity.netcode.Tests.EditMode
{
    /// <summary>
    /// Tests for the NetworkSMUtils.
    /// </summary>
    public class NetworkSMUtilsTests : TestBase
    {
        public class ExampleNetworkSM : NetworkSMBehaviour
        {
            [InitialState]
            public class State0 : State { }
            public class State1 : State { }
            public class State2 : State { }
        }

        /// <summary>
        /// Simple test for the network SM utils
        /// </summary>
        [Test]
        public void Validate_NetworkSMUtils()
        {
            NetworkSMUtils.SetupNetworkCache(typeof(ExampleNetworkSM));
            Assert.AreEqual(0, NetworkSMUtils.LookupStateIndex(typeof(ExampleNetworkSM), typeof(ExampleNetworkSM.State0)));
            Assert.AreEqual(1, NetworkSMUtils.LookupStateIndex(typeof(ExampleNetworkSM), typeof(ExampleNetworkSM.State1)));
            Assert.AreEqual(2, NetworkSMUtils.LookupStateIndex(typeof(ExampleNetworkSM), typeof(ExampleNetworkSM.State2)));
            Assert.AreEqual(-1, NetworkSMUtils.LookupStateIndex(typeof(ExampleNetworkSM), typeof(UninitializedState)));
            Assert.AreEqual(typeof(ExampleNetworkSM.State0), NetworkSMUtils.LookupIndexState(typeof(ExampleNetworkSM), 0));
            Assert.AreEqual(typeof(ExampleNetworkSM.State1), NetworkSMUtils.LookupIndexState(typeof(ExampleNetworkSM), 1));
            Assert.AreEqual(typeof(ExampleNetworkSM.State2), NetworkSMUtils.LookupIndexState(typeof(ExampleNetworkSM), 2));
            Assert.AreEqual(typeof(UninitializedState), NetworkSMUtils.LookupIndexState(typeof(ExampleNetworkSM), -1));
        }
    }
}
