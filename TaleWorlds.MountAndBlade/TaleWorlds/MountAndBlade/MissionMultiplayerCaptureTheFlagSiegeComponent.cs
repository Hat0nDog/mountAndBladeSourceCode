// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerCaptureTheFlagSiegeComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  internal class MissionMultiplayerCaptureTheFlagSiegeComponent : MissionNetwork
  {
    private CaptureTheFlagCaptureResultEnum _captureResult = CaptureTheFlagCaptureResultEnum.NotCaptured;
    private bool _captureTimeOut;
    private MultiplayerGameNotificationsComponent _notificationsComponent;

    public CaptureTheFlagCapturePointSiege CapturePoint { get; private set; }

    public event MissionMultiplayerCaptureTheFlagSiegeComponent.OnCapturedDelegate OnCaptured;

    public override void OnMissionRestart()
    {
      this._captureResult = CaptureTheFlagCaptureResultEnum.NotCaptured;
      this._captureTimeOut = false;
    }

    public override void AfterStart()
    {
      base.AfterStart();
      this._notificationsComponent = this.Mission.GetMissionBehaviour<MultiplayerGameNotificationsComponent>();
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.InitEntities();
      int num = GameNetwork.IsServer ? 1 : 0;
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<FlagRaisingStatus>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagRaisingStatus>(this.HandleServerEventFlagRaisingStatus));
    }

    private void InitEntities()
    {
      this.CapturePoint = this.Mission.MissionObjects.SingleOrDefault<MissionObject>((Func<MissionObject, bool>) (x => x is CaptureTheFlagCapturePointSiege)) as CaptureTheFlagCapturePointSiege;
      this.CapturePoint.Captured += new CaptureTheFlagCapturePointSiege.CapturedDelegate(this.CapturePointCaptured);
    }

    private void OnRoundStarted() => this._captureResult = CaptureTheFlagCaptureResultEnum.NotCaptured;

    public override void OnMissionTick(float dt) => base.OnMissionTick(dt);

    private void HandleServerEventFlagRaisingStatus(FlagRaisingStatus message) => this.CapturePoint.SetFlagMotion(message.Progress, message.Direction, message.Speed);

    private void StartTimeoutMode()
    {
      this._captureTimeOut = true;
      this.CapturePoint.CaptureTimeOutMode = true;
      if (GameNetwork.IsDedicatedServer || !GameNetwork.MyPeer.IsSynchronized)
        return;
      if (GameNetwork.MyPeer.GetComponent<MissionPeer>().Team == this.Mission.AttackerTeam)
        return;
      Team defenderTeam = this.Mission.DefenderTeam;
    }

    private void CapturePointCaptured(CaptureTheFlagCaptureResultEnum result) => this._captureResult = result;

    public void OnCaptureResult(CaptureTheFlagCaptureResultEnum result)
    {
      if (GameNetwork.IsDedicatedServer || !GameNetwork.MyPeer.IsSynchronized)
        return;
      MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
      switch (result)
      {
        case CaptureTheFlagCaptureResultEnum.AttackersWin:
          if (component == null)
            break;
          SoundEvent.PlaySound2D(component.Team.Side == BattleSideEnum.Attacker ? "event:/ui/mission/multiplayer/mp_victory" : "event:/ui/mission/multiplayer/mp_defeat");
          break;
        case CaptureTheFlagCaptureResultEnum.DefendersWin:
          if (component == null)
            break;
          SoundEvent.PlaySound2D(component.Team.Side == BattleSideEnum.Defender ? "event:/ui/mission/multiplayer/mp_victory" : "event:/ui/mission/multiplayer/mp_defeat");
          break;
      }
    }

    protected virtual void OnRoundEnd()
    {
      if (this._captureResult == CaptureTheFlagCaptureResultEnum.NotCaptured)
        this._captureResult = CaptureTheFlagCaptureResultEnum.DefendersWin;
      if (this._captureResult == CaptureTheFlagCaptureResultEnum.NotCaptured)
        return;
      if (this.OnCaptured != null)
        this.OnCaptured(this._captureResult);
      this.OnCaptureResult(this._captureResult);
      if (this._captureResult == CaptureTheFlagCaptureResultEnum.DefendersWin || this._captureResult == CaptureTheFlagCaptureResultEnum.AttackersWin)
        return;
      int captureResult = (int) this._captureResult;
    }

    public override void OnRemoveBehaviour()
    {
      int num = GameNetwork.IsServer ? 1 : 0;
      base.OnRemoveBehaviour();
    }

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      if (networkPeer.IsServerPeer || this.CapturePoint == null)
        return;
      this.CapturePoint.SynchronizeToPeer(networkPeer);
    }

    public bool CheckForRoundEnd()
    {
      int num = this._captureTimeOut ? 1 : 0;
      return this._captureResult != CaptureTheFlagCaptureResultEnum.NotCaptured;
    }

    public delegate void OnCapturedDelegate(CaptureTheFlagCaptureResultEnum result);
  }
}
