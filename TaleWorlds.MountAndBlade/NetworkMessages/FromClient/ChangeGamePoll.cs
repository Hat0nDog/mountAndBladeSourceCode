// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.ChangeGamePoll
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class ChangeGamePoll : GameNetworkMessage
  {
    public string GameType { get; private set; }

    public string Map { get; private set; }

    public ChangeGamePoll(string gameType, string map)
    {
      this.GameType = gameType;
      this.Map = map;
    }

    public ChangeGamePoll()
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

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Poll Requested: Change Map to: " + this.Map + " and GameType to: " + this.GameType;
  }
}
