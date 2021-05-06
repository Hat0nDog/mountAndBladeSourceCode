// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.DefaultObjectResolver
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.SaveSystem
{
  public class DefaultObjectResolver : IObjectResolver
  {
    bool IObjectResolver.CheckIfRequiresAdvancedResolving(object originalObject) => false;

    object IObjectResolver.ResolveObject(object originalObject) => originalObject;

    object IObjectResolver.AdvancedResolveObject(
      object originalObject,
      MetaData metaData,
      ObjectLoadData objectLoadData)
    {
      return originalObject;
    }
  }
}
