using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SugarFrame.Node
{
    //在编辑器里绘制FlowChart的Inspector
    [CustomEditor(typeof(FlowChart))]
    public class FlowChartEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //隐藏不需要显示的组件
            if (GUILayout.Button("Open"))
            {
                FlowChartEditorWindow.userSeletionGo = (target as MonoBehaviour)?.gameObject;
                FlowChartEditorWindow.OpenWindow();
            }
        }
    }

    public class FlowChartEditorWindow : EditorWindow
    {
        [MenuItem("FlowChart/FlowChart")]
        public static void OpenWindow()
        {
            FlowChartEditorWindow wnd = GetWindow<FlowChartEditorWindow>();
            wnd.titleContent = new GUIContent("FlowChart");
        }

        /// <summary>
        /// 当前选择的游戏物品
        /// </summary>
        private static GameObject _userSeletionGo;
        public static GameObject userSeletionGo
        {
            get
            {
                return _userSeletionGo;
            }
            set
            {
                _userSeletionGo = value;
                SeletionChange?.Invoke();
            }
        }

        public static Action SeletionChange = null;

        FlowChartView flowChartView;
        InspectorView inspectorView;

        TextField gameObjectTextField;

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

            //设置节点视图和Inspector视图
            flowChartView = root.Q<FlowChartView>();
            inspectorView = root.Q<InspectorView>();

            flowChartView.OnNodeSelected = OnNodeSelectionChanged;
            flowChartView.userSeletionGo = userSeletionGo;
            flowChartView.window = this;

            //构造节点
            flowChartView.ResetNodeView();

            //获取属性
            gameObjectTextField = root.Q<TextField>();

            //设置选择的ObjectField
            var objectField = root.Q<ObjectField>();
            objectField.objectType = typeof(FlowChart);
            FlowChart flowChart = null;
            if (userSeletionGo != null) 
                objectField.value = userSeletionGo.TryGetComponent(out flowChart) ? flowChart :null;

            if(objectField.value != null)
            {
                gameObjectTextField.SetEnabled(true);
                gameObjectTextField.value = objectField.value.name;
            }
            else
            {
                gameObjectTextField.SetEnabled(false);
                gameObjectTextField.value = "";
            }

            objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>((evt) =>
            {
                if (objectField.value != null)
                {
                    gameObjectTextField.SetEnabled(true);
                    gameObjectTextField.value = objectField.value.name;
                }
                else
                {
                    gameObjectTextField.SetEnabled(false);
                    gameObjectTextField.value = "";
                }

                userSeletionGo = (evt.newValue as FlowChart)?.gameObject;
                flowChartView.userSeletionGo = userSeletionGo;
                flowChartView.window = this;

                //重新选择Selection
                if (objectField.value != null)
                {
                    Selection.activeGameObject = (evt.newValue as FlowChart)?.gameObject;
                }
                //构造节点
                flowChartView.ResetNodeView();
            });

            SeletionChange = delegate ()
            {
                flowChartView.userSeletionGo = userSeletionGo;
                //objectField.value = userSeletionGo;
                //构造节点
                flowChartView.ResetNodeView();
            };

            //新建FlowChart按钮
            var button = root.Q<Button>("new");
            button.clicked += delegate ()
            {
                objectField.value = FlowChart.CreateFlowChartInScene()?.GetComponent<FlowChart>();
            };

            //新建刷新按钮
            var buttonR = root.Q<Button>("refresh");
            buttonR.clicked += delegate ()
            {
                flowChartView.ResetNodeView();
            };

            //关联GO名称
            gameObjectTextField.RegisterValueChangedCallback(evt =>
            {
                if(userSeletionGo != null)
                {
                    userSeletionGo.name = evt.newValue;
                }
            });
        }

        void OnNodeSelectionChanged(BaseNodeView nodeView)
        {
            //Debug.Log("Editor受到节点被选中信息");
            inspectorView.UpdateSelection(nodeView);
        }
    }
}