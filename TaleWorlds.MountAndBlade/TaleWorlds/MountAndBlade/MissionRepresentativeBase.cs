// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRepresentativeBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionRepresentativeBase : PeerComponent
  {
    private MissionPeer _missionPeer;

    protected MissionRepresentativeBase.PlayerTypes PlayerType
    {
      get
      {
        if (!this.Peer.Communicator.IsNetworkActive)
          return MissionRepresentativeBase.PlayerTypes.Bot;
        return !this.Peer.Communicator.IsServerPeer ? MissionRepresentativeBase.PlayerTypes.Client : MissionRepresentativeBase.PlayerTypes.Server;
      }
    }

    public Agent ControlledAgent { get; private set; }

    public int Gold { get; protected set; }

    public MissionPeer MissionPeer
    {
      get
      {
        if (this._missionPeer == null)
          this._missionPeer = this.GetComponent<MissionPeer>();
        return this._missionPeer;
      }
    }

    public abstract void OnAgentInteraction(Agent targetAgent);

    public void SetAgent(Agent agent)
    {
      this.ControlledAgent = agent;
      if (this.ControlledAgent == null)
        return;
      this.ControlledAgent.MissionRepresentative = this;
      this.OnAgentSpawned();
    }

    public virtual void OnAgentSpawned()
    {
    }

    public abstract bool IsThereAgentAction(Agent targetAgent);

    public virtual void Tick(float dt)
    {
    }

    public void UpdateGold(int gold) => this.Gold = gold;

    protected enum PlayerTypes
    {
      Bot,
      Client,
      Server,
    }
  }
}
