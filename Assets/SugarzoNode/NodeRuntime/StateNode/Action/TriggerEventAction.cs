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
        [LabelText("执行事件")]
        public bool executeEvent = false;
        [LabelText("注册事件")]
        public bool registerSaveTypeEvent = false;
        [LabelText("注销事件")]
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
