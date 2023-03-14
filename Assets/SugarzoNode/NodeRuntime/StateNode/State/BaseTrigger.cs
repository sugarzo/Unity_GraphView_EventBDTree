using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SugarFrame.Node
{
    public enum ExecutePeriod
    {
        None,
        Awake,
        Enable,
        Start,
        Update,
        DisEnable,
        Destroy,
    }

    public interface ITriggerEvent
    {
        void RegisterSaveTypeEvent();
        void DeleteSaveTypeEvent();
    }

    public abstract class BaseTrigger : MonoState,ITriggerEvent
    {
        [LabelText("生命周期执行")]
        public ExecutePeriod executePeriod = ExecutePeriod.None;
        [LabelText("运行中可再次被激活")]
        public bool canExecuteOnRunning = false;
        [LabelText("只执行一次")]
        public bool runOnlyOnce = false;

        //(可选)在子类中实现下面两个方法
        public virtual void RegisterSaveTypeEvent()
        {
            //EventManager.StartListening("");
        }
        public virtual void DeleteSaveTypeEvent()
        {
            //EventManager.StopListening("");
        }

        [Button]
        public override void Execute()
        {
            if (!UnityEngine.Application.isPlaying)
                return;
            if (!canExecuteOnRunning)
                if (state == EState.Enter || state == EState.Running || state == EState.Exit)
                    return;

            base.Execute();

            if (runOnlyOnce)
                Destroy(this);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (nextFlow != null)
            {
                if (nextFlow is BaseAction nextAction)
                    nextAction.Execute(this);
                else
                    nextFlow.Execute();
            }
                
        }

        public override void OnRunning()
        {
            //Trigger不需要实现OnRunning,由Action回调OnExit退出
            //base.OnRunning();
        }

        public override void OnExit()
        {
            base.OnExit();

        }

        protected virtual void Awake()
        {
            if (executePeriod == ExecutePeriod.Awake)
                Execute();
        }

        Coroutine updateCoroutine = null;

        protected virtual void OnEnable()
        {
            if (executePeriod == ExecutePeriod.Enable)
                Execute();

            RegisterSaveTypeEvent();

            //使用协程模拟update，优化不选择ExecutePeriod.Update时的性能
            if (executePeriod == ExecutePeriod.Update)
                updateCoroutine = StartCoroutine(IEUpdate());
        }

        protected virtual void Start()
        {
            if (executePeriod == ExecutePeriod.Start)
                Execute();
        }

        protected virtual IEnumerator IEUpdate()
        {
            while(true)
            {
                yield return null;
                if (gameObject.activeSelf)
                    Execute();
                else
                    yield break;
            }
        }

        protected virtual void OnDisable()
        {
            if (executePeriod == ExecutePeriod.DisEnable)
                Execute();

            DeleteSaveTypeEvent();

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
                
        }

        protected virtual void OnDestroy()
        {
            if (executePeriod == ExecutePeriod.Destroy)
                Execute();
        }
    }
}


