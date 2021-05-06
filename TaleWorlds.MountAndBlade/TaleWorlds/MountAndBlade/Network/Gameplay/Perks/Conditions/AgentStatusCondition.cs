// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.AgentStatusCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class AgentStatusCondition : MPPerkCondition
  {
    protected static string StringType = "AgentStatus";
    private AgentStatusCondition.AgentStatus _status;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.MountChange;

    protected AgentStatusCondition()
    {
    }

    protected override void Deserialize(XmlNode node) => Enum.TryParse<AgentStatusCondition.AgentStatus>(node?.Attributes?["agent_status"]?.Value, true, out this._status);

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      if (agent == null)
        return false;
      return agent.MountAgent == null ? this._status == AgentStatusCondition.AgentStatus.OnFoot : this._status == AgentStatusCondition.AgentStatus.OnMount;
    }

    private enum AgentStatus
    {
      OnFoot,
      OnMount,
    }
  }
}
