using SugarFrame.Node;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeViewElement : Node
{
    public new class UxmlFactory : UxmlFactory<NodeViewElement, UxmlTraits> { }
}
