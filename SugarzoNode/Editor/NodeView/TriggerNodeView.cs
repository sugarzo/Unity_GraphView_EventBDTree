using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace SugarFrame.Node
{
    public class TriggerNodeView : BaseNodeView<BaseTrigger>
    {
        public TriggerNodeView()
        {
            title = state != null ? state.name : "TriggerNode";

            //Trigger只有一个输出端口
            Port output = GetPortForNode(this, Direction.Output, Port.Capacity.Single);
            output.portName = "output";
            outputContainer.Add(output);


        }
    }
}
