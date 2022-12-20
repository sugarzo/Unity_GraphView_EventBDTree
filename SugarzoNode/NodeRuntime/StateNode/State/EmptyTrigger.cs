using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SugarFrame.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SugarFrame.Node
{
    public class EmptyTrigger : MonoBehaviour
    {
    #if UNITY_EDITOR
        [ValueDropdown(nameof(GetTriggerList))]
        public string baseTrigger;

        private List<string> GetTriggerList()
        {
            var q = typeof(BaseTrigger).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(BaseTrigger).IsAssignableFrom(x));

            var rlist = new List<string>();
            foreach(var qitem in q)
            {
                rlist.Add(qitem.Name);
            }

            return rlist;
        }
        private Type GetComponentByClassName(string className)
        {
            var q = typeof(BaseTrigger).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(BaseTrigger).IsAssignableFrom(x));

            foreach (var qitem in q)
            {
                if (qitem.Name == className)
                    return qitem;
            }
            return null;
        }


        [Button]
        public void Create()
        {
            if(!baseTrigger.IsNullOrWhitespace())
            {
                //创建对应的Trigger
                var cmp = GetComponentByClassName(baseTrigger);
                gameObject.AddComponent(cmp);
                gameObject.name = cmp.Name;
                DestroyImmediate(this);
            }
        }

        //[UnityEditor.MenuItem("GameObject/EmptyTrigger", false, priority = 0)]
        private static void CreateTriggerInScene()
        {
            var gameObject = new GameObject(typeof(EmptyTrigger).Name);

            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create Trigger");

            gameObject.AddComponent<EmptyTrigger>();

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;
            }
            Selection.activeGameObject = gameObject;

        }
    #endif
    }
}