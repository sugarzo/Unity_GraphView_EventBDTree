using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public abstract class BaseAction : MonoState
    {
        [Header("进入时等待一帧")]
        public bool wait1Frame = false;
        
        //在派生类中填写逻辑，并回调Runover()
        public abstract void RunningLogic(BaseTrigger emitTrigger);

        [Button]
        public override void Execute()
        {
            Execute(null);
        }

        public void Execute(BaseTrigger emitTrigger)
        {
            TransitionState(EState.Running);

            if (wait1Frame && gameObject.activeInHierarchy)
            {
                StartCoroutine(DelayFrame(RunningLogic, emitTrigger));
            }
            else
            {
                RunningLogic(emitTrigger);
            }  
        }

        public virtual void RunOver(BaseTrigger emitTrigger)
        {
            OnExitEvent?.Invoke();
            OnExitEvent = null;

            if (nextFlow)
            {
                //继续执行下一个节点
                if (nextFlow is BaseAction nextAction)
                    nextAction.Execute(emitTrigger);
                else
                    nextFlow.Execute();
            }
            else
            {
                //最后一个节点了，切换Trigger状态
                emitTrigger?.OnExit();
            }
            TransitionState(EState.Exit);
        }

        public override void OnRunning()
        {
            //不执行任何操作，由RunOver触发OnExit
        }

        [HideInInspector]
        public event Action OnExitEvent;

        IEnumerator DelayFrame(Action<BaseTrigger> action,BaseTrigger emitTrigger)
        {
            yield return null;
            action?.Invoke(emitTrigger);
        }
    }
}


