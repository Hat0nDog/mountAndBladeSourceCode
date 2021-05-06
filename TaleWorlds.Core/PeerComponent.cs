// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.PeerComponent
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public abstract class PeerComponent : IEntityComponent
  {
    private VirtualPlayer _peer;

    public VirtualPlayer Peer
    {
      get => this._peer;
      set => this._peer = value;
    }

    public virtual void Initialize()
    {
    }

    public string Name => this.Peer.UserName;

    public bool IsMine => this.Peer.IsMine;

    public T GetComponent<T>() where T : PeerComponent => this.Peer.GetComponent<T>();

    public virtual void OnInitialize()
    {
    }

    public virtual void OnFinalize()
    {
    }

    public uint TypeId { get; set; }
  }
}
