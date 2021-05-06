// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RecordMissionLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Diagnostics;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class RecordMissionLogic : MissionLogic
  {
    private float _lastRecordedTime = -1f;

    public override void OnBehaviourInitialize() => this.Mission.Recorder.StartRecording();

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if ((double) this._lastRecordedTime + 0.0199999995529652 >= (double) this.Mission.Time)
        return;
      this._lastRecordedTime = this.Mission.Time;
      this.Mission.Recorder.RecordCurrentState();
    }

    public override void OnRemoveBehaviour()
    {
      base.OnRemoveBehaviour();
      this.Mission.Recorder.BackupRecordToFile("Mission_record_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}_", (object) DateTime.Now) + (object) Process.GetCurrentProcess().Id, Game.Current.GameType.GetType().Name, this.Mission.SceneLevels);
      GameNetwork.ResetMissionData();
    }
  }
}
