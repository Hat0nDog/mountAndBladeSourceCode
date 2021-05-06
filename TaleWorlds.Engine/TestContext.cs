// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TestContext
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public class TestContext
  {
    private AsyncRunner _asyncRunner;
    private AwaitableAsyncRunner _awaitableAsyncRunner;
    private Thread _asyncThread;
    private Task _asyncTask;

    public void RunTestAux(string commandLine)
    {
      TestCommonBase.BaseInstance.IsTestEnabled = true;
      MBDebug.TestModeEnabled = true;
      string[] strArray = commandLine.Split(' ');
      if (strArray.Length < 2)
      {
        MBDebug.ShowWarning("RunTextAux invalid commandLine!");
      }
      else
      {
        string asyncRunner = strArray[1];
        if (asyncRunner == "OpenSceneOnStartup")
          TestCommonBase.BaseInstance.SceneNameToOpenOnStartup = strArray[2];
        for (int index = 3; index < strArray.Length; ++index)
        {
          int result;
          int.TryParse(strArray[index], out result);
          TestCommonBase.BaseInstance.TestRandomSeed = result;
        }
        MBDebug.Print(nameof (commandLine) + commandLine);
        MBDebug.Print("p" + strArray.ToString());
        MBDebug.Print("Looking for test " + asyncRunner, color: Debug.DebugColor.Yellow);
        ConstructorInfo runnerConstructor = this.GetAsyncRunnerConstructor(asyncRunner);
        object obj = (object) null;
        if (runnerConstructor != (ConstructorInfo) null)
          obj = runnerConstructor.Invoke(new object[0]);
        this._asyncRunner = obj as AsyncRunner;
        this._awaitableAsyncRunner = obj as AwaitableAsyncRunner;
        if (this._asyncRunner != null)
        {
          this._asyncThread = new Thread((ThreadStart) (() => this._asyncRunner.Run()));
          this._asyncThread.Name = "ManagedAsyncThread";
          this._asyncThread.Start();
        }
        if (this._awaitableAsyncRunner == null)
          return;
        this._asyncTask = this._awaitableAsyncRunner.RunAsync();
      }
    }

    private ConstructorInfo GetAsyncRunnerConstructor(string asyncRunner)
    {
      foreach (Assembly asyncRunnerAssembly in this.GetAsyncRunnerAssemblies())
      {
        foreach (System.Type type in asyncRunnerAssembly.GetTypes())
        {
          if (type.Name == asyncRunner && (typeof (AsyncRunner).IsAssignableFrom(type) ? 1 : (typeof (AwaitableAsyncRunner).IsAssignableFrom(type) ? 1 : 0)) != 0)
          {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, new System.Type[0], (ParameterModifier[]) null);
            if (constructor != (ConstructorInfo) null)
              return constructor;
          }
        }
      }
      return (ConstructorInfo) null;
    }

    private Assembly[] GetAsyncRunnerAssemblies()
    {
      List<Assembly> assemblyList = new List<Assembly>();
      Assembly assembly1 = typeof (AsyncRunner).Assembly;
      foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (object referencedAssembly in assembly2.GetReferencedAssemblies())
        {
          if (referencedAssembly.ToString() == assembly1.GetName().ToString())
          {
            assemblyList.Add(assembly2);
            break;
          }
        }
      }
      return assemblyList.ToArray();
    }

    public void OnApplicationTick(float dt)
    {
      if (this._asyncTask == null || this._asyncTask.Status != TaskStatus.Faulted)
        return;
      string str1 = "TestRunTaskFailed\n";
      if (this._asyncTask.Exception.InnerException != null)
      {
        string str2 = str1 + this._asyncTask.Exception.InnerException.Message + "\n" + this._asyncTask.Exception.InnerException.StackTrace;
      }
      this._asyncTask = (Task) null;
      Utilities.DoDelayedexit(5);
    }

    public void TickTest(float dt)
    {
      if (this._asyncThread != null && this._asyncThread.IsAlive && this._asyncRunner != null)
        this._asyncRunner.SyncTick();
      if (this._awaitableAsyncRunner == null)
        return;
      this._awaitableAsyncRunner.OnTick(dt);
    }

    public void FinalizeContext()
    {
      if (this._asyncThread != null)
        this._asyncThread.Join();
      this._asyncThread = (Thread) null;
      this._asyncRunner = (AsyncRunner) null;
      this._awaitableAsyncRunner = (AwaitableAsyncRunner) null;
      this._asyncTask = (Task) null;
    }
  }
}
