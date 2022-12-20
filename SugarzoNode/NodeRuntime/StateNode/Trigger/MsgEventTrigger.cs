using UnityEngine;

namespace SugarFrame.Node
{
    public class MsgEventTrigger : BaseTrigger
    {
        public string msg;

        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            EventManager.StartListening(msg, Execute);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            EventManager.StopListening(msg, Execute);
        }
    }

}