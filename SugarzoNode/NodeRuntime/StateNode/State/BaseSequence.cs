using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public abstract class BaseSequence : BaseAction
    {

        [HideInInspector]
        public List<MonoState> nextflows = new List<MonoState>();
        [Header("每个行为之间是否等待x秒,输入-1时等待1帧")]
        public float waitTimeEachAction = 0;
        [ReadOnly]
        public int runningAction = 0;

        public override void OnEnter()
        {           
            base.OnEnter();
        }


        /// <summary>
        /// 向下执行所有节点
        /// </summary>
        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            if (nextflows != null && nextflows.Count > 0)
            {
                runningAction = nextflows.Count;
                StartCoroutine(StartActions(emitTrigger));
            }
            else
            {
                //Sequence节点输出为空，直接切换到结束状态
                RunOver(emitTrigger);
            }
        }

        private IEnumerator StartActions(BaseTrigger emitTrigger)
        {
            DataCache cache = new DataCache();
            cache.count = nextflows.Count;
            cache.trigger = emitTrigger;

            //继续所有节点
            foreach (var nextFlow in nextflows)
            {
                //依赖注入,当所有Action执行完成时回调Trigger
                if (nextFlow is BaseAction action)
                    action.OnExitEvent += delegate ()
                    {
                        cache.count--;
                        if(cache.count == 0)
                        {
                            cache.trigger?.OnExit();
                        }
                    };

                if (nextFlow is BaseAction nextAction)
                    nextAction.Execute();
                else
                    nextFlow.Execute();

                if (waitTimeEachAction > 0)
                    yield return new WaitForSeconds(waitTimeEachAction);
                if (waitTimeEachAction == -1)
                    yield return null;
            }
            yield return null;
        }


        private class DataCache
        {
            public BaseTrigger trigger;
            public int count;
        }
    }
}


