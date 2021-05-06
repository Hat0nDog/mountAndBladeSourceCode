// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Messages.GameNetworkMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
  public abstract class GameNetworkMessage
  {
    private static readonly Encoding StringEncoding = (Encoding) new UTF8Encoding();

    internal int MessageId { get; set; }

    internal void Write()
    {
      DebugNetworkEventStatistics.StartEvent(this.GetType().Name, this.MessageId);
      if (GameNetwork.IsClientOrReplay)
        GameNetworkMessage.WriteIntToPacket(this.MessageId, CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo);
      else
        GameNetworkMessage.WriteIntToPacket(this.MessageId, CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo);
      this.OnWrite();
      DebugNetworkEventStatistics.EndEvent();
    }

    protected abstract void OnWrite();

    internal bool Read() => this.OnRead();

    protected abstract bool OnRead();

    internal MultiplayerMessageFilter GetLogFilter() => this.OnGetLogFilter();

    protected abstract MultiplayerMessageFilter OnGetLogFilter();

    internal string GetLogFormat() => this.OnGetLogFormat();

    protected abstract string OnGetLogFormat();

    public static bool IsClientMissionOver => GameNetwork.IsClient && !NetworkMain.GameClient.IsInGame;

    public static bool ReadBoolFromPacket(ref bool bufferReadValid)
    {
      CompressionInfo.Integer compressionInfo = new CompressionInfo.Integer(0, 1);
      int output = 0;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out output);
      return (uint) output > 0U;
    }

    public static void WriteBoolToPacket(bool value)
    {
      CompressionInfo.Integer compressionInfo = new CompressionInfo.Integer(0, 1);
      MBAPI.IMBNetwork.WriteIntToPacket(value ? 1 : 0, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static int ReadIntFromPacket(
      CompressionInfo.Integer compressionInfo,
      ref bool bufferReadValid)
    {
      int output = 0;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static void WriteIntToPacket(int value, CompressionInfo.Integer compressionInfo)
    {
      MBAPI.IMBNetwork.WriteIntToPacket(value, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static uint ReadUintFromPacket(
      CompressionInfo.UnsignedInteger compressionInfo,
      ref bool bufferReadValid)
    {
      uint output = 0;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUintFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static void WriteUintToPacket(
      uint value,
      CompressionInfo.UnsignedInteger compressionInfo)
    {
      MBAPI.IMBNetwork.WriteUintToPacket(value, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static long ReadLongFromPacket(
      CompressionInfo.LongInteger compressionInfo,
      ref bool bufferReadValid)
    {
      long output = 0;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadLongFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static void WriteLongToPacket(long value, CompressionInfo.LongInteger compressionInfo)
    {
      MBAPI.IMBNetwork.WriteLongToPacket(value, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static ulong ReadUlongFromPacket(
      CompressionInfo.UnsignedLongInteger compressionInfo,
      ref bool bufferReadValid)
    {
      ulong output = 0;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUlongFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static void WriteUlongToPacket(
      ulong value,
      CompressionInfo.UnsignedLongInteger compressionInfo)
    {
      MBAPI.IMBNetwork.WriteUlongToPacket(value, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static float ReadFloatFromPacket(
      CompressionInfo.Float compressionInfo,
      ref bool bufferReadValid)
    {
      float output = 0.0f;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadFloatFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static void WriteFloatToPacket(float value, CompressionInfo.Float compressionInfo)
    {
      MBAPI.IMBNetwork.WriteFloatToPacket(value, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static string ReadStringFromPacket(ref bool bufferReadValid)
    {
      byte[] numArray = new byte[1024];
      int count = GameNetworkMessage.ReadByteArrayFromPacket(numArray, 0, 1024, ref bufferReadValid);
      return GameNetworkMessage.StringEncoding.GetString(numArray, 0, count);
    }

    public static void WriteStringToPacket(string value)
    {
      byte[] numArray = string.IsNullOrEmpty(value) ? new byte[0] : GameNetworkMessage.StringEncoding.GetBytes(value);
      GameNetworkMessage.WriteByteArrayToPacket(numArray, 0, numArray.Length);
    }

    public static int ReadByteArrayFromPacket(
      byte[] buffer,
      int offset,
      int bufferCapacity,
      ref bool bufferReadValid)
    {
      return MBAPI.IMBNetwork.ReadByteArrayFromPacket(buffer, offset, bufferCapacity, ref bufferReadValid);
    }

    public static void WriteByteArrayToPacket(byte[] value, int offset, int size)
    {
      MBAPI.IMBNetwork.WriteByteArrayToPacket(value, offset, size);
      DebugNetworkEventStatistics.AddDataToStatistic(Math.Min(size, 1024) + 10);
    }

    public static MBActionSet ReadActionSetReferenceFromPacket(
      CompressionInfo.Integer compressionInfo,
      ref bool bufferReadValid)
    {
      int output = MBActionSet.InvalidActionSet.Index;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out output);
      return new MBActionSet(output);
    }

    public static void WriteActionSetReferenceToPacket(
      MBActionSet actionSet,
      CompressionInfo.Integer compressionInfo)
    {
      MBAPI.IMBNetwork.WriteIntToPacket(actionSet.Index, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static int ReadAgentIndexFromPacket(
      CompressionInfo.Integer compressionInfo,
      ref bool bufferReadValid)
    {
      int output = -1;
      bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out output);
      return output;
    }

    public static Agent ReadAgentReferenceFromPacket(ref bool bufferReadValid, bool canBeNull = false)
    {
      int agentId = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
      if (!bufferReadValid || GameNetworkMessage.IsClientMissionOver)
        return (Agent) null;
      Agent agentWithIndex = Mission.Current.FindAgentWithIndex(agentId);
      if (agentWithIndex != null || agentId < 0)
        return agentWithIndex;
      Debug.Print("Agent with index: " + (object) agentId + " could not be found while reading reference from packet.");
      return agentWithIndex;
    }

    public static void WriteAgentReferenceToPacket(Agent value)
    {
      CompressionInfo.Integer agentCompressionInfo = CompressionMission.AgentCompressionInfo;
      MBAPI.IMBNetwork.WriteIntToPacket(value != null ? value.Index : -1, ref agentCompressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(agentCompressionInfo.GetNumBits());
    }

    public static MBObjectBase ReadObjectReferenceFromPacket(
      MBObjectManager objectManager,
      CompressionInfo.UnsignedInteger compressionInfo,
      ref bool bufferReadValid)
    {
      uint id = GameNetworkMessage.ReadUintFromPacket(compressionInfo, ref bufferReadValid);
      if (!bufferReadValid || id <= 0U)
        return (MBObjectBase) null;
      MBGUID objectId = new MBGUID(id);
      return objectManager.GetObject(objectId);
    }

    public static void WriteObjectReferenceToPacket(
      MBObjectBase value,
      CompressionInfo.UnsignedInteger compressionInfo)
    {
      MBAPI.IMBNetwork.WriteUintToPacket(value != null ? value.Id.InternalValue : 0U, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static VirtualPlayer ReadVirtualPlayerReferenceToPacket(
      ref bool bufferReadValid,
      bool canReturnNull = false)
    {
      int index = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      return ((index < 0 ? 0 : (!GameNetworkMessage.IsClientMissionOver ? 1 : 0)) & (bufferReadValid ? 1 : 0)) != 0 ? MBNetwork.VirtualPlayers[index] : (VirtualPlayer) null;
    }

    public static NetworkCommunicator ReadNetworkPeerReferenceFromPacket(
      ref bool bufferReadValid,
      bool canReturnNull = false)
    {
      int index = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      return ((index < 0 ? 0 : (!GameNetworkMessage.IsClientMissionOver ? 1 : 0)) & (bufferReadValid ? 1 : 0)) != 0 ? GameNetwork.FindNetworkPeer(index) : (NetworkCommunicator) null;
    }

    public static void WriteVirtualPlayerReferenceToPacket(VirtualPlayer virtualPlayer)
    {
      MBAPI.IMBNetwork.WriteIntToPacket(virtualPlayer != null ? virtualPlayer.Index : -1, ref CompressionBasic.PlayerCompressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(CompressionBasic.PlayerCompressionInfo.GetNumBits());
    }

    public static void WriteNetworkPeerReferenceToPacket(NetworkCommunicator value)
    {
      MBAPI.IMBNetwork.WriteIntToPacket(value != null ? value.Index : -1, ref CompressionBasic.PlayerCompressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(CompressionBasic.PlayerCompressionInfo.GetNumBits());
    }

    public static MBTeam ReadMBTeamReferenceFromPacket(
      CompressionInfo.Integer compressionInfo,
      ref bool bufferReadValid)
    {
      int index = GameNetworkMessage.ReadIntFromPacket(compressionInfo, ref bufferReadValid);
      return !GameNetworkMessage.IsClientMissionOver & bufferReadValid ? new MBTeam(Mission.Current, index) : MBTeam.InvalidTeam;
    }

    public static Team ReadTeamReferenceFromPacket(ref bool bufferReadValid)
    {
      MBTeam mbTeam = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
      return mbTeam.IsValid & bufferReadValid ? Mission.Current.Teams.Find(mbTeam) : Team.Invalid;
    }

    public static void WriteMBTeamReferenceToPacket(
      MBTeam value,
      CompressionInfo.Integer compressionInfo)
    {
      MBAPI.IMBNetwork.WriteIntToPacket(value.Index, ref compressionInfo);
      DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
    }

    public static void WriteTeamReferenceToPacket(Team value) => GameNetworkMessage.WriteMBTeamReferenceToPacket(value == null ? MBTeam.InvalidTeam : value.MBTeam, CompressionMission.TeamCompressionInfo);

    public static SynchedMissionObject ReadSynchedMissionObjectFromPacket(
      ref bool bufferReadValid)
    {
      MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      if (!bufferReadValid || missionObject == null)
        return (SynchedMissionObject) null;
      SynchedMissionObject synchedMissionObject = (SynchedMissionObject) missionObject;
      bufferReadValid = synchedMissionObject.ReadFromNetwork();
      return synchedMissionObject;
    }

    public static MissionObject ReadMissionObjectReferenceFromPacket(
      ref bool bufferReadValid)
    {
      MissionObjectId missionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid);
      if (!bufferReadValid || missionObjectId.Id == -1 || GameNetworkMessage.IsClientMissionOver)
      {
        if (missionObjectId.Id != -1)
          MBDebug.Print("Reading null MissionObject because IsClientMissionOver: " + GameNetworkMessage.IsClientMissionOver.ToString() + " valid read: " + bufferReadValid.ToString() + " MissionObject ID: " + (object) missionObjectId.Id + " runtime: " + missionObjectId.CreatedAtRuntime.ToString());
        return (MissionObject) null;
      }
      MissionObject missionObject = Mission.Current.MissionObjects.FirstOrDefault<MissionObject>((Func<MissionObject, bool>) (mo => mo.Id == missionObjectId));
      if (missionObject == null)
        MBDebug.Print("MissionObject with ID: " + (object) missionObjectId.Id + " runtime: " + missionObjectId.CreatedAtRuntime.ToString() + " could not be found.");
      return missionObject;
    }

    public static MissionObjectId ReadMissionObjectIdFromPacket(
      ref bool bufferReadValid)
    {
      bool createdAtRuntime = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return new MissionObjectId(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid), createdAtRuntime);
    }

    public void WriteSynchedMissionObjectToPacket(SynchedMissionObject value)
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) value);
      value?.WriteToNetwork();
    }

    public static void WriteMissionObjectReferenceToPacket(MissionObject value) => GameNetworkMessage.WriteMissionObjectIdToPacket(value != null ? value.Id : new MissionObjectId(-1));

    public static void WriteMissionObjectIdToPacket(MissionObjectId value)
    {
      GameNetworkMessage.WriteBoolToPacket(value.CreatedAtRuntime);
      GameNetworkMessage.WriteIntToPacket(value.Id, CompressionBasic.MissionObjectIDCompressionInfo);
    }

    public static Vec3 ReadVec3FromPacket(
      CompressionInfo.Float compressionInfo,
      ref bool bufferReadValid)
    {
      double num1 = (double) GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
      float num2 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
      float num3 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
      double num4 = (double) num2;
      double num5 = (double) num3;
      return new Vec3((float) num1, (float) num4, (float) num5);
    }

    public static void WriteVec3ToPacket(Vec3 value, CompressionInfo.Float compressionInfo)
    {
      GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
      GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
      GameNetworkMessage.WriteFloatToPacket(value.z, compressionInfo);
    }

    public static Vec2 ReadVec2FromPacket(
      CompressionInfo.Float compressionInfo,
      ref bool bufferReadValid)
    {
      return new Vec2(GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid), GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid));
    }

    public static void WriteVec2ToPacket(Vec2 value, CompressionInfo.Float compressionInfo)
    {
      GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
      GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
    }

    public static Mat3 ReadRotationMatrixFromPacket(ref bool bufferReadValid)
    {
      Vec3 s = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      Vec3 vec3_1 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      Vec3 vec3_2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      Vec3 f = vec3_1;
      Vec3 u = vec3_2;
      return new Mat3(s, f, u);
    }

    public static void WriteRotationMatrixToPacket(Mat3 value)
    {
      GameNetworkMessage.WriteVec3ToPacket(value.s, CompressionBasic.UnitVectorCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(value.f, CompressionBasic.UnitVectorCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(value.u, CompressionBasic.UnitVectorCompressionInfo);
    }

    public static MatrixFrame ReadMatrixFrameFromPacket(ref bool bufferReadValid)
    {
      Vec3 o = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      Vec3 scalingVector = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
      MatrixFrame matrixFrame = new MatrixFrame(GameNetworkMessage.ReadRotationMatrixFromPacket(ref bufferReadValid), o);
      matrixFrame.Scale(scalingVector);
      return matrixFrame;
    }

    public static void WriteMatrixFrameToPacket(MatrixFrame frame)
    {
      Vec3 scaleVector = frame.rotation.GetScaleVector();
      MatrixFrame matrixFrame = frame;
      matrixFrame.Scale(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z));
      GameNetworkMessage.WriteVec3ToPacket(matrixFrame.origin, CompressionBasic.PositionCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(scaleVector, CompressionBasic.ScaleCompressionInfo);
      GameNetworkMessage.WriteRotationMatrixToPacket(matrixFrame.rotation);
    }

    public static MatrixFrame ReadNonUniformTransformFromPacket(
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo,
      ref bool bufferReadValid)
    {
      MatrixFrame matrixFrame = GameNetworkMessage.ReadUnitTransformFromPacket(positionCompressionInfo, quaternionCompressionInfo, ref bufferReadValid);
      Vec3 scaleAmountXYZ = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
      matrixFrame.rotation.ApplyScaleLocal(scaleAmountXYZ);
      return matrixFrame;
    }

    public static void WriteNonUniformTransformToPacket(
      MatrixFrame frame,
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo)
    {
      MatrixFrame frame1 = frame;
      Vec3 vec3 = frame1.rotation.MakeUnit();
      GameNetworkMessage.WriteUnitTransformToPacket(frame1, positionCompressionInfo, quaternionCompressionInfo);
      CompressionInfo.Float scaleCompressionInfo = CompressionBasic.ScaleCompressionInfo;
      GameNetworkMessage.WriteVec3ToPacket(vec3, scaleCompressionInfo);
    }

    public static MatrixFrame ReadTransformFromPacket(
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo,
      ref bool bufferReadValid)
    {
      MatrixFrame matrixFrame = GameNetworkMessage.ReadUnitTransformFromPacket(positionCompressionInfo, quaternionCompressionInfo, ref bufferReadValid);
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
      {
        float scaleAmount = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
        matrixFrame.rotation.ApplyScaleLocal(scaleAmount);
      }
      return matrixFrame;
    }

    public static void WriteTransformToPacket(
      MatrixFrame frame,
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo)
    {
      MatrixFrame frame1 = frame;
      Vec3 vec3 = frame1.rotation.MakeUnit();
      GameNetworkMessage.WriteUnitTransformToPacket(frame1, positionCompressionInfo, quaternionCompressionInfo);
      if (vec3.x.ApproximatelyEqualsTo(1f, CompressionBasic.ScaleCompressionInfo.GetPrecision()))
      {
        GameNetworkMessage.WriteBoolToPacket(false);
      }
      else
      {
        GameNetworkMessage.WriteBoolToPacket(true);
        GameNetworkMessage.WriteFloatToPacket(vec3.x, CompressionBasic.ScaleCompressionInfo);
      }
    }

    public static MatrixFrame ReadUnitTransformFromPacket(
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo,
      ref bool bufferReadValid)
    {
      MatrixFrame matrixFrame = new MatrixFrame();
      matrixFrame.origin = GameNetworkMessage.ReadVec3FromPacket(positionCompressionInfo, ref bufferReadValid);
      Quaternion quaternion = GameNetworkMessage.ReadQuaternionFromPacket(quaternionCompressionInfo, ref bufferReadValid);
      matrixFrame.rotation = quaternion.ToMat3;
      return matrixFrame;
    }

    public static void WriteUnitTransformToPacket(
      MatrixFrame frame,
      CompressionInfo.Float positionCompressionInfo,
      CompressionInfo.Float quaternionCompressionInfo)
    {
      GameNetworkMessage.WriteVec3ToPacket(frame.origin, positionCompressionInfo);
      GameNetworkMessage.WriteQuaternionToPacket(frame.rotation.ToQuaternion(), quaternionCompressionInfo);
    }

    public static Quaternion ReadQuaternionFromPacket(
      CompressionInfo.Float compressionInfo,
      ref bool bufferReadValid)
    {
      Quaternion quaternion = new Quaternion();
      float num1 = 0.0f;
      int i1 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.OmittedQuaternionComponentIndexCompressionInfo, ref bufferReadValid);
      for (int i2 = 0; i2 < 4; ++i2)
      {
        if (i2 != i1)
        {
          quaternion[i2] = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
          num1 += quaternion[i2] * quaternion[i2];
        }
      }
      quaternion[i1] = MathF.Sqrt(1f - num1);
      double num2 = (double) quaternion.SafeNormalize();
      return quaternion;
    }

    public static void WriteQuaternionToPacket(Quaternion q, CompressionInfo.Float compressionInfo)
    {
      int i1 = -1;
      float num1 = 0.0f;
      Quaternion quaternion = q;
      double num2 = (double) quaternion.SafeNormalize();
      for (int i2 = 0; i2 < 4; ++i2)
      {
        float num3 = MathF.Abs(quaternion[i2]);
        if ((double) num3 > (double) num1)
        {
          num1 = num3;
          i1 = i2;
        }
      }
      if ((double) quaternion[i1] < 0.0)
        quaternion.Flip();
      GameNetworkMessage.WriteIntToPacket(i1, CompressionBasic.OmittedQuaternionComponentIndexCompressionInfo);
      for (int i2 = 0; i2 < 4; ++i2)
      {
        if (i2 != i1)
          GameNetworkMessage.WriteFloatToPacket(quaternion[i2], compressionInfo);
      }
    }

    public static void WriteBodyPropertiesToPacket(in BodyProperties bodyProperties)
    {
      GameNetworkMessage.WriteFloatToPacket(bodyProperties.Age, CompressionBasic.AgentAgeCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(bodyProperties.Weight, CompressionBasic.FaceKeyDataCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(bodyProperties.Build, CompressionBasic.FaceKeyDataCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart1, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart2, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart3, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart4, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart5, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart6, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart7, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart8, CompressionBasic.DebugULongNonCompressionInfo);
    }

    public static BodyProperties ReadBodyPropertiesFromPacket(ref bool bufferReadValid)
    {
      float age = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentAgeCompressionInfo, ref bufferReadValid);
      float weight = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.FaceKeyDataCompressionInfo, ref bufferReadValid);
      float build = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.FaceKeyDataCompressionInfo, ref bufferReadValid);
      ulong keyPart1 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart2 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart3 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart4 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart5 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart6 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart7 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong keyPart8 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      return bufferReadValid ? new BodyProperties(new DynamicBodyProperties(age, weight, build), new StaticBodyProperties(keyPart1, keyPart2, keyPart3, keyPart4, keyPart5, keyPart6, keyPart7, keyPart8)) : new BodyProperties();
    }

    public delegate bool ClientMessageHandlerDelegate<T>(NetworkCommunicator peer, T message) where T : GameNetworkMessage;

    public delegate void ServerMessageHandlerDelegate<T>(T message) where T : GameNetworkMessage;
  }
}
