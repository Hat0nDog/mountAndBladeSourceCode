// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.LastRemainingFlagCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class LastRemainingFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
  {
    protected static string StringType = "FlagDominationLastRemainingFlag";
    private LastRemainingFlagCondition.FlagOwner _owner;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;

    public override bool IsPeerCondition => true;

    protected LastRemainingFlagCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["owner"]?.Value;
      this._owner = LastRemainingFlagCondition.FlagOwner.Any;
      if (str == null || Enum.TryParse<LastRemainingFlagCondition.FlagOwner>(str, true, out this._owner))
        return;
      this._owner = LastRemainingFlagCondition.FlagOwner.Any;
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent == null)
        return false;
      MissionMultiplayerFlagDomination gameModeInstance = this.GameModeInstance;
      LastRemainingFlagCondition.FlagOwner flagOwner = LastRemainingFlagCondition.FlagOwner.None;
      int num = 0;
      foreach (FlagCapturePoint allCapturePoint in gameModeInstance.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
        {
          ++num;
          Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(allCapturePoint);
          flagOwner = flagOwnerTeam != null ? (flagOwnerTeam != agent.Team ? LastRemainingFlagCondition.FlagOwner.Enemy : LastRemainingFlagCondition.FlagOwner.Ally) : LastRemainingFlagCondition.FlagOwner.None;
        }
      }
      if (num != 1)
        return false;
      return this._owner == LastRemainingFlagCondition.FlagOwner.Any || this._owner == flagOwner;
    }

    private enum FlagOwner
    {
      Ally,
      Enemy,
      None,
      Any,
    }
  }
}
