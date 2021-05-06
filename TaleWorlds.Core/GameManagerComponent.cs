﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameManagerComponent
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public abstract class GameManagerComponent : IEntityComponent
  {
    public GameManagerBase GameManager { get; internal set; }

    void IEntityComponent.OnInitialize() => this.OnInitialize();

    protected virtual void OnInitialize()
    {
    }

    void IEntityComponent.OnFinalize() => this.OnFinalize();

    protected virtual void OnFinalize()
    {
    }

    protected internal virtual void OnTick()
    {
    }

    protected internal virtual void OnPlayerDisconnect(VirtualPlayer peer)
    {
    }

    protected internal virtual void OnEarlyPlayerConnect(VirtualPlayer peer)
    {
    }

    protected internal virtual void OnPlayerConnect(VirtualPlayer peer)
    {
    }

    protected internal virtual void OnGameNetworkBegin()
    {
    }

    protected internal virtual void OnGameNetworkEnd()
    {
    }
  }
}
