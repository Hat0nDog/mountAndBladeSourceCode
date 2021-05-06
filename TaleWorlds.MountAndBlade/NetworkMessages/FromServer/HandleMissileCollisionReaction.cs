// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.HandleMissileCollisionReaction
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class HandleMissileCollisionReaction : GameNetworkMessage
  {
    public int MissileIndex { get; private set; }

    public Mission.MissileCollisionReaction CollisionReaction { get; private set; }

    public MatrixFrame AttachLocalFrame { get; private set; }

    public Agent AttackerAgent { get; private set; }

    public Agent AttachedAgent { get; private set; }

    public bool AttachedToShield { get; private set; }

    public sbyte AttachedBoneIndex { get; private set; }

    public MissionObject AttachedMissionObject { get; private set; }

    public Vec3 BounceBackVelocity { get; private set; }

    public Vec3 BounceBackAngularVelocity { get; private set; }

    public int ForcedSpawnIndex { get; private set; }

    public HandleMissileCollisionReaction(
      int missileIndex,
      Mission.MissileCollisionReaction collisionReaction,
      MatrixFrame attachLocalFrame,
      Agent attackerAgent,
      Agent attachedAgent,
      bool attachedToShield,
      sbyte attachedBoneIndex,
      MissionObject attachedMissionObject,
      Vec3 bounceBackVelocity,
      Vec3 bounceBackAngularVelocity,
      int forcedSpawnIndex)
    {
      this.MissileIndex = missileIndex;
      this.CollisionReaction = collisionReaction;
      this.AttachLocalFrame = attachLocalFrame;
      this.AttackerAgent = attackerAgent;
      this.AttachedAgent = attachedAgent;
      this.AttachedToShield = attachedToShield;
      this.AttachedBoneIndex = attachedBoneIndex;
      this.AttachedMissionObject = attachedMissionObject;
      this.BounceBackVelocity = bounceBackVelocity;
      this.BounceBackAngularVelocity = bounceBackAngularVelocity;
      this.ForcedSpawnIndex = forcedSpawnIndex;
    }

    public HandleMissileCollisionReaction()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissileIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCompressionInfo, ref bufferReadValid);
      this.CollisionReaction = (Mission.MissileCollisionReaction) GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCollisionReactionCompressionInfo, ref bufferReadValid);
      this.AttackerAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.AttachedAgent = (Agent) null;
      this.AttachedToShield = false;
      this.AttachedBoneIndex = (sbyte) -1;
      this.AttachedMissionObject = (MissionObject) null;
      if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
      {
        if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        {
          this.AttachedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
          this.AttachedToShield = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
          if (!this.AttachedToShield)
            this.AttachedBoneIndex = (sbyte) GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref bufferReadValid);
        }
        else
          this.AttachedMissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      }
      this.AttachLocalFrame = this.CollisionReaction == Mission.MissileCollisionReaction.BecomeInvisible || this.CollisionReaction == Mission.MissileCollisionReaction.PassThrough ? MatrixFrame.Identity : (!GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid) ? GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref bufferReadValid) : GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.BigRangeLowResLocalPositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref bufferReadValid));
      if (this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
      {
        this.BounceBackVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemVelocityCompressionInfo, ref bufferReadValid);
        this.BounceBackAngularVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemAngularVelocityCompressionInfo, ref bufferReadValid);
      }
      else
      {
        this.BounceBackVelocity = Vec3.Zero;
        this.BounceBackAngularVelocity = Vec3.Zero;
      }
      if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
        this.ForcedSpawnIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.MissileIndex, CompressionMission.MissileCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.CollisionReaction, CompressionMission.MissileCollisionReactionCompressionInfo);
      GameNetworkMessage.WriteAgentReferenceToPacket(this.AttackerAgent);
      if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
      {
        int num = this.AttachedAgent != null ? 1 : 0;
        GameNetworkMessage.WriteBoolToPacket(num != 0);
        if (num != 0)
        {
          GameNetworkMessage.WriteAgentReferenceToPacket(this.AttachedAgent);
          GameNetworkMessage.WriteBoolToPacket(this.AttachedToShield);
          if (!this.AttachedToShield)
            GameNetworkMessage.WriteIntToPacket((int) this.AttachedBoneIndex, CompressionMission.BoneIndexCompressionInfo);
        }
        else
          GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.AttachedMissionObject);
      }
      if (this.CollisionReaction != Mission.MissileCollisionReaction.BecomeInvisible && this.CollisionReaction != Mission.MissileCollisionReaction.PassThrough)
      {
        int num = this.AttachedAgent != null ? 1 : (this.AttachedMissionObject != null ? 1 : 0);
        GameNetworkMessage.WriteBoolToPacket(num != 0);
        if (num != 0)
          GameNetworkMessage.WriteNonUniformTransformToPacket(this.AttachLocalFrame, CompressionBasic.BigRangeLowResLocalPositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
        else
          GameNetworkMessage.WriteNonUniformTransformToPacket(this.AttachLocalFrame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
      }
      if (this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
      {
        GameNetworkMessage.WriteVec3ToPacket(this.BounceBackVelocity, CompressionMission.SpawnedItemVelocityCompressionInfo);
        GameNetworkMessage.WriteVec3ToPacket(this.BounceBackAngularVelocity, CompressionMission.SpawnedItemAngularVelocityCompressionInfo);
      }
      if (this.CollisionReaction != Mission.MissileCollisionReaction.Stick && this.CollisionReaction != Mission.MissileCollisionReaction.BounceBack)
        return;
      GameNetworkMessage.WriteIntToPacket(this.ForcedSpawnIndex, CompressionBasic.MissionObjectIDCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat()
    {
      object[] objArray = new object[16];
      objArray[0] = (object) "Handle Missile Collision with index: ";
      objArray[1] = (object) this.MissileIndex;
      objArray[2] = (object) " collision reaction: ";
      objArray[3] = (object) this.CollisionReaction;
      objArray[4] = (object) " AttackerAgent index: ";
      Agent attackerAgent = this.AttackerAgent;
      objArray[5] = (object) (attackerAgent != null ? attackerAgent.Index : -1);
      objArray[6] = (object) " AttachedAgent index: ";
      Agent attachedAgent = this.AttachedAgent;
      objArray[7] = (object) (attachedAgent != null ? attachedAgent.Index : -1);
      objArray[8] = (object) " AttachedToShield: ";
      objArray[9] = (object) this.AttachedToShield.ToString();
      objArray[10] = (object) " AttachedBoneIndex: ";
      objArray[11] = (object) this.AttachedBoneIndex;
      objArray[12] = (object) " AttachedMissionObject id: ";
      objArray[13] = this.AttachedMissionObject != null ? (object) this.AttachedMissionObject.Id.ToString() : (object) "-1";
      objArray[14] = (object) " ForcedSpawnIndex: ";
      objArray[15] = (object) this.ForcedSpawnIndex;
      return string.Concat(objArray);
    }
  }
}
