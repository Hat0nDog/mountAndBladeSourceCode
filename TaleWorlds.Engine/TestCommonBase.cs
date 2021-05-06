// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TestCommonBase
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaleWorlds.Engine
{
  public abstract class TestCommonBase
  {
    public int TestRandomSeed;
    public bool IsTestEnabled;
    public bool isParallelThread;
    public string SceneNameToOpenOnStartup;
    public object TestLock = new object();
    private static TestCommonBase _baseInstance;
    private DateTime timeoutTimerStart = DateTime.Now;
    private bool timeoutTimerEnabled = true;
    private int commonWaitTimeoutLimits = 3000;

    public abstract void Tick();

    public static TestCommonBase BaseInstance => TestCommonBase._baseInstance;

    public void StartTimeoutTimer() => this.timeoutTimerStart = DateTime.Now;

    public void ToggleTimeoutTimer() => this.timeoutTimerEnabled = !this.timeoutTimerEnabled;

    public bool CheckTimeoutTimer() => this.timeoutTimerEnabled && DateTime.Now.Subtract(this.timeoutTimerStart).TotalSeconds > (double) this.commonWaitTimeoutLimits;

    protected TestCommonBase() => TestCommonBase._baseInstance = this;

    public void WaitFor(double seconds)
    {
      if (this.isParallelThread)
        return;
      DateTime now = DateTime.Now;
      while ((DateTime.Now - now).TotalSeconds < seconds)
      {
        Monitor.Pulse(this.TestLock);
        Monitor.Wait(this.TestLock);
      }
    }

    public Task WaitForAsync(double seconds, Random random) => Task.Delay((int) (seconds * 1000.0 * random.NextDouble()));

    public Task WaitForAsync(double seconds) => Task.Delay((int) (seconds * 1000.0));

    public static string GetAttachmentsFolderPath() => "..\\..\\..\\Tools\\TestAutomation\\Attachments\\";

    public virtual void OnFinalize() => TestCommonBase._baseInstance = (TestCommonBase) null;
  }
}
