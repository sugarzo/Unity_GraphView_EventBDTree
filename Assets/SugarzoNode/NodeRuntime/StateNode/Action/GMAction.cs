using UnityEngine;

namespace SugarFrame.Node
{
    public class GMAction : BaseAction
    {
        [Header("GMAction")]
        public bool quitGame = true;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            Application.Quit();

            RunOver(emitTrigger);
        }
    }

}