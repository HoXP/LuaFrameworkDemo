
using UnityEngine;

public class FPSInfo : MonoBehaviour
{
    public float _updateRateTime = 1f;          //更新帧显示帧数的时间
    private float _lastUpdateShowTime = 0f;         //上次更新频率的时间
    private int _frameCount;                      //每次统计时间之间运行多少针
    private float _finalFPS;                        //实际帧数

    // Use this for initialization
    void Start()
    {
        _lastUpdateShowTime = Time.realtimeSinceStartup;
        _frameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _frameCount++;
        if (Time.realtimeSinceStartup - _lastUpdateShowTime >= _updateRateTime)
        {
            _finalFPS = _frameCount / (Time.realtimeSinceStartup - _lastUpdateShowTime);
            _lastUpdateShowTime = Time.realtimeSinceStartup;
            _frameCount = 0;
        }
    }

    void OnGUI()
    {
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;
        fontStyle.normal.textColor = new Color(1, 0, 0);
        fontStyle.alignment = TextAnchor.UpperRight;
        fontStyle.fontSize = 35;

        GUI.Label(new Rect(Screen.width - 200, 0, 100, 100), "FPS: " + string.Format("{0:f2}", _finalFPS), fontStyle);
        GUI.Label(new Rect(Screen.width - 200, 40, 100, 100), "FPSTarget: " + string.Format("{0:f2}", Application.targetFrameRate), fontStyle);
        fontStyle.fontSize = 25;
        GUI.Label(new Rect(Screen.width - 200, 80, 100, 100), "DeviceInfo : " + SystemInfo.deviceModel.ToLower(), fontStyle);
    }
}