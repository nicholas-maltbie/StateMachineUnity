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
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.StateMachineUnity.netcode.Example
{
    /// <summary>
    /// Example label for a ExampleSMLabel.
    /// </summary>
    public class NetworkSMLabel : MonoBehaviour
    {
        public void Update()
        {
            ExampleNetworkSM[] players = GameObject.FindObjectsOfType<ExampleNetworkSM>();
            var targetText = new StringBuilder();

            foreach (ExampleNetworkSM player in players
                .OrderBy(players => players.GetComponent<NetworkObject>().OwnerClientId))
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();

                string currentState = player?.CurrentState.ToString() ?? "null";
                if (currentState.Contains("+"))
                {
                    currentState = currentState.Substring(currentState.LastIndexOf("+") + 1);
                }

                targetText.AppendLine($"Player: {networkObject.OwnerClientId}\n  CurrentState: {currentState}");
            }

            if (players.Length == 0)
            {
                targetText.AppendLine($"No Players Connected.");
            }

            TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();
            label.text = targetText.ToString();
        }
    }
}
