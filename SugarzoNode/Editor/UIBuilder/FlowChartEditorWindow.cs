using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarFrame.Node
{
    //�ڱ༭�������FlowChart��Inspector
    [CustomEditor(typeof(FlowChart))]
    public class FlowChartEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //���ز���Ҫ��ʾ�����
            if (GUILayout.Button("Open"))
            {
                FlowChartEditorWindow.userSeletionGo = (target as MonoBehaviour)?.gameObject;
                FlowChartEditorWindow.OpenWindow();
            }
        }
    }

    public class FlowChartEditorWindow : EditorWindow
    {
        //[MenuItem("FlowChart/FlowChart")]
        public static void OpenWindow()
        {
            FlowChartEditorWindow wnd = GetWindow<FlowChartEditorWindow>();
            wnd.titleContent = new GUIContent("FlowChart");
        }

        /// <summary>
        /// ��ǰѡ�����Ϸ��Ʒ
        /// </summary>
        public static GameObject userSeletionGo;

        FlowChartView flowChartView;
        InspectorView inspectorView;

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SugarzoNode/Editor/UIBuilder/FlowChart.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SugarzoNode/Editor/UIBuilder/FlowChart.uss");
            root.styleSheets.Add(styleSheet);

            //���ýڵ���ͼ��Inspector��ͼ
            flowChartView = root.Q<FlowChartView>();
            inspectorView = root.Q<InspectorView>();

            flowChartView.OnNodeSelected = OnNodeSelectionChanged;
            flowChartView.userSeletionGo = userSeletionGo;
            flowChartView.window = this;

            //����ڵ�
            flowChartView.ResetNodeView();
        }

        void OnNodeSelectionChanged(BaseNodeView nodeView)
        {
            Debug.Log("Editor�ܵ��ڵ㱻ѡ����Ϣ");
            inspectorView.UpdateSelection(nodeView);
        }
    }
}