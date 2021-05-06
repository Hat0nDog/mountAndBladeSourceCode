// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPOnSpawnPerkEffectBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MPOnSpawnPerkEffectBase : MPPerkEffectBase, IOnSpawnPerkEffect
  {
    protected MPOnSpawnPerkEffectBase.Target EffectTarget;

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["target"]?.Value;
      this.EffectTarget = MPOnSpawnPerkEffectBase.Target.Any;
      if (str == null || Enum.TryParse<MPOnSpawnPerkEffectBase.Target>(str, true, out this.EffectTarget))
        return;
      this.EffectTarget = MPOnSpawnPerkEffectBase.Target.Any;
    }

    public virtual float GetTroopCountMultiplier() => 0.0f;

    public virtual float GetExtraTroopCount() => 0.0f;

    public virtual List<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
      bool isPlayer,
      List<(EquipmentIndex, EquipmentElement)> alternativeEquipments)
    {
      return alternativeEquipments;
    }

    public virtual float GetDrivenPropertyBonusOnSpawn(
      bool isPlayer,
      DrivenProperty drivenProperty,
      float baseValue)
    {
      return 0.0f;
    }

    public virtual float GetHitpoints(bool isPlayer) => 0.0f;

    protected enum Target
    {
      Player,
      Troops,
      Any,
    }
  }
}
