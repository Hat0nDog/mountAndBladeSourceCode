// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.PlayerConnectionInfo
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public class PlayerConnectionInfo
  {
    private Dictionary<string, object> _parameters;

    public PlayerConnectionInfo() => this._parameters = new Dictionary<string, object>();

    public void AddParameter(string name, object parameter)
    {
      if (this._parameters.ContainsKey(name))
        return;
      this._parameters.Add(name, parameter);
    }

    public T GetParameter<T>(string name) where T : class => this._parameters.ContainsKey(name) ? this._parameters[name] as T : default (T);

    public int SessionKey { get; set; }

    public string Name { get; set; }

    public NetworkCommunicator NetworkPeer { get; set; }
  }
}
