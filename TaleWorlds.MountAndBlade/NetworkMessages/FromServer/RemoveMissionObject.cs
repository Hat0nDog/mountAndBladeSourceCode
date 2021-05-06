// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RemoveMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RemoveMissionObject : GameNetworkMessage
  {
    public MissionObjectId ObjectId { get; private set; }

    public RemoveMissionObject(MissionObjectId objectId) => this.ObjectId = objectId;

    public RemoveMissionObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Remove MissionObject with ID: " + (object) this.ObjectId;
  }
}
