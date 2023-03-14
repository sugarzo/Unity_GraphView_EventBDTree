using Sirenix.OdinInspector.Editor;
using SugarFrame.Node;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(BaseAction),true)]
//public class CustomEditorNodeAction : Editor
//{
//    public override VisualElement CreateInspectorGUI()
//    {
//        //return base.CreateInspectorGUI();
//        var root = new VisualElement();
//        var foldout = new Foldout() { viewDataKey = "节点详细" };
//        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
//        return root;
//    }
//}
//[CustomEditor(typeof(BaseTrigger), true)]
//public class CustomEditorNodeTrigger : OdinEditor
//{
//    public override VisualElement CreateInspectorGUI()
//    {
//        //return base.CreateInspectorGUI();
//        var root = new VisualElement();
//        var foldout = new Foldout() { viewDataKey = "节点详细" };
//        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
//        return root;
//    }
//}
