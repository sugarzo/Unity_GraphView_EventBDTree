using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace SugarFrame.Node
{
    public enum EState
    {
        [LabelText("δִ��")]
        None,
        [LabelText("���ڽ���")]
        Enter,
        [LabelText("����ִ��")]
        Running,
        [LabelText("�����˳�")]
        Exit,
        [LabelText("ִ�����")]
        Finish,
    }
    public interface IStateEvent
    {
        void Execute();
        void OnEnter();
        void OnRunning();
        void OnExit();
    }

    public abstract class NodeState : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 nodePos;
        [LabelText("�ڵ�ע��"), OnValueChanged(nameof(UpdateNodeName))]
        public string node_name = "";
        [HideInInspector]
        public UnityEditor.Experimental.GraphView.Node node;

        private void UpdateNodeName()
        {
            if (node != null && !string.IsNullOrEmpty(node_name))
               node.title = this.GetType().Name + "\n" + node_name;
        }
#endif
        //������һ�ڵ����
        public MonoState nextFlow;
    }

    public abstract class MonoState : NodeState, IStateEvent
    {
        [SerializeField,Space]
        protected EState state;

        [TextArea,Space]
        public string note;

        protected void TransitionState(EState _state)
        {
            state = _state;

            switch (state)
            {
                case EState.Enter:
                    OnEnter();
                    break;
                case EState.Running:
                    OnRunning();
                    break;
                case EState.Exit:
                    OnExit();
                    break;
            }
        }

        public virtual void Execute()
        {
            TransitionState(EState.Enter);
        }
        public virtual void OnEnter()
        {
            TransitionState(EState.Running);
        }
        public virtual void OnRunning()
        {
            TransitionState(EState.Exit);
        }
        public virtual void OnExit()
        {
            TransitionState(EState.Finish);
        }
    }
}


