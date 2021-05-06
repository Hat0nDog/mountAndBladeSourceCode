// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CreateMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreateMissionObject : GameNetworkMessage
  {
    public MissionObjectId ObjectId { get; private set; }

    public string Prefab { get; private set; }

    public MatrixFrame Frame { get; private set; }

    public List<MissionObjectId> ChildObjectIds { get; private set; }

    public CreateMissionObject(
      MissionObjectId objectId,
      string prefab,
      MatrixFrame frame,
      List<MissionObjectId> childObjectIds)
    {
      this.ObjectId = objectId;
      this.Prefab = prefab;
      this.Frame = frame;
      this.ChildObjectIds = childObjectIds;
    }

    public CreateMissionObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.ObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid);
      this.Prefab = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
      int capacity = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.EntityChildCountCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this.ChildObjectIds = new List<MissionObjectId>(capacity);
        for (int index = 0; index < capacity; ++index)
        {
          if (bufferReadValid)
            this.ChildObjectIds.Add(GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid));
        }
      }
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectIdToPacket(this.ObjectId);
      GameNetworkMessage.WriteStringToPacket(this.Prefab);
      GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
      GameNetworkMessage.WriteIntToPacket(this.ChildObjectIds.Count, CompressionBasic.EntityChildCountCompressionInfo);
      foreach (MissionObjectId childObjectId in this.ChildObjectIds)
        GameNetworkMessage.WriteMissionObjectIdToPacket(childObjectId);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Create a MissionObject with index: " + (object) this.ObjectId + " from prefab: " + this.Prefab + " at frame: " + (object) this.Frame;
  }
}
