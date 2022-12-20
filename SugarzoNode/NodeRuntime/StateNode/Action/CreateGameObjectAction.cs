using UnityEngine;

namespace SugarFrame.Node
{
    public class CreateGameObjectAction : BaseAction
    {
        [Header("CreateGameObjectAction")]
        public GameObject protype;
        public Transform startPos;
        public Transform parent;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            GameObject.Instantiate(protype, startPos, parent);

            RunOver(emitTrigger);
        }
    }

}