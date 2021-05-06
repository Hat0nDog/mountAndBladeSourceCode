// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.LoadResult
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.SaveSystem.Load
{
  public class LoadResult
  {
    private LoadCallbackInitializator _loadCallbackInitializator;

    public object Root { get; private set; }

    public bool Successful { get; private set; }

    public LoadError[] Errors { get; private set; }

    public MetaData MetaData { get; private set; }

    private LoadResult()
    {
    }

    internal static LoadResult CreateSuccessful(
      object root,
      MetaData metaData,
      LoadCallbackInitializator loadCallbackInitializator)
    {
      return new LoadResult()
      {
        Root = root,
        Successful = true,
        MetaData = metaData,
        _loadCallbackInitializator = loadCallbackInitializator
      };
    }

    internal static LoadResult CreateFailed(IEnumerable<LoadError> errors) => new LoadResult()
    {
      Successful = false,
      Errors = errors.ToArray<LoadError>()
    };

    public void InitializeObjects() => this._loadCallbackInitializator.InitializeObjects();
  }
}
