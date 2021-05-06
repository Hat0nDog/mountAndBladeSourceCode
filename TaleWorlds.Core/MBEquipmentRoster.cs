// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBEquipmentRoster
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public class MBEquipmentRoster : MBObjectBase
  {
    public static readonly Equipment EmptyEquipment = new Equipment(true);
    private List<Equipment> _equipments;

    public MBReadOnlyList<Equipment> AllEquipments
    {
      get
      {
        if (!this._equipments.IsEmpty<Equipment>())
          return this._equipments.GetReadOnlyList<Equipment>();
        return new List<Equipment>()
        {
          MBEquipmentRoster.EmptyEquipment
        }.GetReadOnlyList<Equipment>();
      }
    }

    public Equipment DefaultEquipment => this._equipments.IsEmpty<Equipment>() ? MBEquipmentRoster.EmptyEquipment : this._equipments.FirstOrDefault<Equipment>();

    public MBEquipmentRoster() => this._equipments = new List<Equipment>();

    public void Init(MBObjectManager objectManager, XmlNode node)
    {
      if (!(node.Name == "EquipmentRoster"))
        return;
      this.InitEquipment(objectManager, node);
    }

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (childNode.Name == "EquipmentSet")
          this.InitEquipment(objectManager, childNode);
      }
    }

    private void InitEquipment(MBObjectManager objectManager, XmlNode node)
    {
      Equipment equipment = new Equipment(node.Attributes["civilian"] != null && bool.Parse(node.Attributes["civilian"].Value));
      equipment.Deserialize(objectManager, node);
      this._equipments.Add(equipment);
    }

    public void AddEquipmentRoster(MBEquipmentRoster equipmentRoster, bool isCivilian)
    {
      foreach (Equipment equipment in equipmentRoster._equipments.ToList<Equipment>())
      {
        if (equipment.IsCivilian == isCivilian)
          this._equipments.Add(equipment);
      }
    }

    public void AddOverridenEquipments(
      MBObjectManager objectManager,
      List<XmlNode> overridenEquipmentSlots)
    {
      List<Equipment> list = this._equipments.ToList<Equipment>();
      this._equipments.Clear();
      foreach (Equipment equipment in list)
        this._equipments.Add(equipment.Clone());
      foreach (XmlNode overridenEquipmentSlot in overridenEquipmentSlots)
      {
        foreach (Equipment equipment in this._equipments)
          equipment.DeserializeNode(objectManager, overridenEquipmentSlot);
      }
    }

    public void OrderEquipments() => this._equipments = new List<Equipment>((IEnumerable<Equipment>) this._equipments.OrderByDescending<Equipment, bool>((Func<Equipment, bool>) (eq => !eq.IsCivilian)));

    public void InitializeDefaultEquipment(string equipmentName)
    {
      if (this._equipments[0] == null)
        this._equipments[0] = new Equipment(false);
      this._equipments[0].FillFrom(Game.Current.GetDefaultEquipmentWithName(equipmentName));
    }
  }
}
