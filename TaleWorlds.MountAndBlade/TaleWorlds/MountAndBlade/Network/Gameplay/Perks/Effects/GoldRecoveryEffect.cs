// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.GoldRecoveryEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class GoldRecoveryEffect : MPPerkEffect
  {
    protected static string StringType = "GoldRecovery";
    private int _value;
    private int _period;

    public override bool IsTickRequired => true;

    protected GoldRecoveryEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s1 = node?.Attributes?["value"]?.Value;
      if (s1 != null)
        int.TryParse(s1, out this._value);
      string s2 = node?.Attributes?["period"]?.Value;
      if (s2 == null || !int.TryParse(s2, out this._period))
        return;
      int period = this._period;
    }

    public override void OnTick(Agent agent, int tickCount)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      MissionPeer peer = agent?.MissionPeer ?? agent?.OwningAgentMissionPeer;
      if (tickCount % this._period != 0 || peer == null)
        return;
      Mission.Current?.GetMissionBehaviour<MissionMultiplayerGameModeBase>()?.ChangeCurrentGoldForPeer(peer, peer.Representative.Gold + this._value);
    }
  }
}
