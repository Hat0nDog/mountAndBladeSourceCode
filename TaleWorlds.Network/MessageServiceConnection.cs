// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageServiceConnection
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Threading.Tasks;

namespace TaleWorlds.Network
{
  public abstract class MessageServiceConnection
  {
    public ConnectionState State;
    public ConnectionState OldState;

    public abstract Task SendAsync(string text);

    public abstract void Init(string address, string token);

    public string Address { get; protected set; }

    public event MessageServiceConnection.ClosedDelegate Closed;

    public event MessageServiceConnection.StateChangedDelegate StateChanged;

    public abstract void RegisterProxyClient(string name, IMessageProxyClient playerClient);

    public abstract Task StartAsync();

    public abstract Task StopAsync();

    protected void InvokeClosed()
    {
      MessageServiceConnection.ClosedDelegate closed = this.Closed;
      if (closed == null)
        return;
      Task task = closed();
    }

    protected void InvokeStateChanged(ConnectionState oldState, ConnectionState newState)
    {
      this.State = newState;
      this.OldState = oldState;
      MessageServiceConnection.StateChangedDelegate stateChanged = this.StateChanged;
      if (stateChanged == null)
        return;
      stateChanged(oldState, newState);
    }

    public delegate Task ClosedDelegate();

    public delegate void StateChangedDelegate(ConnectionState oldState, ConnectionState newState);
  }
}
