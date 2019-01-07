using System;

using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
public class NativeCenter : MonoBehaviour {

    private static volatile NativeCenter ms_Instance;
    public static NativeCenter Instance
    {
        get
        {
            return ms_Instance;
        }
    }

    private static AndroidJavaObject JavaObject;
    private static AndroidJavaObject audioManager;
    private void Awake()
    {
        ms_Instance = this;
    }

#if UNITY_IOS
        [DllImportAttribute("__Internal")]
        public static extern void u3dToNativeFunction(string param);
#endif
    public static void InvokeIosNativeMethod(string methodName)
    {
        InvokeIosNativeMethod(methodName, null);
    }
    public static void InvokeIosNativeMethod(string methodName , string param)
    {
        try
        {
#if UNITY_IOS
                string result = "";
                if (param != null)
                {
                    result = string.Format("{0}\"methodName\":\"{2}\", \"param\":{3}{1}", "{", "}", methodName, param);
                }else
                {
                    result = string.Format("{0}\"methodName\":\"{2}\", \"param\":null{1}", "{", "}", methodName);
                }
                MessageCenter.Instance.StartRunNativeToU3d(result);
#endif
        }
        catch (Exception e)
        {
            Debug.Log("error----------------" + e);
        }
    }

    public static void InvokeAndroidNativeMethod(string methodName)
    {
        InvokeAndroidNativeMethod(methodName, null);
    }
    public static void InvokeAndroidNativeMethod(string methodName, string param)
    {
        AsyncTask.runInUIThread(() =>
        {
            if(LuaCallCSharpMacro.UNITY_ANDROID())
            {
                if (JavaObject == null)
                {
                    AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    JavaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
                }
                if (string.IsNullOrEmpty(param))
                {
                    JavaObject.Call("invokeAndroidMethod", methodName);
                }
                else
                {
                    Debug.Log("xxx InvokeAndroidNativeMethod  param=" + param);
                    JavaObject.Call("invokeAndroidMethod", methodName, param);
                }
            }
        });
    }
    private const string currentVolume = "getStreamVolume";         //当前音量
    private const string maxVolume = "getStreamMaxVolume";          //最大音量

    private const int STREAM_MUSIC = 3;                             // 媒体音量 --unity 用到的


    public static float GetAndroidAudioInfo()
    {
        if (LuaCallCSharpMacro.UNITY_ANDROID())
        {
            if (JavaObject == null)
            {
                AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                JavaObject = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
            if (audioManager == null && JavaObject != null)
            {
                audioManager = JavaObject.Call<AndroidJavaObject>("getSystemService", new AndroidJavaObject("java.lang.String", "audio"));
            }

            if (audioManager != null)
            {
                int musicl_value = audioManager.Call<int>(currentVolume, STREAM_MUSIC);
                int max_musicl_value = audioManager.Call<int>(maxVolume, STREAM_MUSIC);
                return 100 * (float)musicl_value / (float)max_musicl_value;
            }
        }
        return 0;
    }
}
