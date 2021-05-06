// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.LastManStandingCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class LastManStandingCondition : MPPerkCondition
  {
    protected static string StringType = "LastManStanding";

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.SpawnEnd;

    protected LastManStandingCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      MissionPeer missionPeer = agent?.MissionPeer ?? agent?.OwningAgentMissionPeer;
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0 || (missionPeer?.ControlledFormation == null || !agent.IsActive()))
        return false;
      return !agent.IsPlayerControlled ? missionPeer.BotsUnderControlAlive == 1 : missionPeer.BotsUnderControlAlive == 0;
    }
  }
}
