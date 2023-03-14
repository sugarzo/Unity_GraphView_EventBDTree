using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace SugarFrame.Node
{
    public class BranchNodeView : BaseNodeView<BaseBranch>
    {
        public BranchNodeView()
        {
            //Sequence有两个输出端口一个输入端口
            Port input = GetPortForNode(this, Direction.Input, Port.Capacity.Multi);
            Port output1 = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            Port output2 = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            input.portName = "input";
            output1.portName = "true";
            output2.portName = "false";

            title = state != null ? state.name : "IfNode";

            inputContainer.Add(input);
            outputContainer.Add(output1);
            outputContainer.Add(output2);
        }

        public override void OnEdgeCreate(Edge edge)
        {
            base.OnEdgeCreate(edge);

            BaseNodeView parentView = edge.output.node as BaseNodeView; //自己
            BaseNodeView childView = edge.input.node as BaseNodeView;

            if (edge.output.portName.Equals("true"))
            {
                (parentView.state as BaseBranch).trueFlow = childView.state as MonoState;
            }
            if (edge.output.portName.Equals("false"))
            {
                (parentView.state as BaseBranch).falseFlow = childView.state as MonoState;
            }
        }

        public override void OnEdgeRemove(Edge edge)
        {
            base.OnEdgeRemove(edge);

            BaseNodeView parentView = edge.output.node as BaseNodeView; //自己
            BaseNodeView childView = edge.input.node as BaseNodeView;

            if (edge.input.portName == "true")
            {
                (parentView.state as BaseBranch).trueFlow = null;
            }
            if (edge.input.portName == "false")
            {
                (parentView.state as BaseBranch).falseFlow = null;
            }
        }
    }
}
