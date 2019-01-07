#if UNITY_IOS
using System.Collections;
#endif
using UnityEngine;
using XLua;

public class MessageCenter : MonoBehaviour
{
    private static volatile MessageCenter ms_Instance;
    public static MessageCenter Instance
    {
        get
        {
            return ms_Instance;
        }
    }

    private OnCallBackString BridgingAction = null;     //C#与lua桥接消息委托
    private OnCallBackString KeyMessageAction = null;   //向lua发送按键事件的委托
    private OnCallBackStringStringInt StackTrace = null;
    private OnCallBackStringStringIntLong LogInfoAction = null;
    private LuaEnv luaEnv;
    private bool Debugging;
    private bool LogEnabled = true;
    private bool pressBegin = false;
    private float pressTime = float.MaxValue;
    private float pressTimeThreshold = 1.5f;
    private float pressArea = 300;

    private void Awake()
    {
        ms_Instance = this;
        GetInitInfo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (KeyMessageAction != null)
            {
                KeyMessageAction("Escape");
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (KeyMessageAction != null)
            {
                KeyMessageAction("A");
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (KeyMessageAction != null)
            {
                KeyMessageAction("B");
            }
        }

        if (Debugging && Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < 300 && Input.mousePosition.y < Screen.height - 300)
            {
                pressBegin = true;
                pressTime = Time.time;
            }
        }

        if (Debugging && Input.GetMouseButtonUp(0) && pressBegin)
        {
            if (Input.mousePosition.x < pressArea && Input.mousePosition.y < Screen.height - pressArea)
            {
                pressBegin = false;
                pressTime = float.MaxValue;
            }
        }

        if (pressBegin && Time.time - pressTime >= pressTimeThreshold)
        {
            if (Input.mousePosition.x < pressArea && Input.mousePosition.y < Screen.height - pressArea)
            {
                pressBegin = false;
                pressTime = float.MaxValue;
                if (luaEnv != null)
                {
                    var func = luaEnv.Global.Get<OnCallBack>("ShowLoggerWindow");
                    if (func != null)
                    {
                        func.Invoke();
                    }
                }
            }
        }
    }

    public void SetDebug()
    {
        luaEnv = XLuaManager.Instance.GetLuaEnv();
        if (luaEnv != null)
        {
            Debugging = luaEnv.Global.GetInPath<bool>("BaseConfig.Debugging");
            LogInfoAction = luaEnv.Global.Get<OnCallBackStringStringIntLong>("LogInfo");
        }
    }

    public void SeLogEnable(bool enabled)
    {
        LogEnabled = enabled;
    }

    public void OnInit(OnCallBackString _BridgingAction, OnCallBackString _KeyMessageAction, OnCallBackStringStringInt _StackTrace)
    {
        BridgingAction = _BridgingAction;
        KeyMessageAction = _KeyMessageAction;
        StackTrace = _StackTrace;
    }

    public void GetInitInfo()
    {
        if (LuaCallCSharpMacro.IS_NEED_LOSD_USER_INFO())
        {
            if (LuaCallCSharpMacro.UNITY_IOS())
            {
                NativeCenter.InvokeIosNativeMethod("getInitInfo");
            }
            else if (LuaCallCSharpMacro.UNITY_ANDROID())
            {
                NativeCenter.InvokeAndroidNativeMethod("getInitInfo");
            }
        }
    }

    public void unityReceiver(string msg)
    {
        if (BridgingAction != null)
        {
            BridgingAction(msg);
        }
    }
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (StackTrace != null)
            {
                StackTrace(condition, stackTrace, (int)type);
            }
        }
        if (LogInfoAction != null && Debugging && LogEnabled)
        {
            LogInfoAction.Invoke(condition, stackTrace, (int)type, CommonUtils.GetTimeStamp());
        }
    }

    public void Clear()
    {
        BridgingAction = null;
        KeyMessageAction = null;
        StackTrace = null;
        LogInfoAction = null;
    }

#if UNITY_IOS
    public void StartRunNativeToU3d(string data)
    {
        StartCoroutine(StartRunNativeToU3dCallBack(data));
    }

    IEnumerator StartRunNativeToU3dCallBack(string data)
    {
        yield return new WaitForSeconds(0.2f);
        NativeCenter.u3dToNativeFunction(data);
    }
#endif
}