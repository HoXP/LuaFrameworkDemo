

using System;
using System.Collections.Generic;
using System.IO;
using XLua;

public struct CustomGenTask
{
    public LuaTable Data;
    public TextWriter Output;
}

public struct UserConfig
{
    public IEnumerable<Type> LuaCallCSharp;
    public IEnumerable<Type> CSharpCallLua;
    public IEnumerable<Type> ReflectionUse;
}

public class GenCodeMenuAttribute : Attribute
{

}

public class GenPathAttribute : Attribute
{

}

public struct XLuaTemplate
{
    public string name;
    public string text;
}

public struct XLuaTemplates
{
    public XLuaTemplate LuaClassWrap;
    public XLuaTemplate LuaDelegateBridge;
    public XLuaTemplate LuaDelegateWrap;
    public XLuaTemplate LuaEnumWrap;
    public XLuaTemplate LuaInterfaceBridge;
    public XLuaTemplate LuaRegister;
    public XLuaTemplate LuaWrapPusher;
    public XLuaTemplate PackUnpack;
    public XLuaTemplate TemplateCommon;
}
    
class ParameterInfoSimulation
{
    public string Name;
    public bool IsOut;
    public bool IsIn;
    public Type ParameterType;
    public bool IsParamArray;
}

class MethodInfoSimulation
{
    public Type ReturnType;
    public ParameterInfoSimulation[] ParameterInfos;

    public int HashCode;

    public ParameterInfoSimulation[]  GetParameters()
    {
        return ParameterInfos;
    }

    public Type DeclaringType = null;
    public string DeclaringTypeName = null;
}

class XluaFieldInfo
{
    public string Name;
    public Type Type;
    public bool IsField;
    public int Size;
}

class XluaTypeInfo
{
    public Type Type;
    public List<XluaFieldInfo> FieldInfos;
    public List<List<XluaFieldInfo>> FieldGroup;
    public bool IsRoot;
}