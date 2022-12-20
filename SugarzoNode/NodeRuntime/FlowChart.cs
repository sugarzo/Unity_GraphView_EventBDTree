using UnityEditor;
using UnityEngine;

namespace SugarFrame.Node
{
    public class FlowChart:MonoBehaviour
    {

#if UNITY_EDITOR
        [TextArea]
        public string note;

        [UnityEditor.MenuItem("GameObject/FlowChart", false, priority = 0)]
        private static void CreateFlowChartInScene()
        {
            var gameObject = new GameObject(typeof(FlowChart).Name);

            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create Trigger");

            gameObject.AddComponent<FlowChart>();

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;

            }
            Selection.activeGameObject = gameObject;
        }
#endif
    }

}
