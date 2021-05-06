// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.EncumbranceEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class EncumbranceEffect : MPPerkEffect
  {
    protected static string StringType = "Encumbrance";
    private bool _isOnBody;
    private float _value;

    protected EncumbranceEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s = node?.Attributes?["value"]?.Value;
      if (s != null)
        float.TryParse(s, out this._value);
      this._isOnBody = node?.Attributes?["is_on_body"]?.Value?.ToLower() == "true";
    }

    public override void OnUpdate(Agent agent, bool newState)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      agent?.UpdateAgentProperties();
    }

    public override float GetEncumbrance(bool isOnBody) => isOnBody != this._isOnBody ? 0.0f : this._value;
  }
}
