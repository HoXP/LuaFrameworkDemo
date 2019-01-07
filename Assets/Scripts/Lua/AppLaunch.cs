using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class AppLaunch : MonoBehaviour
{
    private static volatile AppLaunch ms_Instance;
    public static AppLaunch Instance
    {
        get
        {
            return ms_Instance;
        }
    }

    private void Awake()
    {
        ms_Instance = this;
        InitGameObject();
        XLuaManager.Instance.PreLoaderLua();//将base的lua全都加载进来
        XLuaManager.Instance.OnInit();//初始化
        //初始化异步任务
        AsyncTaskManager.Setup();
    }

    public void StartGame()
    {
        DragonBones.UnityFactory.factory.Clear();
        StartCoroutine(StartGameCallBack());
    }

    IEnumerator StartGameCallBack()
    {
        yield return null;
        XLuaManager.Instance.StartGame();
    }
    private void Start()
    {
        if (LuaCallCSharpMacro.IS_NEED_LOSD_USER_INFO() == false) //不走userinfo
        {
            if (LuaCallCSharpMacro.RUN_IN_DOWNLOAD_RES() || LuaCallCSharpMacro.DOWNLOAD_TEMP_RES())//去下载资源
            {
                XLuaManager.Instance.StartShowLoginView();
            }
            else
            {
                StartGame();
            }
        }
    }

    void InitGameObject()
    {
        gameObject.AddComponent<AppInit>();
        gameObject.AddComponent<MessageCenter>();
        gameObject.AddComponent<XLuaManager>();
        gameObject.AddComponent<UnityHelpCenter>();
        gameObject.AddComponent<NativeCenter>();
        gameObject.AddComponent<togetherzy.AssetBundle.SoundManager>();
        if (LuaCallCSharpMacro.IS_NEED_LOSD_USER_INFO() == false)
        {
            gameObject.AddComponent<FPSInfo>();
        }
    }

    private void OnDestroy()
    {
        Dispose();
        XLuaManager.Instance.Dispose();
        MessageCenter.Instance.Clear();
    }

    public void Dispose()
    {
        if (UnityHelpCenter.Instance != null)
        {
            UnityHelpCenter.Instance.Clear();
        }

        User.Http.Download.DownloadManager.Clear();
        DragonBones.UnityFactory.factory.Clear();
        ThreeDInteractable.Dispose();
        if (LuaUpdater.Instance != null)
        {
            LuaUpdater.Instance.OnDispose();
        }

    }
    public static void OnLoadUpdateLua()
    {
        XLuaManager.Instance.OnLoadUpdateLua();
    }
}