// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBNetwork
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public static class MBNetwork
  {
    public const int MaxPlayerCount = 511;
    public static VirtualPlayer[] VirtualPlayers;

    public static INetworkCommunication NetworkViewCommunication { get; private set; }

    public static bool MultiplayerDisabled => MBNetwork.NetworkViewCommunication == null || MBNetwork.NetworkViewCommunication.MultiplayerDisabled;

    public static List<ICommunicator> NetworkPeers { get; private set; }

    public static bool LogDisabled => true;

    public static bool IsSessionActive => MBNetwork.IsServer || MBNetwork.IsClient;

    public static bool IsServer => MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsServer;

    public static bool IsClient => MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsClient;

    public static void Initialize(INetworkCommunication networkCommunication)
    {
      MBNetwork.VirtualPlayers = new VirtualPlayer[511];
      MBNetwork.NetworkPeers = new List<ICommunicator>();
      MBNetwork.NetworkViewCommunication = networkCommunication;
    }

    public static VirtualPlayer MyPeer => MBNetwork.NetworkViewCommunication != null ? MBNetwork.NetworkViewCommunication.MyPeer : (VirtualPlayer) null;
  }
}
