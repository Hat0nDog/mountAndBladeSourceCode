// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.LoadMission
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class LoadMission : GameNetworkMessage
  {
    public string GameType { get; private set; }

    public string Map { get; private set; }

    public LoadMission(string gameType, string map)
    {
      this.GameType = gameType;
      this.Map = map;
    }

    public LoadMission()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.GameType = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      this.Map = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteStringToPacket(this.GameType);
      GameNetworkMessage.WriteStringToPacket(this.Map);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Load Mission";
  }
}
