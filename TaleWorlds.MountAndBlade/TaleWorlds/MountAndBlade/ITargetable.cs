// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ITargetable
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public interface ITargetable
  {
    TargetFlags GetTargetFlags();

    float GetTargetValue(List<Vec3> referencePositions);

    GameEntity GetTargetEntity();

    BattleSideEnum GetSide();

    GameEntity Entity();
  }
}
