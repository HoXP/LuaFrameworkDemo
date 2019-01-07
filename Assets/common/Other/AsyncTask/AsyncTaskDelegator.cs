using UnityEngine;


    [ExecuteInEditMode]
    public sealed class AsyncTaskDelegator : MonoBehaviour
    {

        public static bool IsCreated { get; private set; }
        public static AsyncTaskDelegator Instance { get; private set; }
        public static int ThreadFrequencyInMS { get; set; }
        public static OnCallBackBool OnApplicationForegroundStateChanged;


        static AsyncTaskDelegator()
        {
            ThreadFrequencyInMS = 100;
        }


        public static void CheckInstance()
        {
            try
            {
                if (!IsCreated)
                {
                    GameObject go = GameObject.Find("AsyncTaskDelegator");

                    if (go != null)
                    {
                        Instance = go.GetComponent<AsyncTaskDelegator>();
                    }


                    if (Instance == null)
                    {
                        go = new GameObject("AsyncTaskDelegator");
                        go.hideFlags = HideFlags.DontSave;

                        Instance = go.AddComponent<AsyncTaskDelegator>();
                        DontDestroyOnLoad(go);
                    }

                    IsCreated = true;
                }
            }
            catch
            {
                Debug.Log("AsyncTaskDelegator : Please call the AsyncTaskDelegator before it create!");
            }
        }

        void Update()
        {
            AsyncTaskManager.OnUpdate();
        }


        void OnDisable()
        {
             OnApplicationQuit();
        }

        void OnApplicationPause(bool isPaused)
        {
            if (AsyncTaskDelegator.OnApplicationForegroundStateChanged != null)
            AsyncTaskDelegator.OnApplicationForegroundStateChanged(isPaused);
        }

        void OnApplicationQuit()
        {
            if (!IsCreated)
                return;
            IsCreated = false;
            AsyncTaskManager.OnQuit();
        }
    }
