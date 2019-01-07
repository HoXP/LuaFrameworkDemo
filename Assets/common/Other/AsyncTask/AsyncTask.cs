using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//ljfth:异步线程，处理耗时操作

public class AsyncTask
{
    #region private variables
    private List<object> mArgs;
    private Exception mError;
    private bool mIsBusy = false;
    private AsyncTaskInterfaceDelegate mAsyncTaskInterfaceDelegate = new AsyncTaskInterfaceDelegate();
    private int mProgress;
    private bool mCancel = false;
    #endregion
    private int _Result;
    public int Result
    {
        get
        {
            return _Result;
        }
    }
    public void SetResult(int value)
    {
        lock (this)
        {
            _Result = value;
        }
    }
    public OnPreExecute _OnPreExecute;
    public DoInBackground _DoInBackground;
    public OnProgressUpdate _OnProgressUpdate;
    public OnPostExecute _OnPostExecute;

    static AsyncTask()
    {
        AsyncTaskManager.Setup();
    }

    public static void runInUIThread(runInUIThread method)
    {
        new AsyncTask(method).executeToUIThread();
    }

    public AsyncTask()
    {
        mAsyncTaskInterfaceDelegate.onPreExecute = onPreExecute;
        mAsyncTaskInterfaceDelegate.onProgressUpdate = onProgressUpdate;
        mAsyncTaskInterfaceDelegate.onPostExecute = onPostExecute;
    }

    public bool execute(List<object> args)
    {
        lock (this)
        {
            if (!mIsBusy)
            {
                mIsBusy = true;
                mArgs = args;
                mAsyncTaskInterfaceDelegate.State = AsyncTaskStates.Initial;
                AsyncTaskManager.AddTaskDelegate(mAsyncTaskInterfaceDelegate);
                //ljfth:默认大概60个线程，如果同一时间执行的线程过都，QueueUserWorkItem会自动给排队等候
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadFunc));
                return true;
            }
        }
        return false;
    }

    public void executeToUIThread()
    {
        lock (this)
        {
            mAsyncTaskInterfaceDelegate.State = AsyncTaskStates.RunInUIThread;
            AsyncTaskManager.AddTaskDelegate(mAsyncTaskInterfaceDelegate);
        }
    }

    //ljfth:通知任务当前进度，需要自己在doInBackground里面主动调用，告知进度
    public void publishProgress(int progress)
    {
        mAsyncTaskInterfaceDelegate.State = AsyncTaskStates.Progress;
        mProgress = progress;
    }


    #region private method
    private AsyncTask(runInUIThread method)
    {
        mAsyncTaskInterfaceDelegate.runInUIThread = method;
    }

    private void onPreExecute()
    {
        if (_OnPreExecute != null)
        {
            _OnPreExecute(this, mArgs);
        }
    }

    private void doInBackground()
    {
        if (_DoInBackground != null)
        {
            _DoInBackground(this, mArgs);
        }
    }

    private void onProgressUpdate()
    {
        if (_OnProgressUpdate != null)
        {
            _OnProgressUpdate(this, mProgress, mArgs);
        }
    }

    private void onPostExecute()
    {
        if (_OnPostExecute != null)
        {
            _OnPostExecute(this, new AsyncTaskResult(Result, mError, mCancel), mArgs);
        }
        mIsBusy = false;
    }

    private void ThreadFunc(object param)
    {
        while (true)
        {
            if (mAsyncTaskInterfaceDelegate.State == AsyncTaskStates.Initial)
            {
                System.Threading.Thread.Sleep(10);
            }
            else
            {
                break;
            }
        }

        try
        {
            if (_DoInBackground != null)
            {
                _DoInBackground(this, mArgs);
            }
        }
        catch (Exception e)
        {
            Debug.Log("xxx asynctask eeeee===" + e);
            mError = e;
        }
        finally
        {
            mAsyncTaskInterfaceDelegate.State = AsyncTaskStates.Complete;
        }
    }
    #endregion
}
