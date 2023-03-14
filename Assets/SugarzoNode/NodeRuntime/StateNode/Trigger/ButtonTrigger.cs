using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SugarFrame.Node
{
    public class ButtonTrigger : BaseTrigger
    {
        public List<Button> buttons;

        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            foreach (var btn in buttons)
                btn?.onClick.AddListener(Execute);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            foreach (var btn in buttons)
                btn?.onClick.RemoveListener(Execute);
        }
    }
}