// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.ShieldDamageTakenEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class ShieldDamageTakenEffect : MPPerkEffect
  {
    protected static string StringType = "ShieldDamageTaken";
    private float _value;
    private ShieldDamageTakenEffect.BlockType _blockType;

    protected ShieldDamageTakenEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s = node?.Attributes?["value"]?.Value;
      if (s != null)
        float.TryParse(s, out this._value);
      string str = node?.Attributes?["block_type"]?.Value;
      this._blockType = ShieldDamageTakenEffect.BlockType.Any;
      if (str == null || Enum.TryParse<ShieldDamageTakenEffect.BlockType>(str, true, out this._blockType))
        return;
      this._blockType = ShieldDamageTakenEffect.BlockType.Any;
    }

    public override float GetShieldDamageTaken(bool isCorrectSideBlock)
    {
      switch (this._blockType)
      {
        case ShieldDamageTakenEffect.BlockType.Any:
          return this._value;
        case ShieldDamageTakenEffect.BlockType.CorrectSide:
          return !isCorrectSideBlock ? 0.0f : this._value;
        case ShieldDamageTakenEffect.BlockType.WrongSide:
          return !isCorrectSideBlock ? this._value : 0.0f;
        default:
          return 0.0f;
      }
    }

    private enum BlockType
    {
      Any,
      CorrectSide,
      WrongSide,
    }
  }
}
