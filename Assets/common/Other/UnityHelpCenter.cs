using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityHelpCenter : MonoBehaviour
{
    private static volatile UnityHelpCenter ms_Instance;
    public static UnityHelpCenter Instance
    {
        get
        {
            return ms_Instance;
        }
    }

    private class ResCallBackData
    {
        public OnCallBackSObject callBack;
        public string filePath;
    }
    private Dictionary<string, List<ResCallBackData>> objectCallBackDic = new Dictionary<string, List<ResCallBackData>>();
    private Dictionary<string, OnCallBackWWW> wwwCallBackDic = new Dictionary<string, OnCallBackWWW>();
    private Dictionary<string, OnCallBackAudioClip> wwwAudioClipCallBackDic = new Dictionary<string, OnCallBackAudioClip>();
    private Dictionary<string, Coroutine> coroutinesDic = new Dictionary<string, Coroutine>();
    private void Awake()
    {
        ms_Instance = this;
    }
    public void LoadResourceAsync(string fileName, OnCallBackSObject callBack)
    {
        LoadResourceAsync(fileName, callBack, null);
    }

    public void LoadResourceAsync(string filePath, OnCallBackSObject callBack, System.Type systemTypeInstance)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            if (callBack != null)
            {
                callBack(null, null);
            }
            return;
        }
        if(objectCallBackDic.ContainsKey(filePath) == false)
        {
            objectCallBackDic[filePath] = new List<ResCallBackData>();
        }
        //是否正在任务中
        bool isTaskRunning = objectCallBackDic[filePath].Count > 0;

        ResCallBackData data = new ResCallBackData();
        data.filePath = filePath;
        data.callBack = callBack;

        objectCallBackDic[filePath].Add(data);
        if(isTaskRunning == false)
        {
            StartCoroutine(OnLoadResourceAsync(filePath, systemTypeInstance));
        }
    }

    IEnumerator OnLoadResourceAsync(string fileName, System.Type systemTypeInstance)
    {
        ResourceRequest request = null;
        if (systemTypeInstance == null)
        {
            request = Resources.LoadAsync(fileName);
        }
        else
        {
            request = Resources.LoadAsync(fileName, systemTypeInstance);
        }
        yield return request;
        if (objectCallBackDic != null && objectCallBackDic.ContainsKey(fileName))
        {
            var list = objectCallBackDic[fileName];
            int count = list.Count;
            for(int i = 0; i < count;i++)
            {
                var item = list[i];
                if (item != null && item.callBack != null)
                {
                    item.callBack(fileName, request.asset);
                    objectCallBackDic.Remove(fileName);
                }
            }
        }
    }

    public void LoadAssetAsync(AssetBundle assetBundle, string fileName, OnCallBackSObject callBack)
    {
        LoadAssetAsync(assetBundle, fileName, callBack, null);
    }
    
    public void LoadAssetAsync(AssetBundle assetBundle, string filePath, OnCallBackSObject callBack, System.Type systemTypeInstance)
    {
        if(assetBundle == null || string.IsNullOrEmpty(filePath))
        {
            if(callBack != null)
            {
                callBack(null, null);
            }
            return;
        }
        if (objectCallBackDic.ContainsKey(filePath) == false)
        {
            objectCallBackDic[filePath] = new List<ResCallBackData>();
        }
        //是否正在任务中
        bool isTaskRunning = objectCallBackDic[filePath].Count > 0;

        ResCallBackData data = new ResCallBackData();
        data.filePath = filePath;
        data.callBack = callBack;

        objectCallBackDic[filePath].Add(data);
        if(isTaskRunning == false)
        {
            StartCoroutine(OnLoadAssetAsync(assetBundle, filePath, systemTypeInstance));
        }
    }

    IEnumerator OnLoadAssetAsync(AssetBundle assetBundle, string fileName, System.Type systemTypeInstance)
    {
        AssetBundleRequest request = null;
        if (systemTypeInstance == null)
        {
            request = assetBundle.LoadAssetAsync(fileName);
        }
        else
        {
            request = assetBundle.LoadAssetAsync(fileName, systemTypeInstance);
        }
        yield return request;
        if(objectCallBackDic != null && objectCallBackDic.ContainsKey(fileName))
        {
            var list = objectCallBackDic[fileName];
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var item = list[i];
                if (item != null && item.callBack != null)
                {
                    item.callBack(fileName, request.asset);
                    objectCallBackDic.Remove(fileName);
                }
            }
        }
    }

    public void StopWWW(string path)
    {
        if(path != null && coroutinesDic != null && coroutinesDic.ContainsKey(path))
        {
            Coroutine item = coroutinesDic[path];
            if(item != null)
            {
                StopCoroutine(item);
            }
        }
        if(path != null && wwwCallBackDic != null && wwwCallBackDic.ContainsKey(path))
        {
            wwwCallBackDic.Remove(path);
        }
        if (path != null && wwwAudioClipCallBackDic != null && wwwAudioClipCallBackDic.ContainsKey(path))
        {
            wwwAudioClipCallBackDic.Remove(path);
        }
    }

    public void LoadWWW(string path, OnCallBackWWW callBack)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (callBack != null)
            {
                callBack(null);
            }
            return;
        }
        wwwCallBackDic[path] = callBack;
        coroutinesDic[path] = StartCoroutine(OnLoadWWW(path));
    }

    public void LoadWWWAudio(string path, OnCallBackAudioClip callBack)
    {
        LoadWWWAudio(path, callBack, false, AudioType.MPEG);
    }
    public void LoadWWWAudio(string path, OnCallBackAudioClip callBack, bool threeD)
    {
        LoadWWWAudio(path, callBack, threeD, AudioType.MPEG);
    }
    public void LoadWWWAudio(string path, OnCallBackAudioClip callBack, bool threeD, AudioType audioType)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (callBack != null)
            {
                callBack(null);
            }
            return;
        }
        wwwAudioClipCallBackDic[path] = callBack;
        coroutinesDic[path] = StartCoroutine(OnLoadWWWAudio(path, threeD, audioType));
    }

    IEnumerator OnLoadWWWAudio(string path,bool threeD, AudioType audioType)
    {
        WWW www = new WWW(path);
        yield return www;
        AudioClip data = www.GetAudioClipCompressed(threeD, audioType);
        if (wwwAudioClipCallBackDic != null && wwwAudioClipCallBackDic.ContainsKey(path))
        {
            OnCallBackAudioClip callBack = wwwAudioClipCallBackDic[path];
            if (callBack != null)
            {
                callBack(data);
                wwwAudioClipCallBackDic.Remove(path);
                www.Dispose();
            }
        }
    }

    IEnumerator OnLoadWWW(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        if (wwwCallBackDic != null && wwwCallBackDic.ContainsKey(path))
        {
            OnCallBackWWW callBack = wwwCallBackDic[path];
            if (callBack != null)
            {
                if (www.bytes == null || www.bytes.Length == 0)
                {
                    callBack(null);
                }
                else
                {
                    callBack(www);
                }
                wwwCallBackDic.Remove(path);
                www.Dispose();
            }
        }
    }

    private int coroutineIndex = 0;
    private Dictionary<int, OnCallBack> coroutineDic = new Dictionary<int, OnCallBack>();
    public void StartWaitForSecond(float second, OnCallBack callBack)
    {
        coroutineIndex++;
        coroutineDic[coroutineIndex] = callBack;
        StartCoroutine(WaitForSecond(second, coroutineIndex));
    }

    public void StartWaitForNextFrame(OnCallBack callBack)
    {
        coroutineIndex++;
        coroutineDic[coroutineIndex] = callBack;
        StartCoroutine(WaitForNextFrame(coroutineIndex));
    }
    IEnumerator WaitForNextFrame(int key)
    {
        yield return null;
        if (coroutineDic.ContainsKey(key))
        {
            coroutineDic[key]();
            coroutineDic.Remove(key);
        }
    }

    IEnumerator WaitForSecond(float second, int key)
    {
        yield return new WaitForSeconds(second);
        if(coroutineDic.ContainsKey(key))
        {
            coroutineDic[key]();
            coroutineDic.Remove(key);
        }
    }

    public void Clear()
    {
        objectCallBackDic.Clear();
        wwwCallBackDic.Clear();
        wwwAudioClipCallBackDic.Clear();
        coroutinesDic.Clear();
        coroutineDic.Clear();
        OnStopAllCoroutines();
    }

    public void OnStopAllCoroutines()
    {
        StopAllCoroutines();
    }
}
