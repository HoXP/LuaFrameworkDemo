using System.Collections.Generic;
using System.Reflection;

using XLua;
using System;
using CSObjectWrapEditor;


public class GeneratorUtils {
    
    public static bool IsOutExport(PropertyInfo propertyInfo, bool isIncludeMethod = true)
    {
        if(propertyInfo == null)
    	{
    		return false;
    	}
    	List<string> whiteListValue = GetWhiteListOfClass(propertyInfo);
        if (whiteListValue != null)//存在于白名单
    	{
            bool isIn = isWhiteListMethod(propertyInfo, whiteListValue);
            return isIn;
    	}
        bool isOut = !isMemberInBlackList(propertyInfo) && !isObsolete(propertyInfo);
        if (isIncludeMethod)
    	{
            isOut = isOut && !isMethodInBlackList(propertyInfo.GetGetMethod()) && !isObsolete(propertyInfo.GetGetMethod());
    	}
        return isOut;
    }

	//是否输出
    public static bool IsOutExport(MemberInfo memberInfo)
    {
        if(memberInfo == null)
    	{
    		return false;
    	}
        List<string> whiteListValue = GetWhiteListOfClass(memberInfo);
        if (whiteListValue != null)//存在于白名单
        {
            bool isIn = isWhiteListMethod(memberInfo, whiteListValue);
            return isIn;
        }
        bool isOut = !isMemberInBlackList(memberInfo) && !isObsolete(memberInfo);
        return isOut;
    }

    public static PropertyInfo[] GetTypeProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
    }

    //是否过时了
    public static bool isObsolete(MemberInfo mb)
    {
        if (mb == null) return false;
        return mb.IsDefined(typeof(System.ObsoleteAttribute), false);
    }

    //是否是方法函数
    public static bool isWhiteListMethod(MemberInfo memberInfo, List<string> list)
    {
    	if(memberInfo == null || list == null)
    	{
    		return false;
    	}
    	string name = memberInfo.Name;
        if(name.Contains("set_"))
        {
            name = name.Replace("set_", "");
        }
        if (name.Contains("get_"))
        {
            name = name.Replace("get_", "");
        }
        return list.Contains(name);
    }

    //参数，public int  get set
    public static List<string> GetWhiteListOfClass(MemberInfo mi)
    {
    	if (mi.DeclaringType == null)
            return null;
        string className = mi.DeclaringType.Name;
        Type type = mi.DeclaringType;
        for (int i = 0; i < Generator.WhiteList.Count; i++)
        {
            if (type.IsGenericType)
            {
                if (className.Contains(Generator.WhiteList[i].Key) && (mi.MemberType == MemberTypes.Method))
                {
                    return Generator.WhiteList[i].Value;
                }
            }
            else
            {
                if (Generator.WhiteList[i].Key.Trim() == type.FullName.Trim())
                {
                    return Generator.WhiteList[i].Value;
                }
            }
        }
        return null;
    }

    public static bool isMemberInBlackList(MemberInfo mb)
    {
        if (mb.IsDefined(typeof(BlackListAttribute), false))
        {
            return true;
        }
        if (mb.DeclaringType == null)
            return true;
        string className = mb.DeclaringType.Name;
        Type type = mb.DeclaringType;
        string name = mb.Name;
        //UnityEngine.Debug.Log("className===" + className + "  name==" + name + "  type.FullName===" + type.FullName);
        foreach (var exclude in Generator.BlackList)
        {
            if (exclude.Key.Trim() == type.FullName.Trim())
            {
                var value = exclude.Value;
                return value.Contains(name);
            }
        }

        return false;
    }

    public static bool isMethodInBlackList(MethodBase mb)
    {
        return isMemberInBlackList(mb);
        //if (mb.IsDefined(typeof(BlackListAttribute), false))
        //{
        //    return true;
        //}

        //foreach (var exclude in Generator.BlackList)
        //{
        //    if (mb.DeclaringType.FullName == exclude[0].Trim() && mb.Name == exclude[1].Trim())
        //    {
        //        var parameters = mb.GetParameters();
        //        if (parameters.Length != exclude.Count - 2)
        //        {
        //            continue;
        //        }
        //        bool paramsMatch = true;

        //        for (int i = 0; i < parameters.Length; i++)
        //        {
        //            if (parameters[i].ParameterType.FullName.Trim() != exclude[i + 2].Trim())
        //            {
        //                paramsMatch = false;
        //                break;
        //            }
        //        }
        //        if (paramsMatch) return true;
        //    }
        //}
        //return false;
    }

}
