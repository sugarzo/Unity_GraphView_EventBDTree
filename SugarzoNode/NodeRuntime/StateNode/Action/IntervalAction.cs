using System;
using System.Collections;
using UnityEngine;

namespace SugarFrame.Node
{
    public class IntervalAction : BaseAction
    {
        [Header("等待x秒后执行下一个")]
        public float timer = 1f;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            StartCoroutine(WaitTime(()=>RunOver(emitTrigger)));
        }

        IEnumerator WaitTime(Action _event)
        {
            if(timer <= 0)
            {
                _event?.Invoke();
                yield break;
            }
            yield return new WaitForSeconds(timer);
            _event?.Invoke();
        }
    }

}