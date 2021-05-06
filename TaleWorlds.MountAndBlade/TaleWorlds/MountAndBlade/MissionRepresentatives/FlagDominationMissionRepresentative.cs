// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRepresentatives.FlagDominationMissionRepresentative
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade.MissionRepresentatives
{
  public class FlagDominationMissionRepresentative : MissionRepresentativeBase
  {
    private bool _survivedLastRound;

    public override bool IsThereAgentAction(Agent targetAgent) => false;

    public override void OnAgentInteraction(Agent targetAgent)
    {
    }

    public int GetGoldAmountForVisual() => this.Gold < 0 ? 80 : this.Gold;

    public void UpdateSelectedClassServer(Agent agent) => this._survivedLastRound = agent != null;

    public bool CheckIfSurvivedLastRoundAndReset()
    {
      int num = this._survivedLastRound ? 1 : 0;
      this._survivedLastRound = false;
      return num != 0;
    }

    public int GetGoldGainsFromKillData(
      MPPerkObject.MPPerkHandler killerPerkHandler,
      MPPerkObject.MPPerkHandler assistingHitterPerkHandler,
      MultiplayerClassDivisions.MPHeroClass victimClass,
      bool isAssist)
    {
      int num1;
      if (isAssist)
      {
        num1 = (killerPerkHandler != null ? killerPerkHandler.GetRewardedGoldOnAssist() : 0) + (assistingHitterPerkHandler != null ? assistingHitterPerkHandler.GetGoldOnAssist() : 0);
      }
      else
      {
        int num2 = this.ControlledAgent != null ? MultiplayerClassDivisions.GetMPHeroClassForCharacter(this.ControlledAgent.Character).TroopCost : 0;
        num1 = killerPerkHandler != null ? killerPerkHandler.GetGoldOnKill((float) num2, (float) victimClass.TroopCost) : 0;
      }
      if (num1 > 0)
      {
        GameNetwork.BeginModuleEventAsServer(this.Peer);
        GameNetwork.WriteMessage((GameNetworkMessage) new TDMGoldGain(new List<KeyValuePair<ushort, int>>()
        {
          new KeyValuePair<ushort, int>((ushort) 2048, num1)
        }));
        GameNetwork.EndModuleEventAsServer();
      }
      return num1;
    }

    public int GetGoldGainsFromAllyDeathReward(int baseAmount)
    {
      if (baseAmount > 0 && !this.Peer.Communicator.IsServerPeer && this.Peer.Communicator.IsConnectionActive)
      {
        GameNetwork.BeginModuleEventAsServer(this.Peer);
        GameNetwork.WriteMessage((GameNetworkMessage) new TDMGoldGain(new List<KeyValuePair<ushort, int>>()
        {
          new KeyValuePair<ushort, int>((ushort) 2048, baseAmount)
        }));
        GameNetwork.EndModuleEventAsServer();
      }
      return baseAmount;
    }
  }
}
