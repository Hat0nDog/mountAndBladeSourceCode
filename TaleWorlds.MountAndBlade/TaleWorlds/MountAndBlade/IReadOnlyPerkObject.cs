// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IReadOnlyPerkObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public interface IReadOnlyPerkObject
  {
    TextObject Name { get; }

    TextObject Description { get; }

    List<string> GameModes { get; }

    int PerkListIndex { get; }

    string IconId { get; }

    string HeroIdleAnimOverride { get; }

    string HeroMountIdleAnimOverride { get; }

    string TroopIdleAnimOverride { get; }

    string TroopMountIdleAnimOverride { get; }

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

    MPPerkObject Clone(MissionPeer peer);
  }
}
