/********************************************************************
	File: 	TimeSvc.cs
	Author:	groundhog
	Time:	2025/2/26  11:10
	Description: 计时服务
*********************************************************************/

using System;
using System.Collections.Generic;

public class TimeSvc
{
    class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;
        }
    }

    private static TimeSvc instance = null;
    public static TimeSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimeSvc();
            }
            return instance;
        }
    }
    PETimer pt = null;
    Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly string tpQueLock = "tpQueLock";


    public void Init()
    {
        pt = new PETimer(100);
        tpQue.Clear();
        // 设置计时器工具日志
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });
        PECommon.Log("Init TimeSvc...");
        pt.SetHandle((Action<int> cb, int tid) =>
        {
            if (cb != null)
            {
                lock(tpQueLock)
                {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });
    }

    public void Update()
    {
        lock (tpQueLock)
        {
            TaskPack tp = null;
            if (tpQue.Count > 0)
            {
                tp = tpQue.Dequeue();
            }

            if (tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> cb, double delay, PETimeUnit timeUnit= PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(cb, delay, timeUnit, count);
    }
    
    public long GetNowTime()
    {
        return (long)pt.GetMillisecondsTime();
    }
}

