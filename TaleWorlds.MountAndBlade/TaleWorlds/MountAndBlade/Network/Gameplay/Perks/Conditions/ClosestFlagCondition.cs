// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.ClosestFlagCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class ClosestFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
  {
    protected static string StringType = "FlagDominationClosestFlag";
    private ClosestFlagCondition.FlagOwner _owner;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;

    public override bool IsPeerCondition => true;

    protected ClosestFlagCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["owner"]?.Value;
      this._owner = ClosestFlagCondition.FlagOwner.Any;
      if (str == null || Enum.TryParse<ClosestFlagCondition.FlagOwner>(str, true, out this._owner))
        return;
      this._owner = ClosestFlagCondition.FlagOwner.Any;
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent == null)
        return false;
      MissionMultiplayerFlagDomination gameModeInstance = this.GameModeInstance;
      ClosestFlagCondition.FlagOwner flagOwner = ClosestFlagCondition.FlagOwner.None;
      float num1 = float.MaxValue;
      foreach (FlagCapturePoint allCapturePoint in gameModeInstance.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
        {
          float num2 = agent.Position.DistanceSquared(allCapturePoint.Position);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(allCapturePoint);
            flagOwner = flagOwnerTeam != null ? (flagOwnerTeam != agent.Team ? ClosestFlagCondition.FlagOwner.Enemy : ClosestFlagCondition.FlagOwner.Ally) : ClosestFlagCondition.FlagOwner.None;
          }
        }
      }
      return this._owner == ClosestFlagCondition.FlagOwner.Any || this._owner == flagOwner;
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
