// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IOnSpawnPerkEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public interface IOnSpawnPerkEffect
  {
    float GetTroopCountMultiplier();

    float GetExtraTroopCount();

    List<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
      bool isPlayer,
      List<(EquipmentIndex, EquipmentElement)> alternativeEquipments);

    float GetDrivenPropertyBonusOnSpawn(
      bool isPlayer,
      DrivenProperty drivenProperty,
      float baseValue);

    float GetHitpoints(bool isPlayer);
  }
}
