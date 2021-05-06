// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.NetworkStatusReplicationComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  internal sealed class NetworkStatusReplicationComponent : UdpNetworkComponent
  {
    public const int ComponentUniqueID = 3;
    private List<NetworkStatusReplicationComponent.NetworkStatusData> _peerData = new List<NetworkStatusReplicationComponent.NetworkStatusData>();
    private float _nextPerformanceStateTrySendTime;
    private ServerPerformanceState _lastSentPerformanceState;

    public override int UniqueComponentID => 3;

    public override void OnUdpNetworkHandlerTick(float dt)
    {
      if (!GameNetwork.IsServer)
        return;
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        if (networkPeer.IsSynchronized)
        {
          while (this._peerData.Count <= networkPeer.Index)
            this._peerData.Add(new NetworkStatusReplicationComponent.NetworkStatusData());
          double pingInMilliseconds = networkPeer.RefreshAndGetAveragePingInMilliseconds();
          NetworkStatusReplicationComponent.NetworkStatusData networkStatusData = this._peerData[networkPeer.Index];
          bool flag = (double) networkStatusData.NextPingForceSendTime <= (double) time;
          if (flag || (double) networkStatusData.NextPingTrySendTime <= (double) time)
          {
            int ping = pingInMilliseconds.Round();
            if (flag || networkStatusData.LastSentPingValue != ping)
            {
              networkStatusData.LastSentPingValue = ping;
              networkStatusData.NextPingForceSendTime = time + 10f + MBRandom.RandomFloatRanged(1.5f, 2.5f);
              GameNetwork.BeginBroadcastModuleEvent();
              GameNetwork.WriteMessage((GameNetworkMessage) new PingReplication(networkPeer, ping));
              GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None);
            }
            networkStatusData.NextPingTrySendTime = time + MBRandom.RandomFloatRanged(1.5f, 2.5f);
          }
          if (!networkPeer.IsServerPeer && (double) networkStatusData.NextLossTrySendTime <= (double) time)
          {
            networkStatusData.NextLossTrySendTime = time + MBRandom.RandomFloatRanged(1.5f, 2.5f);
            int averageLossPercent = (int) networkPeer.RefreshAndGetAverageLossPercent();
            if (networkStatusData.LastSentLossValue != averageLossPercent)
            {
              networkStatusData.LastSentLossValue = averageLossPercent;
              GameNetwork.BeginModuleEventAsServer(networkPeer);
              GameNetwork.WriteMessage((GameNetworkMessage) new LossReplicationMessage(averageLossPercent));
              GameNetwork.EndModuleEventAsServer();
            }
          }
        }
      }
      if ((double) this._nextPerformanceStateTrySendTime > (double) time)
        return;
      this._nextPerformanceStateTrySendTime = time + MBRandom.RandomFloatRanged(1.5f, 2.5f);
      ServerPerformanceState performanceState = this.GetServerPerformanceState();
      if (performanceState == this._lastSentPerformanceState)
        return;
      this._lastSentPerformanceState = performanceState;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new ServerPerformanceStateReplicationMessage(performanceState));
      GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None);
    }

    public NetworkStatusReplicationComponent()
    {
      if (!GameNetwork.IsClientOrReplay)
        return;
      NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    public override void OnUdpNetworkHandlerClose()
    {
      base.OnUdpNetworkHandlerClose();
      if (!GameNetwork.IsClientOrReplay)
        return;
      NetworkStatusReplicationComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
    }

    private static void HandleServerMessagePingReplication(PingReplication message) => message.Peer?.SetAveragePingInMillisecondsAsClient((double) message.PingValue);

    private static void HandleServerMessageLossReplication(LossReplicationMessage message)
    {
      if (!GameNetwork.IsMyPeerReady)
        return;
      GameNetwork.MyPeer.SetAverageLossPercentAsClient((double) message.LossValue);
    }

    private static void HandleServerMessageServerPerformanceStateReplication(
      ServerPerformanceStateReplicationMessage message)
    {
      if (!GameNetwork.IsMyPeerReady)
        return;
      GameNetwork.MyPeer.SetServerPerformanceProblemStateAsClient(message.ServerPerformanceProblemState);
    }

    private static void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      handlerRegisterer.Register<PingReplication>(new GameNetworkMessage.ServerMessageHandlerDelegate<PingReplication>(NetworkStatusReplicationComponent.HandleServerMessagePingReplication));
      handlerRegisterer.Register<LossReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<LossReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageLossReplication));
      handlerRegisterer.Register<ServerPerformanceStateReplicationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerPerformanceStateReplicationMessage>(NetworkStatusReplicationComponent.HandleServerMessageServerPerformanceStateReplication));
    }

    private ServerPerformanceState GetServerPerformanceState()
    {
      if (Mission.Current == null)
        return ServerPerformanceState.High;
      float averageFps = Mission.Current.GetAverageFps();
      if ((double) averageFps >= 50.0)
        return ServerPerformanceState.High;
      return (double) averageFps >= 30.0 ? ServerPerformanceState.Medium : ServerPerformanceState.Low;
    }

    private class NetworkStatusData
    {
      public float NextPingForceSendTime;
      public float NextPingTrySendTime;
      public int LastSentPingValue = -1;
      public float NextLossTrySendTime;
      public int LastSentLossValue;
    }
  }
}
