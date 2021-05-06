// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.HealthAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class HealthAgentComponent : AgentComponent
  {
    public bool SyncToAllClients { get; private set; }

    public HealthAgentComponent(Agent agent, bool syncToAllClients = false)
      : base(agent)
    {
      this.SyncToAllClients = syncToAllClients;
      if (!GameNetwork.IsServerOrRecorder)
        return;
      this.Agent.OnAgentHealthChanged += new Agent.OnAgentHealthChangedDelegate(this.OnHealthChanged);
    }

    public void UpdateSyncToAllClients(bool syncToAllClients) => this.SyncToAllClients = syncToAllClients;

    private void OnHealthChanged(Agent agent, float oldHealth, float newHealth) => this.SyncHealthToClients();

    public void SyncHealthToClients()
    {
      if (this.SyncToAllClients && (!this.Agent.IsMount || this.Agent.RiderAgent != null))
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentHealth(this.Agent, (int) this.Agent.Health));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      else
      {
        NetworkCommunicator networkCommunicator;
        if (!this.Agent.IsMount)
        {
          MissionPeer missionPeer = this.Agent.MissionPeer;
          networkCommunicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
        }
        else
        {
          Agent riderAgent = this.Agent.RiderAgent;
          if (riderAgent == null)
          {
            networkCommunicator = (NetworkCommunicator) null;
          }
          else
          {
            MissionPeer missionPeer = riderAgent.MissionPeer;
            networkCommunicator = missionPeer != null ? missionPeer.GetNetworkPeer() : (NetworkCommunicator) null;
          }
        }
        NetworkCommunicator communicator = networkCommunicator;
        if (communicator == null || communicator.IsServerPeer)
          return;
        GameNetwork.BeginModuleEventAsServer(communicator);
        GameNetwork.WriteMessage((GameNetworkMessage) new SetAgentHealth(this.Agent, (int) this.Agent.Health));
        GameNetwork.EndModuleEventAsServer();
      }
    }
  }
}
