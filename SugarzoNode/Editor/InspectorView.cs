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
        OdinEditor odinEditor;

        public InspectorView()
        {
        }

        internal void UpdateSelection(BaseNodeView nodeView)
        {
            Clear();
            Debug.Log("显示节点的Inspector面板");
            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.state);
            //odinEditor = (OdinEditor)editor;
            
            IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            Add(container);
        }
        //UPdatesele

    }
}

