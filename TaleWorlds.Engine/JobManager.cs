// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.JobManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;

namespace TaleWorlds.Engine
{
  public class JobManager
  {
    private List<Job> _jobs;
    private object _locker;

    public JobManager()
    {
      this._jobs = new List<Job>();
      this._locker = new object();
    }

    public void AddJob(Job job)
    {
      lock (this._locker)
        this._jobs.Add(job);
    }

    internal void OnTick(float dt)
    {
      lock (this._locker)
      {
        for (int index = 0; index < this._jobs.Count; ++index)
        {
          Job job = this._jobs[index];
          job.DoJob(dt);
          if (job.Finished)
          {
            this._jobs.RemoveAt(index);
            --index;
          }
        }
      }
    }
  }
}
