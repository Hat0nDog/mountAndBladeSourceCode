// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Messages.DeletePlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade.Network.Messages
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class DeletePlayer : GameNetworkMessage
  {
    public int PlayerIndex;

    public DeletePlayer(int playerIndex) => this.PlayerIndex = playerIndex;

    public DeletePlayer()
    {
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Delete player with index" + (object) this.PlayerIndex;
  }
}
