using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace SugarFrame.Node
{
    public class SequenceNodeView : BaseNodeView<BaseSequence>
    {
        public SequenceNodeView()
        {
            //Sequence有一个输出端口一个输入端口,输入接口只能单连接，输出端口可以多连接
            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Single);
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Multi);
            input.portName = "input";
            output.portName = "output";

            title = state != null ? state.name : "SequenceNode";

            inputContainer.Add(input);
            outputContainer.Add(output);
        }

        public override void OnEdgeCreate(Edge edge)
        {
            base.OnEdgeCreate(edge);

            BaseNodeView targetView = edge.input.node as BaseNodeView;
            (state as BaseSequence).nextflows.Add(targetView.state as MonoState);
        }

        public override void OnEdgeRemove(Edge edge)
        {
            base.OnEdgeRemove(edge);

            BaseNodeView targetView = edge.input.node as BaseNodeView;
            (state as BaseSequence).nextflows.Remove(targetView.state as MonoState);
        }
    }
}
