using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class EditorWindow_NodeCreator : OdinMenuEditorWindow
{
    [MenuItem("Sugarzo/FlowChart≈‰÷√")]
    private static void OpenWindow()
    {
        var window = GetWindow<EditorWindow_NodeCreator>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(720, 720);
        window.titleContent = new GUIContent("FlowChart…Ë÷√");
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.AddAllAssetsAtPath("FlowChart…Ë÷√","Assets/SugarzoNode/Editor/EditorWindow", typeof(ScriptableObject), true);
        return tree;
    }
}
