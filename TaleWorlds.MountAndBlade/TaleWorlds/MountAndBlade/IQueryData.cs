// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IQueryData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public interface IQueryData
  {
    void Expire();

    void Evaluate(float currentTime);

    void SetSyncGroup(IEnumerable<IQueryData> syncGroup);
  }
}
