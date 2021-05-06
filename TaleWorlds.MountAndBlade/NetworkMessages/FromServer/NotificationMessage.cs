// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.NotificationMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class NotificationMessage : GameNetworkMessage
  {
    public int Message { get; private set; }

    public int ParameterOne { get; private set; }

    public int ParameterTwo { get; private set; }

    private bool HasParameterOne => this.ParameterOne != -1;

    private bool HasParameterTwo => this.ParameterOne != -1;

    public NotificationMessage(int message, int param1, int param2)
    {
      this.Message = message;
      this.ParameterOne = param1;
      this.ParameterTwo = param2;
    }

    public NotificationMessage()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.Message, CompressionMission.MultiplayerNotificationCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.HasParameterOne);
      if (!this.HasParameterOne)
        return;
      GameNetworkMessage.WriteIntToPacket(this.ParameterOne, CompressionMission.MultiplayerNotificationParameterCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.HasParameterTwo);
      if (!this.HasParameterTwo)
        return;
      GameNetworkMessage.WriteIntToPacket(this.ParameterTwo, CompressionMission.MultiplayerNotificationParameterCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.ParameterOne = this.ParameterTwo = -1;
      this.Message = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationCompressionInfo, ref bufferReadValid);
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
      {
        this.ParameterOne = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationParameterCompressionInfo, ref bufferReadValid);
        if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
          this.ParameterTwo = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MultiplayerNotificationParameterCompressionInfo, ref bufferReadValid);
      }
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Messaging;

    protected override string OnGetLogFormat() => "Receiving message: " + (object) this.Message + (this.HasParameterOne ? (object) (" With first parameter: " + (object) this.ParameterOne) : (object) "") + (this.HasParameterTwo ? (object) (" and second parameter: " + (object) this.ParameterTwo) : (object) "");
  }
}
