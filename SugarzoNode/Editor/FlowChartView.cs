using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static EditorDelayCall;

namespace SugarFrame.Node
{
    public class FlowChartView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<FlowChartView, GraphView.UxmlTraits> { }

        public Action<BaseNodeView> OnNodeSelected;
        public GameObject userSeletionGo;

        public FlowChartEditorWindow window;

        public FlowChartView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SugarzoNode/Editor/UIBuilder/FlowChart.uss");
            styleSheets.Add(styleSheet);

            userSeletionGo = userSeletionGo == null ? FlowChartEditorWindow.userSeletionGo : userSeletionGo;

            //��GraphView�仯ʱ�����÷���
            graphViewChanged += OnGraphViewChanged;

            //�½������˵�
            var menuWindowProvider = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            };
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {

            if (graphViewChange.elementsToRemove != null)
            {
                //����ÿ�����Ƴ��Ľڵ�
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    BaseNodeView BaseNodeView = elem as BaseNodeView;
                    if (BaseNodeView != null)
                    {
                        GameObject.DestroyImmediate(BaseNodeView.state);
                    }
                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        BaseNodeView parentView = edge.output.node as BaseNodeView;
                        BaseNodeView childView = edge.input.node as BaseNodeView;
                        //If��Branch�ڵ����ж�
                        if (edge.output.node is BranchNodeView view)
                        {
                            if (edge.input.portName == "true")
                            {
                                (parentView.state as BaseBranch).trueFlow = null;
                            }
                            if (edge.input.portName == "false")
                            {
                                (parentView.state as BaseBranch).falseFlow = null;
                            }
                        }
                        else if (edge.output.node is SequenceNodeView sqView)
                        {
                            (parentView.state as BaseSequence).nextflows.Remove(childView.state as MonoState);
                        }
                        else
                            parentView.state.nextFlow = null;
                    }
                });
            }
            //����ÿ���������ı�
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    BaseNodeView parentView = edge.output.node as BaseNodeView;
                    BaseNodeView childView = edge.input.node as BaseNodeView;
                    //If��Branch�ڵ����ж�
                    if (edge.output.node is BranchNodeView view)
                    {
                        if (edge.output.portName.Equals("true"))
                        {
                            (parentView.state as BaseBranch).trueFlow = childView.state as MonoState;
                        }
                        if (edge.output.portName.Equals("false"))
                        {
                            (parentView.state as BaseBranch).falseFlow = childView.state as MonoState;
                        }
                    }
                    else if (edge.output.node is SequenceNodeView sqView)
                    {
                        (parentView.state as BaseSequence).nextflows.Add(childView.state as MonoState);
                    }
                    else
                        parentView.state.nextFlow = childView.state as MonoState;
                });
            }
            //�����ڵ㣬��¼λ�õ�
            nodes.ForEach((n) =>
            {
                BaseNodeView view = n as BaseNodeView;
                if (view != null && view.state != null)
                {
                    view.state.nodePos = view.GetPosition().position;
                }
            });

            return graphViewChange;
        }

        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;

            //��ȡ���λ��
            var windowRoot = window.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - window.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
            CreateNode(type, graphMousePosition);

            return true;
        }
        private void CreateNode(Type type, Vector2 pos = default)
        {
            if (userSeletionGo == null)
                return;

            BaseNodeView nodeView = null;
            if (type.IsSubclassOf(typeof(BaseTrigger)))
                nodeView = new TriggerNodeView();
            if (type.IsSubclassOf(typeof(BaseAction)))
                nodeView = new ActionNodeView();
            if (type.IsSubclassOf(typeof(BaseSequence)))
                nodeView = new SequenceNodeView();
            if (type.IsSubclassOf(typeof(BaseBranch)))
                nodeView = new BranchNodeView();


            if (nodeView == null)
            {
                Debug.LogError("�ڵ�δ�ҵ���Ӧ���Ե�NodeView");
                return;
            }

            //���Component�������ڵ�
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.state = (NodeState)userSeletionGo.AddComponent(type);
            nodeView.SetPosition(new Rect(pos, nodeView.GetPosition().size));

            this.AddElement(nodeView);
        }

        //�ع�����
        public void ResetNodeView()
        {
            if (userSeletionGo != null)
            {
                Debug.Log("�����ڵ�ͼ");
                var list = userSeletionGo.GetComponents<NodeState>();
                foreach (var item in list)
                    CreateBaseNodeView(item);
            }
            if (userSeletionGo != null)
            {
                Debug.Log("�����ڵ�ߵĹ�ϵ");
                CreateNodeEdge();
            }
        }

        //��ԭ�ڵ����
        private void CreateBaseNodeView(NodeState nodeClone)
        {
            if (userSeletionGo == null || nodeClone == null)
                return;

            BaseNodeView nodeView = null;
            //�ж���Ҫ��ԭ�Ľڵ�
            if (nodeClone is BaseTrigger trigger)
                nodeView = new TriggerNodeView();
            if (nodeClone is BaseAction action)
                nodeView = new ActionNodeView();
            if (nodeClone is BaseSequence sequence)
                nodeView = new SequenceNodeView();
            if (nodeClone is BaseBranch branch)
                nodeView = new BranchNodeView();

            if (nodeView == null)
            {
                Debug.LogError("�ڵ�δ�ҵ���Ӧ���Ե�NodeView");
                return;
            }

            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.state = nodeClone;
            nodeView.SetPosition(new Rect(nodeClone.nodePos, nodeView.GetPosition().size));

            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();

            AddElement(nodeView);
        }

        //��ԭ�ڵ�ı�
        private void CreateNodeEdge()
        {
            if (userSeletionGo == null)
                return;

            //�����е���ͼ���ڽӱ�
            Dictionary<NodeState, BaseNodeView> map = new Dictionary<NodeState, BaseNodeView>();
            Dictionary<BaseNodeView, Port> inputPorts = new Dictionary<BaseNodeView, Port>();
            Dictionary<BaseNodeView, List<Port>> outputPorts = new Dictionary<BaseNodeView, List<Port>>();

            ports.ForEach(x =>
            {
                var y = x.node;
                var node = y as BaseNodeView;
                if (!map.ContainsKey(node.state))
                {
                    map.Add(node.state, node);
                }
                if (!inputPorts.ContainsKey(node))
                {
                    inputPorts.Add(node, x);
                }
                if (!outputPorts.ContainsKey(node))
                {
                    outputPorts.Add(node, new List<Port>());
                }
                if (x.direction == Direction.Output)
                    outputPorts[node].Add(x);
            });

            //ֻ������������Ľڵ�
            foreach (var node in map.Keys)
            {

                if (node is BaseSequence sequence)
                {
                    Port x = outputPorts[map[sequence]][0];
                    foreach (var nextflow in sequence.nextflows)
                    {
                        Port y = inputPorts[map[nextflow]];
                        AddEdgeByPorts(x, y);
                    }
                }
                else if (node is BaseBranch branch)
                {
                    var truePorts = outputPorts[map[branch]][0].portName == "true" ? outputPorts[map[branch]][0] : outputPorts[map[branch]][1];
                    var falsePorts = outputPorts[map[branch]][0].portName == "false" ? outputPorts[map[branch]][0] : outputPorts[map[branch]][1];

                    if (branch.trueFlow != null)
                        AddEdgeByPorts(truePorts, inputPorts[map[branch.trueFlow]]);
                    if (branch.falseFlow != null)
                        AddEdgeByPorts(falsePorts, inputPorts[map[branch.falseFlow]]);
                }
                else if (node is MonoState state)
                {
                    //��ͨ��Action����Trigger��ֻ����nextFlow�ͺ���
                    if (state.nextFlow != null)
                        AddEdgeByPorts(outputPorts[map[state]][0], inputPorts[map[state.nextFlow]]);
                }

            }
        }


        //�ж�ÿ�����Ƿ��������
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }
        //����������
        private void AddEdgeByPorts(Port _outputPort, Port _inputPort)
        {
            if (_outputPort.node == _inputPort.node)
                return;

            Edge tempEdge = new Edge()
            {
                output = _outputPort,
                input = _inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            Add(tempEdge);
        }

        protected BoolClass isDuplicate = new BoolClass();

        public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);

            if (evt is ValidateCommandEvent commandEvent)
            {
                Debug.Log("Event:");
                Debug.Log(commandEvent.commandName);
                //����һ��0.2sִ��һ��  ��Ȼ��ʱ�����ִ��
                if (commandEvent.commandName.Equals("Paste"))
                {
                    new EditorDelayCall().CheckBoolCall(0.2f, isDuplicate,
                        OnDuplicate);
                }
            }
        }
        /// <summary>
        /// ����ʱ
        /// </summary>
        protected void OnDuplicate()
        {
            Debug.Log("���ƽڵ�");
            //���ƽڵ�
            var nodesDict = new Dictionary<BaseNodeView, BaseNodeView>(); //�¾�Node����

            foreach (var selectable in selection)
            {
                var offset = 1;
                if (selectable is BaseNodeView baseNodeView)
                {
                    offset++;
                    UnityEditorInternal.ComponentUtility.CopyComponent(baseNodeView.state);

                    BaseNodeView nodeView = null;
                    var nodeClone = baseNodeView.state;
                    //�ж���Ҫ��ԭ�Ľڵ�
                    if (nodeClone is BaseTrigger trigger)
                        nodeView = new TriggerNodeView();
                    if (nodeClone is BaseAction action)
                        nodeView = new ActionNodeView();
                    if (nodeClone is BaseSequence sequence)
                        nodeView = new SequenceNodeView();
                    if (nodeClone is BaseBranch branch)
                        nodeView = new BranchNodeView();

                    if (nodeView == null)
                        return;

                    //�¾ɽڵ�ӳ��
                    if (nodeView != null)
                    {
                        nodesDict.Add(baseNodeView, nodeView);
                    }

                    nodeView.OnNodeSelected = OnNodeSelected;
                    AddElement(nodeView);
                    nodeView.state = (NodeState)userSeletionGo.AddComponent(baseNodeView.state.GetType());
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(nodeView.state);

                    //����һ������
                    //����ԭ���������㷨����д������ȫ�����ó�null��
                    nodeView.state.nextFlow = null ;
                    if(nodeView.state is BaseSequence sq)
                    {
                        sq.nextflows = new List<MonoState>();
                    }
                    if (nodeView.state is BaseBranch br)
                    {
                        br.trueFlow = null;
                        br.falseFlow = null;
                    }

                    //���Ƴ����Ľڵ�λ��ƫ��
                    nodeView.SetPosition(new Rect(baseNodeView.GetPosition().position + (Vector2.one * 30 * offset),nodeView.GetPosition().size));
                }
            }

            for (int i = selection.Count - 1; i >= 0; i--)
            {
                //ȡ��ѡ��
                this.RemoveFromSelection(selection[i]);
            }

            foreach (var node in nodesDict.Values)
            {
                //ѡ�������ɵĽڵ�
                this.AddToSelection(node);
            }
        }
    }
}
