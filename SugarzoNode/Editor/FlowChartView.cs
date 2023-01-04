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

            //当GraphView变化时，调用方法
            graphViewChanged += OnGraphViewChanged;

            //新建搜索菜单
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
                //对于每个被移除的节点
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
                        //If和Branch节点特判定
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
            //对于每个被创建的边
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    BaseNodeView parentView = edge.output.node as BaseNodeView;
                    BaseNodeView childView = edge.input.node as BaseNodeView;
                    //If和Branch节点特判定
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
            //遍历节点，记录位置点
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

            //获取鼠标位置
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
                Debug.LogError("节点未找到对应属性的NodeView");
                return;
            }

            //添加Component，关联节点
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.state = (NodeState)userSeletionGo.AddComponent(type);
            nodeView.SetPosition(new Rect(pos, nodeView.GetPosition().size));

            this.AddElement(nodeView);
        }

        //重构布局
        public void ResetNodeView()
        {
            if (userSeletionGo != null)
            {
                Debug.Log("构建节点图");
                var list = userSeletionGo.GetComponents<NodeState>();
                foreach (var item in list)
                    CreateBaseNodeView(item);
            }
            if (userSeletionGo != null)
            {
                Debug.Log("构建节点边的关系");
                CreateNodeEdge();
            }
        }

        //复原节点操作
        private void CreateBaseNodeView(NodeState nodeClone)
        {
            if (userSeletionGo == null || nodeClone == null)
                return;

            BaseNodeView nodeView = null;
            //判断需要复原的节点
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
                Debug.LogError("节点未找到对应属性的NodeView");
                return;
            }

            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.state = nodeClone;
            nodeView.SetPosition(new Rect(nodeClone.nodePos, nodeView.GetPosition().size));

            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();

            AddElement(nodeView);
        }

        //复原节点的边
        private void CreateNodeEdge()
        {
            if (userSeletionGo == null)
                return;

            //这里有点像图的邻接表
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

            //只负责连接下面的节点
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
                    //普通的Action或者Trigger，只处理nextFlow就好了
                    if (state.nextFlow != null)
                        AddEdgeByPorts(outputPorts[map[state]][0], inputPorts[map[state.nextFlow]]);
                }

            }
        }


        //判断每个点是否可以相连
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }
        //连接两个点
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
                //限制一下0.2s执行一次  不然短时间会多次执行
                if (commandEvent.commandName.Equals("Paste"))
                {
                    new EditorDelayCall().CheckBoolCall(0.2f, isDuplicate,
                        OnDuplicate);
                }
            }
        }
        /// <summary>
        /// 复制时
        /// </summary>
        protected void OnDuplicate()
        {
            Debug.Log("复制节点");
            //复制节点
            var nodesDict = new Dictionary<BaseNodeView, BaseNodeView>(); //新旧Node对照

            foreach (var selectable in selection)
            {
                var offset = 1;
                if (selectable is BaseNodeView baseNodeView)
                {
                    offset++;
                    UnityEditorInternal.ComponentUtility.CopyComponent(baseNodeView.state);

                    BaseNodeView nodeView = null;
                    var nodeClone = baseNodeView.state;
                    //判断需要复原的节点
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

                    //新旧节点映射
                    if (nodeView != null)
                    {
                        nodesDict.Add(baseNodeView, nodeView);
                    }

                    nodeView.OnNodeSelected = OnNodeSelected;
                    AddElement(nodeView);
                    nodeView.state = (NodeState)userSeletionGo.AddComponent(baseNodeView.state.GetType());
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(nodeView.state);

                    //调整一下流向
                    //保持原来的流向算法好难写，还是全部设置成null把
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

                    //复制出来的节点位置偏移
                    nodeView.SetPosition(new Rect(baseNodeView.GetPosition().position + (Vector2.one * 30 * offset),nodeView.GetPosition().size));
                }
            }

            for (int i = selection.Count - 1; i >= 0; i--)
            {
                //取消选择
                this.RemoveFromSelection(selection[i]);
            }

            foreach (var node in nodesDict.Values)
            {
                //选择新生成的节点
                this.AddToSelection(node);
            }
        }
    }
}
