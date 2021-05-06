// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ApplicationHealthChecker
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public class ApplicationHealthChecker
  {
    private Thread _thread;
    private bool _isRunning;
    private Stopwatch _stopwatch;
    private Thread _mainThread;

    public void Start()
    {
      TaleWorlds.Library.Debug.Print("Starting ApplicationHealthChecker");
      try
      {
        File.WriteAllText(BasePath.Name + "Application.HealthCheckerStarted", "...");
      }
      catch (Exception ex)
      {
        ApplicationHealthChecker.Print("Blocked main thread file create e: " + (object) ex);
      }
      this._isRunning = true;
      this._stopwatch = new Stopwatch();
      this._stopwatch.Start();
      this._thread = new Thread(new ThreadStart(this.ThreadUpdate));
      this._thread.IsBackground = true;
      this._thread.Start();
    }

    public void Stop()
    {
      this._thread = (Thread) null;
      this._stopwatch = (Stopwatch) null;
      this._isRunning = false;
    }

    public void Update()
    {
      if (!this._isRunning)
        return;
      this._stopwatch.Restart();
      this._mainThread = Thread.CurrentThread;
    }

    private static void Print(string log)
    {
      TaleWorlds.Library.Debug.Print(log);
      Console.WriteLine(log);
    }

    private void ThreadUpdate()
    {
      while (this._isRunning)
      {
        long num = this._stopwatch.ElapsedMilliseconds / 1000L;
        if (num > 180L)
        {
          ApplicationHealthChecker.Print("Main thread is blocked for " + (object) num + " seconds");
          try
          {
            File.WriteAllText(BasePath.Name + "Application.Blocked", num.ToString());
          }
          catch (Exception ex)
          {
            ApplicationHealthChecker.Print("Blocked main thread file create e: " + (object) ex);
          }
          try
          {
            ApplicationHealthChecker.Print("Blocked main thread IsAlive: " + this._mainThread.IsAlive.ToString());
            ApplicationHealthChecker.Print("Blocked main thread ThreadState: " + (object) this._mainThread.ThreadState);
          }
          catch (Exception ex)
          {
            ApplicationHealthChecker.Print("Blocked main thread e: " + (object) ex);
          }
        }
        else
        {
          try
          {
            if (File.Exists(BasePath.Name + "Application.Blocked"))
              File.Delete(BasePath.Name + "Application.Blocked");
          }
          catch (Exception ex)
          {
            ApplicationHealthChecker.Print("Blocked main thread file delete e: " + (object) ex);
          }
        }
        Thread.Sleep(10000);
      }
    }
  }
}
