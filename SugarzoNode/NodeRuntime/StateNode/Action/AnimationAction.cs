using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SugarFrame.Node
{
    public class AnimationAction : BaseAction
    {
        [Header("AnimatorAction")]
        public Animator animator;
        public bool waitUntilFinish = true;
        [ValueDropdown(nameof(Animations))] public AnimationClip animationClip;

        [ValueDropdown(nameof(Animations)), ShowIf("ignore")] public AnimationClip ignoreActionInState;

        public float timerDelta = 0f;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {

            animator.Play(animationClip.name);

            if (!waitUntilFinish)
            {
                RunOver(emitTrigger);
            }
            else
            {
                if (animationClip.isLooping)
                {
                    RunOver(emitTrigger);

                }
                else
                {
                    StartCoroutine(Delay(()=>RunOver(emitTrigger), animationClip.length + timerDelta));
                }
            }
        }

        List<AnimationClip> Animations()
        {
            if (animator != null)
            {
                return animator.runtimeAnimatorController.animationClips.ToList();
            }

            return null;
        }

        IEnumerator Delay(Action action,float timer)
        {
            yield return new WaitForSeconds(animationClip.length + timerDelta);
            action?.Invoke();
        }
    }

}