// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectImpulse
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectImpulse : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public bool LocalSpace { get; private set; }

    public Vec3 Position { get; private set; }

    public Vec3 Impulse { get; private set; }

    public SetMissionObjectImpulse(
      MissionObject missionObject,
      Vec3 position,
      Vec3 impulse,
      bool localSpace)
    {
      this.MissionObject = missionObject;
      this.Position = position;
      this.Impulse = impulse;
      this.LocalSpace = localSpace;
    }

    public SetMissionObjectImpulse()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.LocalSpace = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec3FromPacket(this.LocalSpace ? CompressionBasic.LocalPositionCompressionInfo : CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      this.Impulse = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ImpulseCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.LocalSpace);
      GameNetworkMessage.WriteVec3ToPacket(this.Position, this.LocalSpace ? CompressionBasic.LocalPositionCompressionInfo : CompressionBasic.PositionCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(this.Impulse, CompressionBasic.ImpulseCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Set impulse on MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name;
  }
}
