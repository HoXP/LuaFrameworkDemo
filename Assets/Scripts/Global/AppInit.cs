using UnityEngine;

public class AppInit : MonoBehaviour
{
    internal static AppInit Instance = null;

    public static int _FPS = 60;
    private void Awake()
    {
        Instance = this;
        //这个lua在做一次
        Application.targetFrameRate = _FPS;
        //修改texture，保证图片分辨率，是full res，高清显示 >=QualityLevel.Fast 
        string[] names = QualitySettings.names;
        if (names != null && names.Length > 1)
        {
            QualitySettings.SetQualityLevel(1);
        }
    }
}