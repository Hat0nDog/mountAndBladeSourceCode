// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.PeerExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public static class PeerExtensions
  {
    public static void SendExistingObjects(this NetworkCommunicator peer, Mission mission) => MBAPI.IMBPeer.SendExistingObjects(peer.Index, mission.Pointer);

    public static VirtualPlayer GetPeer(this PeerComponent peerComponent) => peerComponent.Peer;

    public static NetworkCommunicator GetNetworkPeer(
      this PeerComponent peerComponent)
    {
      return peerComponent.Peer.Communicator as NetworkCommunicator;
    }

    public static T GetComponent<T>(this NetworkCommunicator networkPeer) where T : PeerComponent => networkPeer.VirtualPlayer.GetComponent<T>();

    public static void RemoveComponent<T>(this NetworkCommunicator networkPeer, bool synched = true) where T : PeerComponent => networkPeer.VirtualPlayer.RemoveComponent<T>();

    public static void RemoveComponent(
      this NetworkCommunicator networkPeer,
      PeerComponent component)
    {
      networkPeer.VirtualPlayer.RemoveComponent(component);
    }

    public static PeerComponent GetComponent(
      this NetworkCommunicator networkPeer,
      uint componentId)
    {
      return networkPeer.VirtualPlayer.GetComponent(componentId);
    }

    public static void AddComponent(this NetworkCommunicator networkPeer, Type peerComponentType) => networkPeer.VirtualPlayer.AddComponent(peerComponentType);

    public static void AddComponent(this NetworkCommunicator networkPeer, uint componentId) => networkPeer.VirtualPlayer.AddComponent(componentId);

    public static T AddComponent<T>(this NetworkCommunicator networkPeer) where T : PeerComponent, new() => networkPeer.VirtualPlayer.AddComponent<T>();
  }
}
