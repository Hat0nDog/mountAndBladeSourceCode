// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBSaveLoad
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.Core
{
  public static class MBSaveLoad
  {
    private const int MaxNumberOfAutoSaveFiles = 3;
    private static ISaveDriver _saveDriver = (ISaveDriver) new FileDriver();
    private static int AutoSaveIndex = 0;
    private static string DefaultSaveGamePrefix = "save_";
    private static string AutoSaveNamePrefix = MBSaveLoad.DefaultSaveGamePrefix + "auto_";
    private static string ActiveSaveSlotName = (string) null;
    private static bool DoNotShowSaveErrorAgain = false;

    private static string GetAutoSaveName() => MBSaveLoad.AutoSaveNamePrefix + (object) MBSaveLoad.AutoSaveIndex;

    private static void IncrementAutoSaveIndex()
    {
      ++MBSaveLoad.AutoSaveIndex;
      if (MBSaveLoad.AutoSaveIndex <= 3)
        return;
      MBSaveLoad.AutoSaveIndex = 1;
    }

    private static void InitializeAutoSaveIndex(string saveName)
    {
      string str = "";
      if (saveName.Contains(MBSaveLoad.AutoSaveNamePrefix))
      {
        str = saveName;
      }
      else
      {
        foreach (string saveFileName in MBSaveLoad.GetSaveFileNames())
        {
          if (saveFileName.Contains(MBSaveLoad.AutoSaveNamePrefix))
          {
            str = saveFileName;
            break;
          }
        }
      }
      if (str.IsStringNoneOrEmpty())
      {
        MBSaveLoad.AutoSaveIndex = 1;
      }
      else
      {
        string[] strArray = str.Split('_');
        int result;
        if (strArray.Length != 3 || !int.TryParse(strArray[strArray.Length - 1], out result) || (result <= 0 || result > 3))
          return;
        MBSaveLoad.AutoSaveIndex = result;
      }
    }

    public static void SetSaveDriver(ISaveDriver saveDriver) => MBSaveLoad._saveDriver = saveDriver;

    public static SaveGameFileInfo[] GetSaveFiles()
    {
      SaveGameFileInfo[] saveGameFileInfos = MBSaveLoad._saveDriver.GetSaveGameFileInfos();
      List<SaveGameFileInfo> source = new List<SaveGameFileInfo>();
      foreach (SaveGameFileInfo saveGameFileInfo in saveGameFileInfos)
      {
        if (saveGameFileInfo.MetaData.GetApplicationVersion() != ApplicationVersion.Empty)
          source.Add(saveGameFileInfo);
      }
      return source.OrderByDescending<SaveGameFileInfo, DateTime>((Func<SaveGameFileInfo, DateTime>) (info => info.MetaData.GetCreationTime())).ToArray<SaveGameFileInfo>();
    }

    public static string[] GetSaveFileNames() => MBSaveLoad._saveDriver.GetSaveGameFileNames();

    public static LoadGameResult LoadSaveGameData(
      string saveName,
      string[] loadedModuleNames)
    {
      List<ModuleInfo> modules = MBSaveLoad.GetModules(loadedModuleNames);
      MBSaveLoad.InitializeAutoSaveIndex(saveName);
      string fileName = saveName + ".sav";
      ISaveDriver driver = MBSaveLoad._saveDriver;
      driver.SetFileName(fileName);
      ApplicationVersion applicationVersion = driver.LoadMetaData().GetApplicationVersion();
      if (applicationVersion.Major <= 1 && applicationVersion.Minor <= 4 && applicationVersion.Revision < 2)
      {
        driver = (ISaveDriver) new OldFileDriver();
        driver.SetFileName(fileName);
      }
      LoadResult loadResult = SaveManager.Load(driver, true);
      if (loadResult.Successful)
      {
        MBSaveLoad.ActiveSaveSlotName = !MBSaveLoad.IsSaveFileNameReserved(saveName) ? saveName : (string) null;
        return new LoadGameResult(loadResult, MBSaveLoad.CheckModules(loadResult.MetaData, modules));
      }
      Debug.Print("Error: Could not load the game!");
      return (LoadGameResult) null;
    }

    public static bool QuickSaveCurrentGame(CampaignSaveMetaDataArgs campaignMetaData)
    {
      MetaData saveMetaData = MBSaveLoad.GetSaveMetaData(campaignMetaData);
      if (MBSaveLoad.ActiveSaveSlotName == null)
        MBSaveLoad.ActiveSaveSlotName = MBSaveLoad.GetNextAvailableSaveName();
      string activeSaveSlotName = MBSaveLoad.ActiveSaveSlotName;
      return MBSaveLoad.OverwriteSaveFile(saveMetaData, activeSaveSlotName);
    }

    public static bool AutoSaveCurrentGame(CampaignSaveMetaDataArgs campaignMetaData)
    {
      MetaData saveMetaData = MBSaveLoad.GetSaveMetaData(campaignMetaData);
      MBSaveLoad.IncrementAutoSaveIndex();
      string autoSaveName = MBSaveLoad.GetAutoSaveName();
      return MBSaveLoad.OverwriteSaveFile(saveMetaData, autoSaveName);
    }

    public static bool SaveAsCurrentGame(CampaignSaveMetaDataArgs campaignMetaData, string saveName)
    {
      int num = MBSaveLoad.OverwriteSaveFile(MBSaveLoad.GetSaveMetaData(campaignMetaData), saveName) ? 1 : 0;
      if (num == 0)
        return num != 0;
      MBSaveLoad.ActiveSaveSlotName = saveName;
      return num != 0;
    }

    public static void DeleteSaveGame(string saveName)
    {
      string fileName = saveName + ".sav";
      MBSaveLoad._saveDriver.Delete(fileName);
    }

    public static void OnNewGame()
    {
      MBSaveLoad.ActiveSaveSlotName = (string) null;
      MBSaveLoad.AutoSaveIndex = 0;
    }

    public static bool IsSaveFileNameReserved(string name)
    {
      for (int index = 1; index <= 3; ++index)
      {
        if (name == MBSaveLoad.AutoSaveNamePrefix + (object) index)
          return true;
      }
      return false;
    }

    private static string GetNextAvailableSaveName()
    {
      uint num1 = 0;
      foreach (string saveFileName in MBSaveLoad.GetSaveFileNames())
      {
        uint result;
        if (saveFileName.StartsWith(MBSaveLoad.DefaultSaveGamePrefix) && uint.TryParse(saveFileName.Substring(MBSaveLoad.DefaultSaveGamePrefix.Length), out result) && result > num1)
          num1 = result;
      }
      uint num2 = num1 + 1U;
      return MBSaveLoad.DefaultSaveGamePrefix + num2.ToString("000");
    }

    private static bool OverwriteSaveFile(MetaData metaData, string name)
    {
      bool flag;
      try
      {
        flag = MBSaveLoad.SaveGame(name, metaData);
      }
      catch
      {
        flag = false;
      }
      if (flag)
        return true;
      if (!MBSaveLoad.DoNotShowSaveErrorAgain)
        InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_save_unsuccessful_title").ToString(), GameTexts.FindText("str_save_unsuccessful_cannot_create_save_data").ToString(), true, false, GameTexts.FindText("str_ok").ToString(), GameTexts.FindText("str_dont_show_again").ToString(), (Action) null, (Action) (() => MBSaveLoad.DoNotShowSaveErrorAgain = true)));
      return false;
    }

    private static bool SaveGame(string saveName, MetaData metaData)
    {
      string fileName = saveName + ".sav";
      ISaveDriver saveDriver = MBSaveLoad._saveDriver;
      saveDriver.SetFileName(fileName);
      bool flag = false;
      try
      {
        flag = Game.Current.Save(metaData, saveDriver);
      }
      catch (Exception ex)
      {
        Debug.Print("Unable to create save game data");
        Debug.Print(ex.Message);
        Debug.SilentAssert(ModuleHelper.GetModules().Any<ModuleInfo>((Func<ModuleInfo, bool>) (m => !m.IsOfficial)), ex.Message, callerFile: "C:\\Develop\\mb3\\Source\\Bannerlord\\TaleWorlds.Core\\MBSaveLoad.cs", callerMethod: nameof (SaveGame), callerLine: 334);
      }
      return flag;
    }

    private static List<ModuleCheckResult> CheckModules(
      MetaData fileMetaData,
      List<ModuleInfo> loadedModules)
    {
      string[] modulesInSaveFile = fileMetaData.GetModules();
      List<ModuleCheckResult> moduleCheckResultList = new List<ModuleCheckResult>();
      foreach (string str in modulesInSaveFile)
      {
        string moduleName = str;
        if (loadedModules.All<ModuleInfo>((Func<ModuleInfo, bool>) (loadedModule => loadedModule.Name != moduleName)))
          moduleCheckResultList.Add(new ModuleCheckResult(moduleName, ModuleCheckResultType.ModuleRemoved));
        else if (!fileMetaData.GetModuleVersion(moduleName).IsSame(loadedModules.Single<ModuleInfo>((Func<ModuleInfo, bool>) (loadedModule => loadedModule.Name == moduleName)).Version))
          moduleCheckResultList.Add(new ModuleCheckResult(moduleName, ModuleCheckResultType.VersionMismatch));
      }
      foreach (ModuleInfo moduleInfo in loadedModules.Where<ModuleInfo>((Func<ModuleInfo, bool>) (loadedModule => ((IEnumerable<string>) modulesInSaveFile).All<string>((Func<string, bool>) (moduleName => loadedModule.Name != moduleName)))))
        moduleCheckResultList.Add(new ModuleCheckResult(moduleInfo.Name, ModuleCheckResultType.ModuleAdded));
      return moduleCheckResultList;
    }

    private static List<ModuleInfo> GetModules(string[] moduleNames)
    {
      List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();
      for (int index = 0; index < moduleNames.Length; ++index)
      {
        ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(moduleNames[index]);
        moduleInfoList.Add(moduleInfo);
      }
      return moduleInfoList;
    }

    private static MetaData GetSaveMetaData(CampaignSaveMetaDataArgs data)
    {
      MetaData metaData = new MetaData();
      List<ModuleInfo> modules = MBSaveLoad.GetModules(data.ModuleNames);
      metaData["Modules"] = string.Join(";", modules.Select<ModuleInfo, string>((Func<ModuleInfo, string>) (q => q.Name)));
      foreach (ModuleInfo moduleInfo in modules)
        metaData["Module_" + moduleInfo.Name] = moduleInfo.Version.ToString();
      metaData.Add("ApplicationVersion", ApplicationVersion.FromParametersFile(ApplicationVersionGameType.Singleplayer).ToString());
      metaData.Add("CreationTime", DateTime.Now.ToString());
      foreach (KeyValuePair<string, string> keyValuePair in data.OtherData)
        metaData.Add(keyValuePair.Key, keyValuePair.Value);
      return metaData;
    }
  }
}
