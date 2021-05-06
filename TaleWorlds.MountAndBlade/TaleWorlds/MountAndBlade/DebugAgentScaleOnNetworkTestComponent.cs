// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DebugAgentScaleOnNetworkTestComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  internal sealed class DebugAgentScaleOnNetworkTestComponent : UdpNetworkComponent
  {
    public const int ComponentUniqueID = 1;
    private float _lastTestSendTime;

    public override int UniqueComponentID => 1;

    public override void OnUdpNetworkHandlerTick(float dt)
    {
      if (!GameNetwork.IsServer)
        return;
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      if ((double) this._lastTestSendTime >= (double) time + 10.0)
        return;
      IReadOnlyList<Agent> agents = Mission.Current.Agents;
      int count = agents.Count;
      this._lastTestSendTime = time;
      Agent toTest = agents[(int) (new Random().NextDouble() * (double) count)];
      if (!toTest.IsActive())
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new DebugAgentScaleOnNetworkTest(toTest, toTest.AgentScale));
      GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.None);
    }

    public DebugAgentScaleOnNetworkTestComponent()
    {
      if (!GameNetwork.IsClientOrReplay)
        return;
      DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    public override void OnUdpNetworkHandlerClose()
    {
      base.OnUdpNetworkHandlerClose();
      if (!GameNetwork.IsClientOrReplay)
        return;
      DebugAgentScaleOnNetworkTestComponent.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
    }

    private static void HandleServerMessageDebugAgentScaleOnNetworkTest(
      DebugAgentScaleOnNetworkTest message)
    {
      if (message.AgentToTest == null || !message.AgentToTest.IsActive())
        return;
      double precision = (double) CompressionMission.DebugScaleValueCompressionInfo.GetPrecision();
      double agentScale = (double) message.AgentToTest.AgentScale;
    }

    private static void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      new GameNetwork.NetworkMessageHandlerRegisterer(mode).Register<DebugAgentScaleOnNetworkTest>(new GameNetworkMessage.ServerMessageHandlerDelegate<DebugAgentScaleOnNetworkTest>(DebugAgentScaleOnNetworkTestComponent.HandleServerMessageDebugAgentScaleOnNetworkTest));
    }
  }
}
