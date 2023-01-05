using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarFrame.Node
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        Editor editor;

        public InspectorView()
        {
        }

        internal void UpdateSelection(BaseNodeView nodeView)
        {
            Clear();
            Debug.Log("显示节点的Inspector面板");
            UnityEngine.Object.DestroyImmediate(editor);

            if (nodeView == null)
                return;

            editor = Editor.CreateEditor(nodeView.state);

            IMGUIContainer container = new IMGUIContainer(() => {
                if (nodeView != null && nodeView.state != null)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}

