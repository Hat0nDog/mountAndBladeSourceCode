﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CaptainScoreboardData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade
{
  public class CaptainScoreboardData : IScoreboardData
  {
    public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
    {
      GameNetwork.MyPeer.GetComponent<MissionRepresentativeBase>();
      return new MissionScoreboardComponent.ScoreboardHeader[9]
      {
        new MissionScoreboardComponent.ScoreboardHeader("avatar", (Func<MissionPeer, string>) (missionPeer => ""), (Func<BotData, string>) (bot => "")),
        new MissionScoreboardComponent.ScoreboardHeader("badge", (Func<MissionPeer, string>) (missionPeer => BadgeManager.GetByIndex(missionPeer.GetPeer().ChosenBadgeIndex)?.StringId), (Func<BotData, string>) (bot => "")),
        new MissionScoreboardComponent.ScoreboardHeader("name", (Func<MissionPeer, string>) (missionPeer => missionPeer.Name.ToString()), (Func<BotData, string>) (bot => new TextObject("{=hvQSOi79}Bot").ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("kill", (Func<MissionPeer, string>) (missionPeer => missionPeer.KillCount.ToString()), (Func<BotData, string>) (bot => bot.KillCount.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("death", (Func<MissionPeer, string>) (missionPeer => missionPeer.DeathCount.ToString()), (Func<BotData, string>) (bot => bot.DeathCount.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("assist", (Func<MissionPeer, string>) (missionPeer => missionPeer.AssistCount.ToString()), (Func<BotData, string>) (bot => bot.AssistCount.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("score", (Func<MissionPeer, string>) (missionPeer => missionPeer.Score.ToString()), (Func<BotData, string>) (bot => bot.Score.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("soldiers", (Func<MissionPeer, string>) (missionPeer =>
        {
          TextObject textObject = new TextObject("{=4FVIuqsl}{ALIVE}/{TOTAL}");
          textObject.SetTextVariable("ALIVE", missionPeer.BotsUnderControlAlive + (missionPeer.IsControlledAgentActive ? 1 : 0));
          textObject.SetTextVariable("TOTAL", missionPeer.BotsUnderControlTotal + 1);
          return textObject.ToString();
        }), (Func<BotData, string>) (bot =>
        {
          TextObject textObject = new TextObject("{=4FVIuqsl}{ALIVE}/{TOTAL}");
          textObject.SetTextVariable("ALIVE", bot.AliveCount);
          textObject.SetTextVariable("TOTAL", 0);
          return textObject.ToString();
        })),
        new MissionScoreboardComponent.ScoreboardHeader("ping", (Func<MissionPeer, string>) (missionPeer => missionPeer.GetNetworkPeer().AveragePingInMilliseconds.Round().ToString()), (Func<BotData, string>) (bot => ""))
      };
    }
  }
}
