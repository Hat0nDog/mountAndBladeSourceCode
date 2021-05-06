// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.SaveManager
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Save;

namespace TaleWorlds.SaveSystem
{
  public static class SaveManager
  {
    public const string SaveFileExtension = "sav";
    private const int CurrentVersion = 1;
    private static DefinitionContext _definitionContext;

    public static void InitializeGlobalDefinitionContext()
    {
      SaveManager._definitionContext = new DefinitionContext();
      SaveManager._definitionContext.FillWithCurrentTypes();
    }

    public static SaveOutput Save(object target, MetaData metaData, ISaveDriver driver)
    {
      if (SaveManager._definitionContext == null)
        SaveManager.InitializeGlobalDefinitionContext();
      if (SaveManager._definitionContext.GotError)
      {
        List<SaveError> saveErrorList = new List<SaveError>();
        foreach (string error in SaveManager._definitionContext.Errors)
          saveErrorList.Add(new SaveError(error));
        return SaveOutput.CreateFailed((IEnumerable<SaveError>) saveErrorList);
      }
      using (new PerformanceTestBlock("Save Context"))
      {
        Debug.Print("Saving with new context");
        SaveContext saveContext = new SaveContext(SaveManager._definitionContext);
        if (saveContext.Save(target, metaData))
        {
          try
          {
            driver.Save(1, metaData, saveContext.SaveData);
            return SaveOutput.CreateSuccessful(saveContext.SaveData);
          }
          catch (Exception ex)
          {
            return SaveOutput.CreateFailed((IEnumerable<SaveError>) new SaveError[1]
            {
              new SaveError(ex.Message)
            });
          }
        }
        else
          return SaveOutput.CreateFailed((IEnumerable<SaveError>) new SaveError[1]
          {
            new SaveError("Not implemented")
          });
      }
    }

    public static MetaData LoadMetaData(ISaveDriver driver) => driver.LoadMetaData();

    public static LoadResult Load(ISaveDriver driver) => SaveManager.Load(driver, false);

    public static LoadResult Load(ISaveDriver driver, bool loadAsLateInitialize)
    {
      DefinitionContext definitionContext = new DefinitionContext();
      definitionContext.FillWithCurrentTypes();
      LoadContext loadContext = new LoadContext(definitionContext, driver);
      LoadData loadData = driver.Load();
      LoadResult loadResult;
      if (loadContext.Load(loadData, loadAsLateInitialize))
      {
        LoadCallbackInitializator loadCallbackInitializator = (LoadCallbackInitializator) null;
        if (loadAsLateInitialize)
          loadCallbackInitializator = loadContext.CreateLoadCallbackInitializator(loadData);
        loadResult = LoadResult.CreateSuccessful(loadContext.RootObject, loadData.MetaData, loadCallbackInitializator);
      }
      else
        loadResult = LoadResult.CreateFailed((IEnumerable<LoadError>) new LoadError[1]
        {
          new LoadError("Not implemented")
        });
      return loadResult;
    }
  }
}
