// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.NetworkCommunicator
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public sealed class NetworkCommunicator : ICommunicator
  {
    private double _averagePingInMilliseconds;
    private double _averageLossPercent;
    private Agent _controlledAgent;
    private bool _isServerPeer;
    private bool _isSynchronized;
    private ServerPerformanceState _serverPerformanceProblemState;

    public static event Action<PeerComponent> OnPeerComponentAdded;

    public static event Action<NetworkCommunicator> OnPeerSynchronized;

    public static event Action<NetworkCommunicator> OnPeerAveragePingUpdated;

    public VirtualPlayer VirtualPlayer { get; }

    public PlayerConnectionInfo PlayerConnectionInfo { get; private set; }

    public bool QuitFromMission { get; set; }

    public int SessionKey { get; internal set; }

    public double AveragePingInMilliseconds
    {
      get => this._averagePingInMilliseconds;
      private set
      {
        if (value == this._averagePingInMilliseconds)
          return;
        this._averagePingInMilliseconds = value;
        Action<NetworkCommunicator> averagePingUpdated = NetworkCommunicator.OnPeerAveragePingUpdated;
        if (averagePingUpdated == null)
          return;
        averagePingUpdated(this);
      }
    }

    public double AverageLossPercent
    {
      get => this._averageLossPercent;
      private set
      {
        if (value == this._averageLossPercent)
          return;
        this._averageLossPercent = value;
      }
    }

    public bool IsMine => GameNetwork.MyPeer == this;

    public bool IsAdmin { get; private set; }

    public int Index => this.VirtualPlayer.Index;

    public string UserName => this.VirtualPlayer.UserName;

    public Agent ControlledAgent
    {
      get => this._controlledAgent;
      set
      {
        this._controlledAgent = value;
        if (!GameNetwork.IsServer)
          return;
        Mission mission = value?.Mission;
        UIntPtr missionPointer = mission != null ? mission.Pointer : UIntPtr.Zero;
        int agentIndex = value == null ? -1 : value.Index;
        MBAPI.IMBPeer.SetControlledAgent(this.Index, missionPointer, agentIndex);
      }
    }

    public bool IsMuted { get; set; }

    public int ForcedAvatarIndex { get; set; } = -1;

    public bool IsNetworkActive => true;

    public bool IsConnectionActive => MBAPI.IMBPeer.IsActive(this.Index);

    public bool IsSynchronized
    {
      get => GameNetwork.IsServer ? MBAPI.IMBPeer.GetIsSynchronized(this.Index) : this._isSynchronized;
      set
      {
        if (value == this._isSynchronized)
          return;
        if (GameNetwork.IsServer)
          MBAPI.IMBPeer.SetIsSynchronized(this.Index, value);
        this._isSynchronized = value;
        if (this._isSynchronized)
        {
          Action<NetworkCommunicator> peerSynchronized = NetworkCommunicator.OnPeerSynchronized;
          if (peerSynchronized != null)
            peerSynchronized(this);
        }
        if (!GameNetwork.IsServer || this.IsServerPeer)
          return;
        GameNetwork.BeginModuleEventAsServer(this);
        GameNetwork.WriteMessage((GameNetworkMessage) new SynchronizingDone(this, value));
        GameNetwork.EndModuleEventAsServer();
        if (value)
          MBDebug.Print("Server: " + this.UserName + " is now synchronized.", debugFilter: 17179869184UL);
        else
          MBDebug.Print("Server: " + this.UserName + " is not synchronized.", debugFilter: 17179869184UL);
      }
    }

    public bool IsServerPeer => this._isServerPeer;

    public ServerPerformanceState ServerPerformanceProblemState
    {
      get => this._serverPerformanceProblemState;
      private set
      {
        if (value == this._serverPerformanceProblemState)
          return;
        this._serverPerformanceProblemState = value;
      }
    }

    private NetworkCommunicator(int index, string name) => this.VirtualPlayer = new VirtualPlayer(index, name, (ICommunicator) this);

    internal static NetworkCommunicator CreateAsServer(
      PlayerConnectionInfo playerConnectionInfo,
      int index,
      bool isAdmin)
    {
      NetworkCommunicator networkPeer = new NetworkCommunicator(index, playerConnectionInfo.Name);
      networkPeer.PlayerConnectionInfo = playerConnectionInfo;
      networkPeer.IsAdmin = isAdmin;
      MBNetworkPeer data = new MBNetworkPeer(networkPeer);
      MBAPI.IMBPeer.SetUserData(index, data);
      return networkPeer;
    }

    internal static NetworkCommunicator CreateAsClient(string name, int index) => new NetworkCommunicator(index, name);

    void ICommunicator.OnAddComponent(PeerComponent component)
    {
      if (GameNetwork.IsServer)
      {
        if (!this.IsServerPeer)
        {
          GameNetwork.BeginModuleEventAsServer(this);
          GameNetwork.WriteMessage((GameNetworkMessage) new AddPeerComponent(this, component.TypeId));
          GameNetwork.EndModuleEventAsServer();
        }
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new AddPeerComponent(this, component.TypeId));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord, this);
      }
      Action<PeerComponent> peerComponentAdded = NetworkCommunicator.OnPeerComponentAdded;
      if (peerComponentAdded == null)
        return;
      peerComponentAdded(component);
    }

    void ICommunicator.OnRemoveComponent(PeerComponent component)
    {
      if (!GameNetwork.IsServer)
        return;
      if (!this.IsServerPeer)
      {
        GameNetwork.BeginModuleEventAsServer(this);
        GameNetwork.WriteMessage((GameNetworkMessage) new RemovePeerComponent(this, component.TypeId));
        GameNetwork.EndModuleEventAsServer();
      }
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new RemovePeerComponent(this, component.TypeId));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord, this);
    }

    void ICommunicator.OnSynchronizeComponentTo(
      VirtualPlayer peer,
      PeerComponent component)
    {
      GameNetwork.BeginModuleEventAsServer(peer);
      GameNetwork.WriteMessage((GameNetworkMessage) new AddPeerComponent(this, component.TypeId));
      GameNetwork.EndModuleEventAsServer();
    }

    internal void SetServerPeer(bool serverPeer) => this._isServerPeer = serverPeer;

    internal double RefreshAndGetAveragePingInMilliseconds()
    {
      this.AveragePingInMilliseconds = MBAPI.IMBPeer.GetAveragePingInMilliseconds(this.Index);
      return this.AveragePingInMilliseconds;
    }

    internal void SetAveragePingInMillisecondsAsClient(double pingValue)
    {
      this.AveragePingInMilliseconds = pingValue;
      this.ControlledAgent?.SetAveragePingInMilliseconds(this.AveragePingInMilliseconds);
    }

    internal double RefreshAndGetAverageLossPercent()
    {
      this.AverageLossPercent = MBAPI.IMBPeer.GetAverageLossPercent(this.Index);
      return this.AverageLossPercent;
    }

    internal void SetAverageLossPercentAsClient(double lossValue) => this.AverageLossPercent = lossValue;

    internal void SetServerPerformanceProblemStateAsClient(
      ServerPerformanceState serverPerformanceProblemState)
    {
      this.ServerPerformanceProblemState = serverPerformanceProblemState;
    }

    public void SetRelevantGameOptions(bool sendMeBloodEvents, bool sendMeSoundEvents) => MBAPI.IMBPeer.SetRelevantGameOptions(this.Index, sendMeBloodEvents, sendMeSoundEvents);

    public uint GetHost() => MBAPI.IMBPeer.GetHost(this.Index);

    public uint GetReversedHost() => MBAPI.IMBPeer.GetReversedHost(this.Index);

    public ushort GetPort() => MBAPI.IMBPeer.GetPort(this.Index);
  }
}
