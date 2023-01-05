using UnityEditor;
using UnityEngine;

namespace SugarFrame.Node
{
    [DisallowMultipleComponent]
    public class FlowChart:MonoBehaviour
    {

#if UNITY_EDITOR
        [TextArea]
        public string note;

        [UnityEditor.MenuItem("GameObject/FlowChart", false, priority = 0)]
        public static GameObject CreateFlowChartInScene()
        {
            var gameObject = new GameObject(typeof(FlowChart).Name);

            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "New FlowChart");

            gameObject.AddComponent<FlowChart>();

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;
            }
            return Selection.activeGameObject = gameObject;
        }
#endif
    }

}
