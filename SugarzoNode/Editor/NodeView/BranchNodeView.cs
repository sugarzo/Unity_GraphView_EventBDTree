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
    }
}
