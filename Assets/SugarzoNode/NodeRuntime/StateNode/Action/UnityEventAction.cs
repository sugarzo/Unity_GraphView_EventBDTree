using UnityEngine;
using UnityEngine.Events;

namespace SugarFrame.Node
{
    public class UnityEventAction : BaseAction
    {
        [Header("UnityEventAction Action")]
        public UnityEvent unityEvent;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            unityEvent?.Invoke();

            RunOver(emitTrigger);
        }
    }

}