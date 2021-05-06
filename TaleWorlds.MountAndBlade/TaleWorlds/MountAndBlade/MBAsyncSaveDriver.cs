// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBAsyncSaveDriver
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MBAsyncSaveDriver : ISaveDriver
  {
    private MBSaveDriver _saveDriver;
    private Task _currentTask;

    public MBAsyncSaveDriver() => this._saveDriver = new MBSaveDriver();

    void ISaveDriver.SetFileName(string fileName)
    {
      this.WaitPreviousTask();
      this._saveDriver.SetFileName(fileName);
    }

    private void WaitPreviousTask()
    {
      Task currentTask = this._currentTask;
      if (currentTask == null || currentTask.IsCompleted)
        return;
      using (new PerformanceTestBlock("MBAsyncSaveDriver::Save - waiting previous save"))
        currentTask.Wait();
    }

    void ISaveDriver.Save(int version, MetaData metaData, GameData gameData)
    {
      this.WaitPreviousTask();
      using (new PerformanceTestBlock("MBAsyncSaveDriver::Save"))
        this._currentTask = Task.Run((Action) (() =>
        {
          using (new PerformanceTestBlock("MBAsyncSaveDriver::Save - Task itself"))
          {
            this._saveDriver.Save(version, metaData, gameData);
            this._currentTask = (Task) null;
          }
        }));
    }

    SaveGameFileInfo[] ISaveDriver.GetSaveGameFileInfos()
    {
      this.WaitPreviousTask();
      return this._saveDriver.GetSaveGameFileInfos();
    }

    string[] ISaveDriver.GetSaveGameFileNames()
    {
      this.WaitPreviousTask();
      return this._saveDriver.GetSaveGameFileNames();
    }

    MetaData ISaveDriver.LoadMetaData()
    {
      this.WaitPreviousTask();
      return this._saveDriver.LoadMetaData();
    }

    LoadData ISaveDriver.Load()
    {
      this.WaitPreviousTask();
      return this._saveDriver.Load();
    }

    void ISaveDriver.Delete(string fileName)
    {
      this.WaitPreviousTask();
      this._saveDriver.Delete(fileName);
    }
  }
}
