// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DummyCommunicator
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class DummyCommunicator : ICommunicator
  {
    public VirtualPlayer VirtualPlayer { get; }

    public void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component)
    {
    }

    public void OnAddComponent(PeerComponent component)
    {
    }

    public void OnRemoveComponent(PeerComponent component)
    {
    }

    public bool IsNetworkActive => false;

    public bool IsConnectionActive => false;

    public bool IsServerPeer => false;

    public bool IsSynchronized
    {
      get => true;
      set
      {
      }
    }

    private DummyCommunicator(int index, string name) => this.VirtualPlayer = new VirtualPlayer(index, name, (ICommunicator) this);

    public static DummyCommunicator CreateAsServer(int index, string name) => new DummyCommunicator(index, name);

    public static DummyCommunicator CreateAsClient(string name, int index) => new DummyCommunicator(index, name);
  }
}
