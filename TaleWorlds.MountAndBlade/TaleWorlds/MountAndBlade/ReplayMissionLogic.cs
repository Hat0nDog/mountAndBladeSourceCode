// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ReplayMissionLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class ReplayMissionLogic : MissionLogic
  {
    private bool _isMultiplayer;

    public string FileName { get; private set; }

    public ReplayMissionLogic(bool isMultiplayer, string fileName = "")
    {
      if (!string.IsNullOrEmpty(fileName))
        this.FileName = fileName;
      this._isMultiplayer = isMultiplayer;
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      if (this._isMultiplayer)
        GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      MBCommon.CurrentGameType = MBCommon.GameType.SingleReplay;
      GameNetwork.InitializeClientSide((string) null, 0, -1, -1);
      this.Mission.Recorder.RestoreRecordFromFile(this.FileName);
    }

    public override void OnRemoveBehaviour()
    {
      if (this._isMultiplayer)
        GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
      GameNetwork.TerminateClientSide();
      this.Mission.Recorder.ClearRecordBuffers();
    }
  }
}
