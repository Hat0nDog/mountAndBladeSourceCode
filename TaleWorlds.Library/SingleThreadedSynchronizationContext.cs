// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.SingleThreadedSynchronizationContext
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Library
{
  public sealed class SingleThreadedSynchronizationContext : SynchronizationContext
  {
    private readonly Queue<SingleThreadedSynchronizationContext.WorkRequest> _workQueue;
    private readonly int _mainThreadId;

    public SingleThreadedSynchronizationContext()
    {
      this._workQueue = new Queue<SingleThreadedSynchronizationContext.WorkRequest>(100);
      this._mainThreadId = Thread.CurrentThread.ManagedThreadId;
    }

    public override void Send(SendOrPostCallback callback, object state)
    {
      if (this._mainThreadId == Thread.CurrentThread.ManagedThreadId)
      {
        callback.DynamicInvokeWithLog(state);
      }
      else
      {
        using (ManualResetEvent waitHandle = new ManualResetEvent(false))
        {
          lock (this._workQueue)
            this._workQueue.Enqueue(new SingleThreadedSynchronizationContext.WorkRequest(callback, state, waitHandle));
          waitHandle.WaitOne();
        }
      }
    }

    public override void Post(SendOrPostCallback callback, object state)
    {
      SingleThreadedSynchronizationContext.WorkRequest workRequest = new SingleThreadedSynchronizationContext.WorkRequest(callback, state);
      lock (this._workQueue)
        this._workQueue.Enqueue(workRequest);
    }

    public void Tick()
    {
      Queue<SingleThreadedSynchronizationContext.WorkRequest> workRequestQueue = (Queue<SingleThreadedSynchronizationContext.WorkRequest>) null;
      lock (this._workQueue)
      {
        workRequestQueue = new Queue<SingleThreadedSynchronizationContext.WorkRequest>((IEnumerable<SingleThreadedSynchronizationContext.WorkRequest>) this._workQueue);
        this._workQueue.Clear();
      }
      while (workRequestQueue.Count > 0)
        workRequestQueue.Dequeue().Invoke();
    }

    private struct WorkRequest
    {
      private readonly SendOrPostCallback _callback;
      private readonly object _state;
      private readonly ManualResetEvent _waitHandle;

      public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
      {
        this._callback = callback;
        this._state = state;
        this._waitHandle = waitHandle;
      }

      public void Invoke()
      {
        this._callback.DynamicInvokeWithLog(this._state);
        if (this._waitHandle == null)
          return;
        this._waitHandle.Set();
      }
    }
  }
}
