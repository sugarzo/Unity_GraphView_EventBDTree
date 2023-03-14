using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SugarFrame.Node
{
    [DisallowMultipleComponent]
    public class FlowChart:MonoBehaviour
    {
#if UNITY_EDITOR
        [LabelText("隐藏所有节点组件"), OnValueChanged(nameof(HideCompoment))]
        public bool hideNode = false;

        private void HideCompoment()
        {
            GetComponents<MonoState>().ToList().ForEach(x =>
            x.hideFlags = hideNode ?
            HideFlags.HideInInspector : HideFlags.None
            );
        }

        [TextArea]
        public string note;

        [UnityEditor.MenuItem("GameObject/FlowChart", false, priority = 0)]
        public static GameObject CreateFlowChartInScene()
        {
            var gameObject = new GameObject(typeof(FlowChart).Name);

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;
            }
            //当前处于预制件模式
            else if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage prefabStage)
            {
                gameObject.transform.parent = prefabStage.prefabContentsRoot.transform;
                EditorUtility.SetDirty(prefabStage.prefabContentsRoot);
            }
            
            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "New FlowChart");
            gameObject.AddComponent<FlowChart>();

            return Selection.activeGameObject = gameObject;
        }
#endif
    }

}
