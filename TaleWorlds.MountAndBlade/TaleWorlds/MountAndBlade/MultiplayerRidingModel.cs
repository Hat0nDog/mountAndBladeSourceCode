﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerRidingModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  internal class MultiplayerRidingModel : RidingModel
  {
    public override float CalculateAcceleration(
      in EquipmentElement mountElement,
      in EquipmentElement harnessElement,
      int ridingSkill)
    {
      float num = (float) mountElement.GetModifiedMountManeuver(in harnessElement) * 0.008f;
      if (ridingSkill >= 0)
        num *= (float) (0.699999988079071 + 3.0 / 1000.0 * ((double) ridingSkill - 1.5 * (double) mountElement.Item.Difficulty));
      return MathF.Clamp(num, 0.15f, 0.7f);
    }
  }
}