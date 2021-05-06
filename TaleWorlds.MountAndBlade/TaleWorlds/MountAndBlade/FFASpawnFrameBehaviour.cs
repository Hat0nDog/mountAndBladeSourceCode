// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FFASpawnFrameBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FFASpawnFrameBehaviour : SpawnFrameBehaviourBase
  {
    public override MatrixFrame GetSpawnFrame(
      Team team,
      bool hasMount,
      bool isInitialSpawn)
    {
      return this.GetSpawnFrameFromSpawnPoints((IList<GameEntity>) this.SpawnPoints.ToList<GameEntity>(), (Team) null, hasMount);
    }
  }
}
