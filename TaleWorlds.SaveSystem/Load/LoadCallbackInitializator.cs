// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.LoadCallbackInitializator
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
  internal class LoadCallbackInitializator
  {
    private ObjectHeaderLoadData[] _objectHeaderLoadDatas;
    private int _objectCount;
    private LoadData _loadData;

    public LoadCallbackInitializator(
      LoadData loadData,
      ObjectHeaderLoadData[] objectHeaderLoadDatas,
      int objectCount)
    {
      this._loadData = loadData;
      this._objectHeaderLoadDatas = objectHeaderLoadDatas;
      this._objectCount = objectCount;
    }

    public void InitializeObjects()
    {
      using (new PerformanceTestBlock("LoadContext::Callbacks"))
      {
        for (int i = 0; i < this._objectCount; ++i)
        {
          ObjectHeaderLoadData objectHeaderLoadData = this._objectHeaderLoadDatas[i];
          if (objectHeaderLoadData.Target != null)
          {
            IEnumerable<MethodInfo> initializationCallbacks = objectHeaderLoadData.TypeDefinition?.InitializationCallbacks;
            if (initializationCallbacks != null)
            {
              foreach (MethodInfo methodInfo in initializationCallbacks)
              {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (((IEnumerable<ParameterInfo>) parameters).Count<ParameterInfo>() > 1 && parameters[1].ParameterType == typeof (ObjectLoadData))
                {
                  ObjectLoadData loadData = LoadContext.CreateLoadData(this._loadData, i, objectHeaderLoadData);
                  methodInfo.Invoke(objectHeaderLoadData.Target, new object[2]
                  {
                    (object) this._loadData.MetaData,
                    (object) loadData
                  });
                }
                else
                  methodInfo.Invoke(objectHeaderLoadData.Target, new object[1]
                  {
                    (object) this._loadData.MetaData
                  });
              }
            }
          }
        }
      }
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }
}
