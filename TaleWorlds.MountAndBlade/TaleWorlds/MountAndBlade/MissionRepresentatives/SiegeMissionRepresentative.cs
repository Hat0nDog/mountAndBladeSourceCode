// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRepresentatives.SiegeMissionRepresentative
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade.MissionRepresentatives
{
  public class SiegeMissionRepresentative : MissionRepresentativeBase
  {
    private const int FirstRangedKillGold = 10;
    private const int FirstMeleeKillGold = 10;
    private const int FirstAssistGold = 10;
    private const int SecondAssistGold = 10;
    private const int ThirdAssistGold = 10;
    private const int FifthKillGold = 20;
    private const int TenthKillGold = 30;
    private GoldGainFlags _currentGoldGains;
    private int _killCountOnSpawn;
    private int _assistCountOnSpawn;

    public override void OnAgentInteraction(Agent targetAgent)
    {
    }

    public override bool IsThereAgentAction(Agent targetAgent) => false;

    public override void OnAgentSpawned()
    {
      this._currentGoldGains = (GoldGainFlags) 0;
      this._killCountOnSpawn = this.MissionPeer.KillCount;
      this._assistCountOnSpawn = this.MissionPeer.AssistCount;
    }

    public int GetGoldGainsFromKillDataAndUpdateFlags(
      MPPerkObject.MPPerkHandler killerPerkHandler,
      MPPerkObject.MPPerkHandler assistingHitterPerkHandler,
      MultiplayerClassDivisions.MPHeroClass victimClass,
      bool isAssist,
      bool isRanged)
    {
      int num1 = 0;
      List<KeyValuePair<ushort, int>> goldChangeEventList = new List<KeyValuePair<ushort, int>>();
      if (isAssist)
      {
        int num2 = 1;
        int num3 = (killerPerkHandler != null ? killerPerkHandler.GetRewardedGoldOnAssist() : 0) + (assistingHitterPerkHandler != null ? assistingHitterPerkHandler.GetGoldOnAssist() : 0);
        if (num3 > 0)
        {
          num1 += num3;
          this._currentGoldGains |= GoldGainFlags.PerkBonus;
          goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 2048, num3));
        }
        switch (this.MissionPeer.AssistCount - this._assistCountOnSpawn)
        {
          case 1:
            num1 += 10;
            this._currentGoldGains |= GoldGainFlags.FirstAssist;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 4, 10));
            break;
          case 2:
            num1 += 10;
            this._currentGoldGains |= GoldGainFlags.SecondAssist;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 8, 10));
            break;
          case 3:
            num1 += 10;
            this._currentGoldGains |= GoldGainFlags.ThirdAssist;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 16, 10));
            break;
          default:
            num1 += num2;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 256, num2));
            break;
        }
      }
      else
      {
        int num2 = 0;
        if (this.ControlledAgent != null)
        {
          num2 = MultiplayerClassDivisions.GetMPHeroClassForCharacter(this.ControlledAgent.Character).TroopCost;
          int num3 = 2 + Math.Max(0, (victimClass.TroopCost - num2) / 2);
          num1 += num3;
          goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 128, num3));
        }
        int num4 = killerPerkHandler != null ? killerPerkHandler.GetGoldOnKill((float) num2, (float) victimClass.TroopCost) : 0;
        if (num4 > 0)
        {
          num1 += num4;
          this._currentGoldGains |= GoldGainFlags.PerkBonus;
          goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 2048, num4));
        }
        switch (this.MissionPeer.KillCount - this._killCountOnSpawn)
        {
          case 5:
            num1 += 20;
            this._currentGoldGains |= GoldGainFlags.FifthKill;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 32, 20));
            break;
          case 10:
            num1 += 30;
            this._currentGoldGains |= GoldGainFlags.TenthKill;
            goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 64, 30));
            break;
        }
        if (isRanged && !this._currentGoldGains.HasAnyFlag<GoldGainFlags>(GoldGainFlags.FirstRangedKill))
        {
          num1 += 10;
          this._currentGoldGains |= GoldGainFlags.FirstRangedKill;
          goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 1, 10));
        }
        if (!isRanged && !this._currentGoldGains.HasAnyFlag<GoldGainFlags>(GoldGainFlags.FirstMeleeKill))
        {
          num1 += 10;
          this._currentGoldGains |= GoldGainFlags.FirstMeleeKill;
          goldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) 2, 10));
        }
      }
      int num5 = 0;
      if (this.MissionPeer.Team == Mission.Current.Teams.Attacker)
        num5 = MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetIntValue();
      else if (this.MissionPeer.Team == Mission.Current.Teams.Defender)
        num5 = MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.GetIntValue();
      if (num5 != 0 && (num1 > 0 || goldChangeEventList.Count > 0))
      {
        num1 = 0;
        float num2 = (float) (1.0 + (double) num5 * 0.00999999977648258);
        for (int index = 0; index < goldChangeEventList.Count; ++index)
        {
          int num3 = (int) ((double) goldChangeEventList[index].Value * (double) num2);
          goldChangeEventList[index] = new KeyValuePair<ushort, int>(goldChangeEventList[index].Key, num3);
          num1 += num3;
        }
      }
      if (goldChangeEventList.Count > 0 && !this.Peer.Communicator.IsServerPeer && this.Peer.Communicator.IsConnectionActive)
      {
        GameNetwork.BeginModuleEventAsServer(this.Peer);
        GameNetwork.WriteMessage((GameNetworkMessage) new TDMGoldGain(goldChangeEventList));
        GameNetwork.EndModuleEventAsServer();
      }
      return num1;
    }

    public int GetGoldGainsFromObjectiveAssist(
      GameEntity objectiveMostParentEntity,
      float contributionRatio,
      bool isCompleted)
    {
      int num = (int) ((double) contributionRatio * (double) this.GetTotalGoldDistributionForDestructable(objectiveMostParentEntity));
      if (num > 0 && !this.Peer.Communicator.IsServerPeer && this.Peer.Communicator.IsConnectionActive)
      {
        GameNetwork.BeginModuleEventAsServer(this.Peer);
        GameNetwork.WriteMessage((GameNetworkMessage) new TDMGoldGain(new List<KeyValuePair<ushort, int>>()
        {
          new KeyValuePair<ushort, int>(isCompleted ? (ushort) 512 : (ushort) 1024, num)
        }));
        GameNetwork.EndModuleEventAsServer();
      }
      return num;
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

    private int GetTotalGoldDistributionForDestructable(GameEntity objectiveMostParentEntity)
    {
      string str1 = (string) null;
      foreach (string tag in objectiveMostParentEntity.Tags)
      {
        if (tag.StartsWith("mp_siege_objective_"))
        {
          str1 = tag;
          break;
        }
      }
      if (str1 == null)
        return 20;
      string str2 = str1.Replace("mp_siege_objective_", "");
      if (str2 == "wall_breach" || str2 == "castle_gate")
        return 500;
      return str2 == "battering_ram" || str2 == "siege_tower" ? 600 : 20;
    }
  }
}
