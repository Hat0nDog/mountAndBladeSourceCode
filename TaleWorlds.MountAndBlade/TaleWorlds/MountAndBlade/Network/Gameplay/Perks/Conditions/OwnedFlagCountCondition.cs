// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.OwnedFlagCountCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class OwnedFlagCountCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
  {
    protected static string StringType = "FlagDominationOwnedFlagCount";
    private int _min;
    private int _max;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;

    public override bool IsPeerCondition => true;

    protected OwnedFlagCountCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s1 = node?.Attributes?["min"]?.Value;
      if (s1 == null)
        this._min = 0;
      else
        int.TryParse(s1, out this._min);
      string s2 = node?.Attributes?["max"]?.Value;
      if (s2 == null)
        this._max = int.MaxValue;
      else
        int.TryParse(s2, out this._max);
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent == null)
        return false;
      MissionMultiplayerFlagDomination gameModeInstance = this.GameModeInstance;
      int num = 0;
      foreach (FlagCapturePoint allCapturePoint in gameModeInstance.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated && gameModeInstance.GetFlagOwnerTeam(allCapturePoint) == agent.Team)
          ++num;
      }
      return num >= this._min && num <= this._max;
    }
  }
}
