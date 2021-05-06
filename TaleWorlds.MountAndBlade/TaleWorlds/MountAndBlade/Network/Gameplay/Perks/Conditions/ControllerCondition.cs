// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.ControllerCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class ControllerCondition : MPPerkCondition
  {
    protected static string StringType = "Controller";
    private bool _isPlayerControlled;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.PeerControlledAgentChange;

    protected ControllerCondition()
    {
    }

    protected override void Deserialize(XmlNode node) => this._isPlayerControlled = node?.Attributes?["is_player_controlled"]?.Value?.ToLower() == "true";

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      return agent != null && agent.IsPlayerControlled == this._isPlayerControlled;
    }
  }
}
