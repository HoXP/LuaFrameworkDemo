/// <summary>
/// added by ggr @ 2018/04/19
/// 用于lua侧使用C#侧或unity定义的宏。
/// 使用：
/// 1、如果添加在Unity3D的 File->Build Setting->Scripting Define Symbols 添加了新的宏并且在代码里需要使用，
///    则需要在这里添加对应的判断函数。
/// 2、Lua侧使用请参考 MacroUtil.lua 的类头。
/// </summary>
public static class LuaCallCSharpMacro
{
    public static bool UNITY_ANDROID()
    {
#if UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }

    //ios环境
    public static bool UNITY_IOS()
    {
#if UNITY_IOS
        return true;
#else
        return false;
#endif
    }

    //编辑器环境
    public static bool UNITY_EDITOR()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    //是否需要接受userinfo
    public static bool IS_NEED_LOSD_USER_INFO()
    {
#if IS_NEED_LOSD_USER_INFO
        return true;
#else
        return false;
#endif
    }

    public static bool RUN_IN_DOWNLOAD_RES()
    {
#if RUN_IN_DOWNLOAD_RES
        return true;
#else
        return false;
#endif
    }
    public static bool DOWNLOAD_TEMP_RES()
    {
#if DOWNLOAD_TEMP_RES
        return true;
#else
        return false;
#endif
    }
    
}
