// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ICommunicator
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public interface ICommunicator
  {
    VirtualPlayer VirtualPlayer { get; }

    void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component);

    void OnAddComponent(PeerComponent component);

    void OnRemoveComponent(PeerComponent component);

    bool IsNetworkActive { get; }

    bool IsConnectionActive { get; }

    bool IsServerPeer { get; }

    bool IsSynchronized { get; set; }
  }
}
