// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CreateFreeMountAgent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreateFreeMountAgent : GameNetworkMessage
  {
    public int AgentIndex { get; private set; }

    public EquipmentElement HorseItem { get; private set; }

    public EquipmentElement HorseHarnessItem { get; private set; }

    public Vec3 Position { get; private set; }

    public Vec2 Direction { get; private set; }

    public CreateFreeMountAgent(Agent agent, Vec3 position, Vec2 direction)
    {
      this.AgentIndex = agent.Index;
      this.HorseItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.ArmorItemEndSlot);
      this.HorseHarnessItem = agent.SpawnEquipment.GetEquipmentFromSlot(EquipmentIndex.HorseHarness);
      this.Position = position;
      this.Direction = direction.Normalized();
    }

    public CreateFreeMountAgent()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
      this.HorseItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref bufferReadValid);
      this.HorseHarnessItem = ModuleNetworkData.ReadItemReferenceFromPacket(Game.Current.ObjectManager, ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.AgentIndex, CompressionMission.AgentCompressionInfo);
      ModuleNetworkData.WriteItemReferenceToPacket(this.HorseItem);
      ModuleNetworkData.WriteItemReferenceToPacket(this.HorseHarnessItem);
      GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
      GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat() => "Create a mount-agent with index: " + (object) this.AgentIndex;
  }
}
