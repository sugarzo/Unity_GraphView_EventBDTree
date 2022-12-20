using UnityEngine;

namespace SugarFrame.Node
{
    public class MsgEventAction : BaseAction
    {
        [Header("MsgEventAction")]
        public string msg;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            EventManager.EmitEvent(msg, this);

            RunOver(emitTrigger);
        }
    }

}