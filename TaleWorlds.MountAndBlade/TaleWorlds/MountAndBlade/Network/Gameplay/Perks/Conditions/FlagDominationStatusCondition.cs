// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.FlagDominationStatusCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class FlagDominationStatusCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
  {
    protected static string StringType = "FlagDominationStatus";
    private FlagDominationStatusCondition.Status _status;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;

    public override bool IsPeerCondition => true;

    protected FlagDominationStatusCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["status"]?.Value;
      this._status = FlagDominationStatusCondition.Status.Tie;
      if (str == null || Enum.TryParse<FlagDominationStatusCondition.Status>(str, true, out this._status))
        return;
      this._status = FlagDominationStatusCondition.Status.Tie;
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent == null)
        return false;
      MissionMultiplayerFlagDomination gameModeInstance = this.GameModeInstance;
      int num1 = 0;
      int num2 = 0;
      foreach (FlagCapturePoint allCapturePoint in gameModeInstance.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
        {
          Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(allCapturePoint);
          if (flagOwnerTeam == agent.Team)
            ++num1;
          else if (flagOwnerTeam != null)
            ++num2;
        }
      }
      if (this._status == FlagDominationStatusCondition.Status.Winning)
        return num1 > num2;
      return this._status != FlagDominationStatusCondition.Status.Losing ? num1 == num2 : num2 > num1;
    }

    private enum Status
    {
      Winning,
      Losing,
      Tie,
    }
  }
}
