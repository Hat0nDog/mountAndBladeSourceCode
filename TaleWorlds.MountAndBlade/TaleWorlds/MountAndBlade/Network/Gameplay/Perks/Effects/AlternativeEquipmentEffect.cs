// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.AlternativeEquipmentEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class AlternativeEquipmentEffect : MPOnSpawnPerkEffect
  {
    protected static string StringType = "AlternativeEquipmentOnSpawn";
    private EquipmentElement _item;
    private EquipmentIndex _index;

    protected AlternativeEquipmentEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      base.Deserialize(node);
      this._item = new EquipmentElement();
      this._index = EquipmentIndex.None;
      XmlNode attribute1 = (XmlNode) node?.Attributes?["item"];
      if (attribute1 != null)
        this._item = new EquipmentElement(MBObjectManager.Instance.GetObject<ItemObject>(attribute1.Value));
      XmlNode attribute2 = (XmlNode) node?.Attributes?["slot"];
      if (attribute2 == null)
        return;
      this._index = Equipment.GetEquipmentIndexFromOldEquipmentIndexName(attribute2.Value);
    }

    public override List<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
      bool isPlayer,
      List<(EquipmentIndex, EquipmentElement)> alternativeEquipments)
    {
      if (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player ? 1 : 0) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops ? 1 : 0)) != 0)
      {
        if (alternativeEquipments == null)
          alternativeEquipments = new List<(EquipmentIndex, EquipmentElement)>()
          {
            (this._index, this._item)
          };
        else
          alternativeEquipments.Add((this._index, this._item));
      }
      return alternativeEquipments;
    }
  }
}
