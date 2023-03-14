using System;
using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public class GameObjectAction : BaseAction
    {
        [Header("GameObjectAction")]
        public List<ActiveGo> activeGoes;

        public override void RunningLogic(BaseTrigger emitTrigger)
        {
            

            foreach (var activeGO in activeGoes)
            {
                if (activeGO.go != null)
                {
                    activeGO.go.SetActive(activeGO.isActive);
                    if (activeGO.isDestroy)
                    {
                        GameObject.Destroy(activeGO.go);
                    }
                }
            }


            RunOver(emitTrigger);
        }


        [Serializable]
        public class ActiveGo
        {
            public GameObject go;
            public bool isActive;
            public bool isDestroy = false;
        }
    }
}
