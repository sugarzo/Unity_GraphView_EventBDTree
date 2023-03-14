using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

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

    public abstract partial class NodeState : MonoBehaviour
    {
        public virtual EState State 
        { 
            get 
            { 
                return EState.None; 
            } 
        }
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
#if UNITY_EDITOR
            //��ʾEditor��Graph��ɫ�����ޱ༭��ģʽ�µ�Runtime Debug��
            if(node != null)
            {
                Color runningColor = new Color(0.37f, 1, 1, 1f); //ǳ��
                Color compeletedColor = new Color(0.5f, 1, 0.37f, 1f); //ǳ��
                Color portColor = new Color(0.41f, 0.72f, 0.72f, 1f); //����

                if (State == EState.Running || State == EState.Enter || State == EState.Exit)
                {
                    node.titleContainer.style.backgroundColor = new StyleColor(runningColor);
                }
                if (State == EState.Finish)
                {
                    node.titleContainer.style.backgroundColor = new StyleColor(compeletedColor);
                }
            }
#endif
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

        public override EState State
        { 
            get 
            { 
                return state; 
            } 
        }
    }
}


