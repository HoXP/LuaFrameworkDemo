
using System.IO;
using UnityEngine;
using XLua;

public class XLuaManager : MonoBehaviour
{
    private static volatile XLuaManager ms_Instance;
    public static XLuaManager Instance
    {
        get
        {
            return ms_Instance;
        }
    }

    private void Awake()
    {
        ms_Instance = this;
        luaEnv = new LuaEnv();
    }

    public const string luaAssetbundleAssetName = "Lua";
    const string preLoadMainLua = "unpack.LoginMain";
    const string gameMainScriptName = "AppMain";
    LuaEnv luaEnv = null;
    LuaUpdater luaUpdater = null;

    public bool HasGameStart
    {
        get;
        protected set;
    }

    public void OnInit()
    {
        if (luaEnv == null)
        {
            return;
        }
        OnCallBackString BridgingAction = luaEnv.Global.Get<OnCallBackString>("BridgingAction");
        OnCallBackString KeyMessageAction = luaEnv.Global.Get<OnCallBackString>("KeyMessage");
        OnCallBackStringStringInt StackTrace = luaEnv.Global.Get<OnCallBackStringStringInt>("StackTrace");
        MessageCenter.Instance.OnInit(BridgingAction, KeyMessageAction, StackTrace);
    }
    public LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }

    public void PreLoaderLua()
    {
        if (LuaCallCSharpMacro.UNITY_EDITOR() == false)//只要不是deitor环境，都需要解压luadata
        {
            //先检查是否有下载的unpack lua,优先找下载的unpackdata
            string unPackluaDataFilePath = Path.Combine(PathManager.GetRootDataPath, PathManager.download_unpackLuaName);
            if (FileManager.FileExist(unPackluaDataFilePath))
            {
                YQPackageManagerEX.Instance.LoadLuaData(unPackluaDataFilePath);
            }
            else
            {
                string path = Path.Combine(PathManager.unpackPath, PathManager.unpackLuaName);
                YQPackageManagerEX.Instance.LoadLuaDataFromResource(FileManager.RemoveFileExtension(path));
            }
        }
        if (luaEnv != null)
        {
            luaEnv.AddLoader(PreLoaderLua);
            LoadScript(preLoadMainLua);
        }
        else
        {
            UnityEngine.Debug.LogError("PreLoaderLua null!!!");
        }
    }

    public void LoadCustomLua()
    {
        HasGameStart = false;

        if (LuaCallCSharpMacro.RUN_IN_DOWNLOAD_RES() == false && LuaCallCSharpMacro.DOWNLOAD_TEMP_RES() == false)//不走下载
        {
            YQPackageManagerEX.Instance.LoadLuaDataFromResource(FileManager.RemoveFileExtension(PathManager.needPackLuaName));
        }

        if (luaEnv != null)
        {
            luaEnv.AddLoader(CustomLoader);
        }
        else
        {
            UnityEngine.Debug.LogError("LoadCustomLua null!!!");
        }
    }

    // 这里必须要等待资源管理模块加载Lua AB包以后才能初始化
    public void OnLoadUpdateLua()
    {
        if (luaEnv != null)
        {
            luaUpdater = gameObject.GetComponent<LuaUpdater>();
            if (luaUpdater == null)
            {
                luaUpdater = gameObject.AddComponent<LuaUpdater>();
            }
            luaUpdater.OnInit(luaEnv);
        }
    }

    public string AssetbundleName
    {
        get;
        protected set;
    }

    // 重启虚拟机：热更资源以后被加载的lua脚本可能已经过时，需要重新加载
    // 最简单和安全的方式是另外创建一个虚拟器，所有东西一概重启
    public void Restart()
    {
        Dispose();
    }

    public void SafeDoString(string scriptContent)
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.DoString(scriptContent);
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                UnityEngine.Debug.LogError(msg, null);
            }
        }
    }

    public void StartShowLoginView()
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.DoString("LoginMain.LoadLoginView()", "LoginMain.lua");
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                UnityEngine.Debug.LogError(msg, null);
            }
        }
    }
    public void StartGame()
    {
        LoadCustomLua();
        if (luaEnv != null)
        {
            LoadScript(gameMainScriptName);
            SafeDoString("AppMain.Start()");
            HasGameStart = true;
        }
    }

    public void ReloadScript(string scriptName)
    {
        SafeDoString(string.Format("package.loaded['{0}'] = nil", scriptName));
        LoadScript(scriptName);
    }

    void LoadScript(string scriptName)
    {
        SafeDoString(string.Format("require('{0}')", scriptName));
    }

    public static byte[] PreLoaderLua(ref string filepath)
    {
        if (LuaCallCSharpMacro.UNITY_EDITOR())//只有开发环境才走本地
        {
            string scriptPath = string.Empty;
            filepath = filepath.Replace(".", "/") + ".lua";
            string path = Application.dataPath + "/Resources/" + PathManager.common_lua_scripts_Path;
            scriptPath = Path.Combine(path, filepath);
            FileCatchData data = FileManager.ReadAllBytes(scriptPath);
            if (data == null || data.state == false)
            {
                path = Application.dataPath + "/Resources/" + PathManager.app_lua_scripts_Path;
                scriptPath = Path.Combine(path, filepath);
                data = FileManager.ReadAllBytes(scriptPath);
            }
            return (byte[])data.data;
        }
        else
        {
            return YQPackageManagerEX.Instance.GetLuaDataByName(filepath);
        }
    }

    public static byte[] CustomLoader(ref string filepath)
    {
        return YQPackageManagerEX.Instance.GetLuaDataByName(filepath);
    }

    private void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();

            if (Time.frameCount % 100 == 0)
            {
                luaEnv.FullGc();
            }
        }
    }

    public void OnLevelWasLoaded()
    {
        if (luaEnv != null && HasGameStart)
        {
            SafeDoString("AppMain.OnLevelWasLoaded()");
        }
    }

    private void OnApplicationQuit()
    {
        if (luaEnv != null && HasGameStart)
        {
            SafeDoString("AppMain.OnApplicationQuit()");
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        Debug.Log("xxx OnApplicationPause" + isPaused);
        if (luaEnv == null || HasGameStart == false)
        {
            return;
        }
        if (isPaused)
        {
            SafeDoString("AppMain.OnApplicationPause()");
        }
        else
        {
            SafeDoString("AppMain.OnApplicationUnPause()");
        }
    }

    public void Dispose()
    {
        if (luaUpdater != null)
        {
            luaUpdater.OnDispose();
        }
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                UnityEngine.Debug.LogError(msg, null);
            }
        }
    }
}
