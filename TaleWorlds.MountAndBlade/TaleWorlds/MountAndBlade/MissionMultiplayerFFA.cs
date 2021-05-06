// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerFFA
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerFFA : MissionMultiplayerGameModeBase
  {
    public override bool IsGameModeHidingAllAgentVisuals => true;

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType() => MissionLobbyComponent.MultiplayerGameType.FreeForAll;

    public override void AfterStart()
    {
      BasicCultureObject basicCultureObject = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      Banner banner = new Banner(basicCultureObject.BannerKey, basicCultureObject.BackgroundColor1, basicCultureObject.ForegroundColor1);
      Team otherTeam = this.Mission.Teams.Add(BattleSideEnum.Attacker, basicCultureObject.BackgroundColor1, basicCultureObject.ForegroundColor1, banner, false);
      otherTeam.SetIsEnemyOf(otherTeam, true);
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      networkPeer.AddComponent<FFAMissionRepresentative>();
      networkPeer.GetComponent<MissionPeer>().Team = this.Mission.AttackerTeam;
    }
  }
}
