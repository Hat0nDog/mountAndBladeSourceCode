// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.Handlers.BasicMissionHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
  public class BasicMissionHandler : MissionLogic
  {
    private InquiryData _retreatPopUpData;

    public bool IsWarningWidgetOpened { get; private set; }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._retreatPopUpData = new InquiryData("", GameTexts.FindText("str_retreat_question").ToString(), true, true, GameTexts.FindText("str_ok").ToString(), GameTexts.FindText("str_cancel").ToString(), new Action(this.OnEventAcceptSelectionWidget), new Action(this.OnEventCancelSelectionWidget));
      this.IsWarningWidgetOpened = false;
    }

    public void CreateWarningWidget()
    {
      if (!GameNetwork.IsClient)
        MBCommon.PauseGameEngine();
      InformationManager.ShowInquiry(this._retreatPopUpData, true);
      this.IsWarningWidgetOpened = true;
    }

    private void CloseSelectionWidget()
    {
      if (!this.IsWarningWidgetOpened)
        return;
      this.IsWarningWidgetOpened = false;
      if (GameNetwork.IsClient)
        return;
      MBCommon.UnPauseGameEngine();
    }

    private void OnEventCancelSelectionWidget() => this.CloseSelectionWidget();

    private void OnEventAcceptSelectionWidget()
    {
      foreach (MissionLogic missionLogic in this.Mission.MissionLogics.ToArray<MissionLogic>())
        missionLogic.OnBattleEnded();
      this.CloseSelectionWidget();
      this.Mission.RetreatMission();
    }
  }
}
