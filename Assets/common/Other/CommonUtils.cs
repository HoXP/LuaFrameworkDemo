
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public partial class CommonUtils
{
    public static long GetTimeStamp()
    {
        return GetTimeStamp(true);
    }

    public static RenderTexture CreateRenderTexture(int width, int height, int depth, bool useMipMap)
    {
        RenderTexture data =  new RenderTexture(width, height, depth);
        data.useMipMap = useMipMap;
        return data;
    }
    public static void SetQualitySettingsLevel(int level)
    {
        string[] names = QualitySettings.names;
        if(names != null &&  names.Length > level)
        {
            QualitySettings.SetQualityLevel(level);
        }
    }

    public static void SetPixelDragThreshold(int value)
    {
        UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = Mathf.CeilToInt(Screen.dpi / value);
    }

    public static void UpdatePixelDragThreshold()
    {
        UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = Mathf.CeilToInt(Screen.dpi / 40);
    }
    public static long GetTimeStamp(bool bflag)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (bflag)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }

    public static Uri GetSystemUri(string luaUrl)
    {
        Uri _uri = new Uri(luaUrl);
        return _uri;
    }
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    public static void UnityEvent_RemoveAllListeners(UnityEngine.Events.UnityEventBase evt)
    {
        evt.RemoveAllListeners();
    }

    public static void WriteLuaMemoryInfoToFile(int type, string memorySnapshot)
    {
        if (type == 0)
        {
            Debug.Log(string.Format("lua 虚拟机占用总内存{0}kb", memorySnapshot));
            return;
        }
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
        string fileFolder = dirInfo.Parent + "/LuaMemorySnapShotFiles";
        if (!FileManager.DirectoryExist(fileFolder))
        {
            FileManager.CreateDirectory(fileFolder);
        }
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        string filePath = fileFolder + "/" + fileName;
        FileManager.WriteAllText(filePath, memorySnapshot);
    }
    public static void SetPosition(Transform trans, float x, float y, float z)
    {
        SetPosition(trans, x, y, z, false);
    }
    public static void SetPosition(Transform trans, float x, float y, float z, bool isLocal)
    {
        if (trans != null)
        {
            if (isLocal)
            {
                trans.localPosition = new Vector3(x, y, z);
            }
            else
            {
                trans.position = new Vector3(x, y, z);
            }
        }
    }

    //--localEulerAngles
    public static void SetLocalEulerAngles(Transform trans, float x, float y, float z)
    {
        if(trans != null)
        {
            trans.localEulerAngles = new Vector3(x, y, z);
        }
    }

    public static void SetEulerAngles(Transform trans, float x, float y, float z)
    {
        if (trans != null)
        {
            trans.eulerAngles = new Vector3(x, y, z);
        }
    }

    public static void SetRotation(Transform trans, float x, float y, float z, float w)
    {
        SetRotation(trans, x, y, z, w, false);
    }

    public static void SetRotation(Transform trans, float x, float y, float z, float w, bool isLocal)
    {
        if (trans != null)
        {
            if (isLocal)
            {
                trans.localRotation = new Quaternion(x, y, z, w);
            }
            else
            {
                trans.rotation = new Quaternion(x, y, z, w);
            }
        }
    }
    public static void SetPositionAndRotation(Transform trans, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        if (trans != null)
        {
            trans.SetPositionAndRotation(new Vector3(posX, posY, posZ), new Quaternion(rotX, rotX, rotZ, rotW));
        }
    }
    //替换字符串，lua的gsub有一些特殊字符无法进行替换，调用c#的string.replace来替换
    public static string ReplaceStr(string str,string targetStr,string replaceStr)
    {
        if (str != null)
        {
            return str.Replace(targetStr, replaceStr);
        }
        else
            return null;
    }

    #region Meshfilter 操作;
    public static void SetMeshFilterData(Transform trans, Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        MeshFilter tmpMeshFilter = trans.GetComponent<MeshFilter>();
        if (tmpMeshFilter == null)
        {
            return;
        }
        if (tmpMeshFilter.mesh == null)
        {
            tmpMeshFilter.mesh = new Mesh();
        }
        tmpMeshFilter.mesh.Clear();
        tmpMeshFilter.mesh.vertices = vertices;
        tmpMeshFilter.mesh.uv = uv;
        tmpMeshFilter.mesh.triangles = triangles;
        tmpMeshFilter.mesh.RecalculateNormals();
    }
    #endregion

    #region Dropdown
    public static void AddOptions(Dropdown dpd, string options)
    {
		if (dpd == null) {
			return;
		}
		if (string.IsNullOrEmpty (options)) {
			return;
		}
		string[] arr = options.Split (',');
		if (arr != null && arr.Length > 0) {
			List<string> lst = new List<string> ();
			for (int i = 0; i < arr.Length; i++) {
				lst.Add (arr[i]);
			}
			dpd.AddOptions(lst);
		}
    }
    #endregion
}