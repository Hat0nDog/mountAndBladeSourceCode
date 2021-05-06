// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Messages.CreatePlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade.Network.Messages
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreatePlayer : GameNetworkMessage
  {
    public int PlayerIndex;
    public string PlayerName;

    public CreatePlayer(int playerIndex, string playerName)
    {
      this.PlayerIndex = playerIndex;
      this.PlayerName = playerName;
    }

    public CreatePlayer()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);
      GameNetworkMessage.WriteStringToPacket(this.PlayerName);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      this.PlayerName = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Create a new player with name: " + this.PlayerName + " and index: " + (object) this.PlayerIndex;
  }
}
