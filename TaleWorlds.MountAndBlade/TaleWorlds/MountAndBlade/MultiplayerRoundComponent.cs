// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerRoundComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerRoundComponent : MissionNetwork, IRoundComponent, IMissionBehavior
  {
    public const int RoundEndDelayTime = 3;
    public const int RoundEndWaitTime = 8;
    public const int MatchEndWaitTime = 5;
    public const int WarmupEndWaitTime = 30;
    private MissionMultiplayerGameModeBaseClient _gameModeClient;

    public event Action OnRoundStarted;

    public event Action OnPreparationEnded;

    public event Action OnPreRoundEnding;

    public event Action OnRoundEnding;

    public event Action OnPostRoundEnded;

    public event Action OnCurrentRoundStateChanged;

    public float RemainingRoundTime => this._gameModeClient.TimerComponent.GetRemainingTime(true);

    public float LastRoundEndRemainingTime { get; private set; }

    public MultiplayerRoundState CurrentRoundState { get; private set; }

    public int RoundCount { get; private set; }

    public BattleSideEnum RoundWinner { get; private set; }

    public RoundEndReason RoundEndReason { get; private set; }

    public override void AfterStart()
    {
      GameNetwork.AddNetworkHandler((IUdpNetworkHandler) this);
      this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      this._gameModeClient = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBaseClient>();
    }

    protected override void OnUdpNetworkHandlerClose() => this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      if (!GameNetwork.IsClient)
        return;
      handlerRegisterer.Register<RoundStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundStateChange>(this.HandleServerEventChangeRoundState));
      handlerRegisterer.Register<RoundCountChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundCountChange>(this.HandleServerEventRoundCountChange));
      handlerRegisterer.Register<RoundWinnerChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundWinnerChange>(this.HandleServerEventRoundWinnerChange));
      handlerRegisterer.Register<RoundEndReasonChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundEndReasonChange>(this.HandleServerEventRoundEndReasonChange));
    }

    private void HandleServerEventChangeRoundState(RoundStateChange message)
    {
      if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
        this.LastRoundEndRemainingTime = (float) message.RemainingTimeOnPreviousState;
      this.CurrentRoundState = message.RoundState;
      switch (this.CurrentRoundState)
      {
        case MultiplayerRoundState.Preparation:
          this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, (float) MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue());
          if (this.OnRoundStarted != null)
          {
            this.OnRoundStarted();
            break;
          }
          break;
        case MultiplayerRoundState.InProgress:
          this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, (float) MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue());
          if (this.OnPreparationEnded != null)
          {
            this.OnPreparationEnded();
            break;
          }
          break;
        case MultiplayerRoundState.Ending:
          this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 3f);
          if (this.OnPreRoundEnding != null)
            this.OnPreRoundEnding();
          if (this.OnRoundEnding != null)
          {
            this.OnRoundEnding();
            break;
          }
          break;
        case MultiplayerRoundState.Ended:
          this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 5f);
          if (this.OnPostRoundEnded != null)
          {
            this.OnPostRoundEnded();
            break;
          }
          break;
        case MultiplayerRoundState.MatchEnded:
          this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 5f);
          break;
      }
      Action roundStateChanged = this.OnCurrentRoundStateChanged;
      if (roundStateChanged == null)
        return;
      roundStateChanged();
    }

    private void HandleServerEventRoundCountChange(RoundCountChange message) => this.RoundCount = message.RoundCount;

    private void HandleServerEventRoundWinnerChange(RoundWinnerChange message) => this.RoundWinner = message.RoundWinner;

    private void HandleServerEventRoundEndReasonChange(RoundEndReasonChange message) => this.RoundEndReason = message.RoundEndReason;
  }
}
