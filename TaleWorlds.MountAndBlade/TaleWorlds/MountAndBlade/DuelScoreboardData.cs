// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DuelScoreboardData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class DuelScoreboardData : IScoreboardData
  {
    public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
    {
      GameNetwork.MyPeer.GetComponent<MissionRepresentativeBase>();
      return new MissionScoreboardComponent.ScoreboardHeader[6]
      {
        new MissionScoreboardComponent.ScoreboardHeader("avatar", (Func<MissionPeer, string>) (missionPeer => ""), (Func<BotData, string>) (bot => "")),
        new MissionScoreboardComponent.ScoreboardHeader("name", (Func<MissionPeer, string>) (missionPeer => missionPeer.Name.ToString()), (Func<BotData, string>) (bot => new TextObject("{=hvQSOi79}Bot").ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("kill", (Func<MissionPeer, string>) (missionPeer => missionPeer.KillCount.ToString()), (Func<BotData, string>) (bot => bot.KillCount.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("death", (Func<MissionPeer, string>) (missionPeer => missionPeer.DeathCount.ToString()), (Func<BotData, string>) (bot => bot.DeathCount.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("score", (Func<MissionPeer, string>) (missionPeer => missionPeer.Score.ToString()), (Func<BotData, string>) (bot => bot.Score.ToString())),
        new MissionScoreboardComponent.ScoreboardHeader("ping", (Func<MissionPeer, string>) (missionPeer => missionPeer.GetNetworkPeer().AveragePingInMilliseconds.Round().ToString()), (Func<BotData, string>) (bot => ""))
      };
    }
  }
}
