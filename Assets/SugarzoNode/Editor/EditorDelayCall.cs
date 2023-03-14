using System;
using UnityEditor;
using UnityEngine;

public class EditorDelayCall
{
    public class BoolClass
    {
        public bool value;
    }
    /// <summary>
    /// 延迟秒数
    /// </summary>
    private float _delay;

    private Action _callback;
    private float _startupTime;

    public void Call(float delay, Action callback)
    {
        this._delay = delay;
        this._callback = callback;

        EditorApplication.update += Update;
    }

    public void CheckBoolCall(float delay, BoolClass boolClass,
            Action action)
    {
        if (!boolClass.value)
        {
            boolClass.value = true;
            action?.Invoke();
            Call(delay, delegate { boolClass.value = false; });
        }
    }

    // 主动停止
    public void Stop()
    {
        _startupTime = 0;
        _callback = null;

        EditorApplication.update -= Update;
    }

    private void Update()
    {
        // 时间初始化放在这里是因为如果在某些类的构造函数中获取时间是不允许的
        if (_startupTime <= 0)
        {
            _startupTime = Time.realtimeSinceStartup;
        }

        if (Time.realtimeSinceStartup - _startupTime >= _delay)
        {
            _callback?.Invoke();
            Stop();
        }
    }
}