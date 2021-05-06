// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BladeData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public sealed class BladeData : MBObjectBase
  {
    public readonly CraftingPiece.PieceTypes PieceType;

    public DamageTypes ThrustDamageType { get; private set; }

    public float ThrustDamageFactor { get; private set; }

    public DamageTypes SwingDamageType { get; private set; }

    public float SwingDamageFactor { get; private set; }

    public float BladeLength { get; private set; }

    public float BladeWidth { get; private set; }

    public short StackAmount { get; private set; }

    public string PhysicsMaterial { get; private set; }

    public string BodyName { get; private set; }

    public string HolsterMeshName { get; private set; }

    public string HolsterBodyName { get; private set; }

    public float HolsterMeshLength { get; private set; }

    public BladeData(CraftingPiece.PieceTypes pieceType, float bladeLength)
    {
      this.PieceType = pieceType;
      this.BladeLength = bladeLength;
      this.ThrustDamageType = DamageTypes.Invalid;
      this.SwingDamageType = DamageTypes.Invalid;
    }

    public override void Deserialize(MBObjectManager objectManager, XmlNode childNode)
    {
      this.Initialize();
      XmlAttribute attribute1 = childNode.Attributes["stack_amount"];
      XmlAttribute attribute2 = childNode.Attributes["blade_length"];
      XmlAttribute attribute3 = childNode.Attributes["blade_width"];
      XmlAttribute attribute4 = childNode.Attributes["physics_material"];
      XmlAttribute attribute5 = childNode.Attributes["body_name"];
      XmlAttribute attribute6 = childNode.Attributes["holster_mesh"];
      XmlAttribute attribute7 = childNode.Attributes["holster_body_name"];
      XmlAttribute attribute8 = childNode.Attributes["holster_mesh_length"];
      this.StackAmount = attribute1 != null ? short.Parse(attribute1.Value) : (short) 1;
      this.BladeLength = attribute2 != null ? 0.01f * float.Parse(attribute2.Value) : this.BladeLength;
      this.BladeWidth = attribute3 != null ? 0.01f * float.Parse(attribute3.Value) : (float) (0.150000005960464 + (double) this.BladeLength * 0.300000011920929);
      this.PhysicsMaterial = attribute4?.InnerText;
      this.BodyName = attribute5?.InnerText;
      this.HolsterMeshName = attribute6?.InnerText;
      this.HolsterBodyName = attribute7?.InnerText;
      this.HolsterMeshLength = (float) (0.00999999977648258 * (attribute8 != null ? (double) float.Parse(attribute8.Value) : 0.0));
      foreach (XmlNode childNode1 in childNode.ChildNodes)
      {
        string name = childNode1.Name;
        if (!(name == "Thrust"))
        {
          if (name == "Swing")
          {
            XmlAttribute attribute9 = childNode1.Attributes["damage_type"];
            XmlAttribute attribute10 = childNode1.Attributes["damage_factor"];
            this.SwingDamageType = (DamageTypes) Enum.Parse(typeof (DamageTypes), attribute9.Value, true);
            this.SwingDamageFactor = float.Parse(attribute10.Value);
          }
        }
        else
        {
          XmlAttribute attribute9 = childNode1.Attributes["damage_type"];
          XmlAttribute attribute10 = childNode1.Attributes["damage_factor"];
          this.ThrustDamageType = (DamageTypes) Enum.Parse(typeof (DamageTypes), attribute9.Value, true);
          this.ThrustDamageFactor = float.Parse(attribute10.Value);
        }
      }
    }
  }
}
