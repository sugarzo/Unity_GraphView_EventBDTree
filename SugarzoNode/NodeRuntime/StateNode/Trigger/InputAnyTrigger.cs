using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public class InputAnyTrigger : BaseTrigger
    {
        public bool allKey = false;
        //Called on Enable
        public override void RegisterSaveTypeEvent()
        {
            //EventManager.StartListening("",Execute);
        }

        //Called on DisEnable
        public override void DeleteSaveTypeEvent()
        {
            //EventManager.StopListening("",Execute);
        }

        public List<KeyCode> keys;

        private void Update()
        {
            foreach(var key in keys)
            {
                if (Input.GetKeyDown(key))
                    Execute();
            }
            if(allKey && Input.anyKey)
            {
                Execute();
            }
        }
    }

}