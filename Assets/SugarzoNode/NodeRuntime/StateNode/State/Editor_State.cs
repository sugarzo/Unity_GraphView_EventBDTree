using Sirenix.OdinInspector;
using UnityEngine;

namespace SugarFrame.Node
{
#if UNITY_EDITOR
    public abstract partial class NodeState : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 nodePos; //GraphView使用

        [LabelText("节点注释"), OnValueChanged(nameof(UpdateNodeName))]
        public string explanatoryNote = "";

        [HideInInspector]
        public UnityEditor.Experimental.GraphView.Node node;

        private void UpdateNodeName()
        {
            node.title = GetNodeName();
        }

        public string GetNodeName()
        {
            if (!string.IsNullOrWhiteSpace(explanatoryNote))
                return explanatoryNote;

            string ret = GetType().Name;
            if (GetType().IsDefined(typeof(NodeNoteAttribute), true))
                ret += "\n" + (System.Attribute.GetCustomAttribute(GetType(), typeof(NodeNoteAttribute)) as NodeNoteAttribute).note;
            return ret;
        }
    }

#endif
}
