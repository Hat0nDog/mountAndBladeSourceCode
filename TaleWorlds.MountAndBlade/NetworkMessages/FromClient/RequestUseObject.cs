// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.RequestUseObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class RequestUseObject : GameNetworkMessage
  {
    public UsableMissionObject UsableGameObject { get; private set; }

    public int UsedObjectPreferenceIndex { get; private set; }

    public RequestUseObject(UsableMissionObject usableGameObject, int usedObjectPreferenceIndex)
    {
      this.UsableGameObject = usableGameObject;
      this.UsedObjectPreferenceIndex = usedObjectPreferenceIndex;
    }

    public RequestUseObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as UsableMissionObject;
      this.UsedObjectPreferenceIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.UsableGameObject);
      GameNetworkMessage.WriteIntToPacket(this.UsedObjectPreferenceIndex, CompressionMission.WieldSlotCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Request to use UsableMissionObject with ID: " + (object) this.UsableGameObject.Id + " and with name: " + this.UsableGameObject.GameEntity.Name;
  }
}
