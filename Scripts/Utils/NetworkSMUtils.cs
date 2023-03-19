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
using System.Linq;

namespace nickmaltbie.StateMachineUnity.netcode.Utils
{
    public class UninitializedState : State { }

    public static class NetworkSMUtils
    {
        public static ConcurrentDictionary<Type, Dictionary<Type, int>> SMStateIndexLookup =
            new ConcurrentDictionary<Type, Dictionary<Type, int>>();

        public static ConcurrentDictionary<Type, Dictionary<int, Type>> SMIndexStateLookup =
            new ConcurrentDictionary<Type, Dictionary<int, Type>>();

        public static int LookupStateIndex(Type stateMachine, Type state)
        {
            if (state == typeof(UninitializedState))
            {
                return -1;
            }

            return SMStateIndexLookup[stateMachine][state];
        }

        public static Type LookupIndexState(Type stateMachine, int index)
        {
            if (index == -1)
            {
                return typeof(UninitializedState);
            }

            return SMIndexStateLookup[stateMachine][index];
        }

        internal static void SetupStateLookupDictionary(Type stateMachine)
        {
            // Find all the supported states for the state machine.
            int index = 0;

            foreach (Type state in stateMachine.GetNestedTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(State)))
                .OrderBy(type => type.FullName))
            {
                SMStateIndexLookup[stateMachine][state] = index;
                SMIndexStateLookup[stateMachine][index] = state;
                index++;
            }
        }

        public static void SetupNetworkCache(Type stateMachine)
        {
            if (!SMStateIndexLookup.ContainsKey(stateMachine) || !SMIndexStateLookup.ContainsKey(stateMachine))
            {
                SMStateIndexLookup[stateMachine] = new Dictionary<Type, int>();
                SMIndexStateLookup[stateMachine] = new Dictionary<int, Type>();
                SetupStateLookupDictionary(stateMachine);
            }
        }
    }
}
