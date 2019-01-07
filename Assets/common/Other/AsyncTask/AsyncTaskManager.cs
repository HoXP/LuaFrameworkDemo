using System;
using System.Collections.Generic;

using UnityEngine;

public static class AsyncTaskManager
{
    internal static System.Object Locker = new System.Object();
    private static List<AsyncTaskInterfaceDelegate> ActiveTasks = new List<AsyncTaskInterfaceDelegate>();
    private static List<AsyncTaskInterfaceDelegate> RecycledTasks = new List<AsyncTaskInterfaceDelegate>();

    // Static constructor. Setup default values
    public static void Setup()
    {
        AsyncTaskDelegator.CheckInstance();
    }

    public static void AddTaskDelegate(AsyncTaskInterfaceDelegate Delegate)
    {
        if (Delegate != null)
        {
            lock (ActiveTasks)
            {
                ActiveTasks.Add(Delegate);
            }
        }
    }


    public static void OnUpdate()
    {
        // We will try to acquire a lock. If it fails, we will skip this frame without calling any callback.
        lock (ActiveTasks)
        {
            try
            {
                foreach (AsyncTaskInterfaceDelegate task in ActiveTasks)
                {
                    if (task.State != AsyncTaskStates.None)
                    {
                        Debug.Log("xxx :OnUpdate:" + task.State);
                    }

                    switch (task.State)
                    {
                        case AsyncTaskStates.Initial:
                            try
                            {
                                task.onPreExecute();
                            }
                            finally
                            {
                                task.State = AsyncTaskStates.None;
                            }

                            break;

                        case AsyncTaskStates.Progress:
                            try
                            {
                                task.onProgressUpdate();
                            }
                            finally
                            {
                                task.State = AsyncTaskStates.None;
                            }
                            break;

                        case AsyncTaskStates.Complete:
                            try
                            {
                                task.onPostExecute();
                            }
                            finally
                            {
                                task.State = AsyncTaskStates.None;
                                RecycledTasks.Add(task);
                            }
                            break;

                        case AsyncTaskStates.RunInUIThread:
                            try
                            {
                                task.runInUIThread();
                            }
                            finally
                            {
                                RecycledTasks.Add(task);
                            }
                            break;
                    }
                }
            }
            //这里必须catch，否则出现异常后，执行finally后，就不会再执行后面代码
            catch (Exception e)
            {
                Debug.Log("e==" + e);
            }
            finally
            {
            }
        }

        lock (RecycledTasks)
        {
            try
            {
                foreach (AsyncTaskInterfaceDelegate task in RecycledTasks)
                {
                    ActiveTasks.Remove(task);
                }
                RecycledTasks.Clear();
            }
            catch (Exception e)
            {
                Debug.Log("e==" + e);
            }
        }
    }

    public static void OnQuit()
    {
        lock (Locker)
        {

            OnUpdate();
        }
    }
}
