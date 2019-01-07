using System;
using System.Collections.Generic;


//ljfth:异步线程，处理耗时操作接口

//ljfth:异步任务处理结果

public class AsyncTaskResult
{
    //ljfth:如果在doInBackground中发生异常，那么Error!=null，在onPostExecute判断，并做相应处理
    public readonly Exception Error;
    //ljfth:如果在doInBackground中任务被取消，那么Cancel==true，在onPostExecute判断，并做相应处理
    public readonly bool Cancel;
    //ljfth:在doInBackground中任务处理的是个什么结果，用Result来表示，在onPostExecute中做判断
    public readonly int Result;

    public AsyncTaskResult(int result, Exception error, bool cancel)
    {
        Result = result;
        Error = error;
        Cancel = cancel;
    }
}
public delegate void onPreExecute();
public delegate void onProgressUpdate();
public delegate void onPostExecute();
public delegate void runInUIThread();

public class AsyncTaskInterfaceDelegate
{
    public AsyncTaskStates State
    {
        get;
        set;
    }
    public onPreExecute onPreExecute;
    public onProgressUpdate onProgressUpdate;
    public onPostExecute onPostExecute;
    public runInUIThread runInUIThread;
}

public delegate void OnPreExecute(AsyncTask asyncTask, List<object> args);
public delegate void DoInBackground(AsyncTask asyncTask, List<object> args);
public delegate void OnProgressUpdate(AsyncTask asyncTask, int progress, List<object> args);
public delegate void OnPostExecute(AsyncTask asyncTask, AsyncTaskResult asyncTaskResult, List<object> args);


