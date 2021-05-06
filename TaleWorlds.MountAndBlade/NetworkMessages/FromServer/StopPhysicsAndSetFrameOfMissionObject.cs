// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.StopPhysicsAndSetFrameOfMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class StopPhysicsAndSetFrameOfMissionObject : GameNetworkMessage
  {
    public MissionObjectId ObjectId { get; private set; }

    public MissionObject Parent { get; private set; }

    public MatrixFrame Frame { get; private set; }

    public StopPhysicsAndSetFrameOfMissionObject(
      MissionObjectId objectId,
      MissionObject parent,
      MatrixFrame frame)
    {
      this.ObjectId = objectId;
      this.Parent = parent;
      this.Frame = frame;
    }

    public StopPhysicsAndSetFrameOfMissionObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid);
      this.Parent = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.Frame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.Parent);
      GameNetworkMessage.WriteNonUniformTransformToPacket(this.Frame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Stop physics and set frame of MissionObject with ID: " + (object) this.ObjectId + " Parent Index: " + (this.Parent?.Id.ToString() ?? "-1");
  }
}
