using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
namespace SugarFrame.Node
{
    public class TriggerEventAction : BaseAction
    {
        [Header("TriggerEventAction")]
        public BaseTrigger baseTrigger;
        [LabelText("ִ���¼�")]
        public bool executeEvent = false;
        [LabelText("ע���¼�")]
        public bool registerSaveTypeEvent = false;
        [LabelText("ע���¼�")]
        public bool deleteSaveTypeEvent = false;
        
        public override void RunningLogic(BaseTrigger emitTrigger) {

            if (executeEvent)
                baseTrigger.Execute();

            if (registerSaveTypeEvent)
                baseTrigger.RegisterSaveTypeEvent();

            if (deleteSaveTypeEvent)
                baseTrigger.DeleteSaveTypeEvent();


            RunOver(emitTrigger);
        }
    }
}
