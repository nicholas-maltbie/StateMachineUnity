

using System;
using System.Linq;
using UnityEngine;

namespace nickmaltbie.StateMachineUnity.Utils
{
    /// <summary>
    /// Class with utility functions for state machine.
    /// </summary>
    public class AnimationCompleteListener : MonoBehaviour
    {
        public EventHandler<string> OnAnimationCompleted;

        public void Awake()
        {
            Animator anim = GetComponent<Animator>();
            foreach (var animClip in anim.runtimeAnimatorController.animationClips)
            {
                // Avoid adding duplicate events.
                if (animClip.events.Any(evt =>
                {
                    return evt.time == animClip.length &&
                        evt.functionName == "_OnAnimationCompleteInternal" &&
                        evt.stringParameter == animClip.name;
                }))
                {
                    continue;
                }

                animClip.AddEvent(new AnimationEvent
                {
                    time = animClip.length,
                    functionName = "_OnAnimationCompleteInternal",
                    stringParameter = animClip.name,
                });
            }
        }

        protected void _OnAnimationCompleteInternal(string clipName)
        {
            OnAnimationCompleted?.Invoke(this, clipName);
        }
    }
}
