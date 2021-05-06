// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AddMissionObjectBodyFlags
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class AddMissionObjectBodyFlags : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public BodyFlags BodyFlags { get; private set; }

    public bool ApplyToChildren { get; private set; }

    public AddMissionObjectBodyFlags(
      MissionObject missionObject,
      BodyFlags bodyFlags,
      bool applyToChildren)
    {
      this.MissionObject = missionObject;
      this.BodyFlags = bodyFlags;
      this.ApplyToChildren = applyToChildren;
    }

    public AddMissionObjectBodyFlags()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.BodyFlags = (BodyFlags) GameNetworkMessage.ReadIntFromPacket(CompressionBasic.FlagsCompressionInfo, ref bufferReadValid);
      this.ApplyToChildren = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteIntToPacket((int) this.BodyFlags, CompressionBasic.FlagsCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.ApplyToChildren);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Add bodyflags: " + (object) this.BodyFlags + " to MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name + (this.ApplyToChildren ? (object) "" : (object) " and to all of its children.");
  }
}
