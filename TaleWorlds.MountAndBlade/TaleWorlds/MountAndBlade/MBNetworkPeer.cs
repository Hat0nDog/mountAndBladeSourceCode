// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBNetworkPeer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  internal class MBNetworkPeer : DotNetObject
  {
    private NetworkCommunicator _networkPeer;

    public NetworkCommunicator NetworkPeer => this._networkPeer;

    internal MBNetworkPeer(NetworkCommunicator networkPeer) => this._networkPeer = networkPeer;
  }
}
