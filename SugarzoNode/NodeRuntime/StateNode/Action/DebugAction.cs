using UnityEngine;

namespace SugarFrame.Node
{
    public class DebugAction : BaseAction
    {
        [Header("Debug Action")]
        public string content;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            Debug.Log(content);

            RunOver(emitTrigger);
        }
    }

}