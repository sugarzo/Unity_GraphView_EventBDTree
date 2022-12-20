using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public abstract class BaseBranch : BaseAction
    {
        //流向下一节点的流
        [HideInInspector]
        public MonoState trueFlow;
        [HideInInspector]
        public MonoState falseFlow;

        //在派生类中实现该逻辑
        public abstract bool IfResult();

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            RunOver(emitTrigger);
        }

        public override void RunOver(BaseTrigger emitTrigger)
        {
            //判断下一节点的流向
            nextFlow = IfResult() ? trueFlow : falseFlow;

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
            TransitionState(EState.Finish);
        }
    }
}


