using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using XLua;

public class LuaUpdater : MonoBehaviour
{
    private static volatile LuaUpdater ms_Instance;
    public static LuaUpdater Instance
    {
        get
        {
            return ms_Instance;
        }
    }
    private void Awake()
    {
        ms_Instance = this;
    }
    OnCallBackFF luaUpdate = null;
    OnCallBack luaSlowUpdate = null;
    OnCallBack luaLateUpdate = null;
    OnCallBackFloat luaFixedUpdate = null;
    int slowUpdateFPS = 0;

    float slowUpdateRate = 0;
    float timeUpdateElapsed = float.MaxValue;

#if UNITY_EDITOR
#pragma warning disable 0414
    // added by ggr @ 2017-12-29
    [SerializeField]
    long updateElapsedMilliseconds = 0;
    [SerializeField]
    long lateUpdateElapsedMilliseconds = 0;
    [SerializeField]
    long fixedUpdateElapsedMilliseconds = 0;
#pragma warning restore 0414
    Stopwatch sw = new Stopwatch();
#endif

    public void OnInit(LuaEnv luaEnv)
    {
#if UNITY_EDITOR
        sw.Start();
#endif
        Restart(luaEnv);
    }

    public void Restart(LuaEnv luaEnv)
    {
        luaUpdate = luaEnv.Global.Get<OnCallBackFF>("Update");
        luaSlowUpdate = luaEnv.Global.Get<OnCallBack>("SlowUpdate");
        luaLateUpdate = luaEnv.Global.Get<OnCallBack>("LateUpdate");
        luaFixedUpdate = luaEnv.Global.Get<OnCallBackFloat>("FixedUpdate");
        slowUpdateFPS = luaEnv.Global.Get<int>("SlowUpdateFPS");
        slowUpdateRate = 1F / slowUpdateFPS;
    }

    void Update()
    {
        if (luaUpdate != null)
        {
#if UNITY_EDITOR
            var start = sw.ElapsedMilliseconds;
#endif
            try
            {
                luaUpdate(Time.deltaTime, Time.unscaledDeltaTime);
                if(timeUpdateElapsed >= slowUpdateRate)
                {
                    luaSlowUpdate();
                    timeUpdateElapsed = 0;
                }
                timeUpdateElapsed += Time.deltaTime;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("luaUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
#if UNITY_EDITOR
            updateElapsedMilliseconds = sw.ElapsedMilliseconds - start;
#endif
        }
    }

    void LateUpdate()
    {
        if (luaLateUpdate != null)
        {
#if UNITY_EDITOR
            var start = sw.ElapsedMilliseconds;
#endif
            try
            {
                luaLateUpdate();
            }
            catch (Exception ex)
            {
                UnityEngine. Debug.LogError("luaLateUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
#if UNITY_EDITOR
            lateUpdateElapsedMilliseconds = sw.ElapsedMilliseconds - start;
#endif
        }
    }

    void FixedUpdate()
    {
        if (luaFixedUpdate != null)
        {
#if UNITY_EDITOR
            var start = sw.ElapsedMilliseconds;
#endif
            try
            {
                luaFixedUpdate(Time.fixedDeltaTime);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("luaFixedUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
#if UNITY_EDITOR
            fixedUpdateElapsedMilliseconds = sw.ElapsedMilliseconds - start;
#endif
        }
    }

    public void OnDispose()
    {
        luaUpdate = null;
        luaLateUpdate = null;
        luaFixedUpdate = null;
        luaSlowUpdate = null;
    }

}