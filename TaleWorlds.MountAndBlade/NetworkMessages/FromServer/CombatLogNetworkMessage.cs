// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CombatLogNetworkMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CombatLogNetworkMessage : GameNetworkMessage
  {
    private Agent AttackerAgent { get; set; }

    private Agent VictimAgent { get; set; }

    private bool IsVictimEntity { get; set; }

    public DamageTypes DamageType { get; private set; }

    public bool CrushedThrough { get; private set; }

    public bool Chamber { get; private set; }

    public bool IsRangedAttack { get; private set; }

    public bool IsFriendlyFire { get; private set; }

    public bool IsFatalDamage { get; private set; }

    public BoneBodyPartType BodyPartHit { get; private set; }

    public float HitSpeed { get; private set; }

    public float Distance { get; private set; }

    public int InflictedDamage { get; private set; }

    public int AbsorbedDamage { get; private set; }

    public int ExtraDamage { get; private set; }

    public CombatLogNetworkMessage()
    {
    }

    public CombatLogNetworkMessage(
      Agent attackerAgent,
      Agent victimAgent,
      GameEntity hitEntity,
      CombatLogData combatLogData)
    {
      this.AttackerAgent = attackerAgent;
      this.VictimAgent = victimAgent;
      this.IsVictimEntity = (NativeObject) hitEntity != (NativeObject) null;
      this.DamageType = combatLogData.DamageType;
      this.CrushedThrough = combatLogData.CrushedThrough;
      this.Chamber = combatLogData.Chamber;
      this.IsRangedAttack = combatLogData.IsRangedAttack;
      this.IsFriendlyFire = combatLogData.IsFriendlyFire;
      this.IsFatalDamage = combatLogData.IsFatalDamage;
      this.BodyPartHit = combatLogData.BodyPartHit;
      this.HitSpeed = combatLogData.HitSpeed;
      this.Distance = combatLogData.Distance;
      this.InflictedDamage = combatLogData.InflictedDamage;
      this.AbsorbedDamage = combatLogData.AbsorbedDamage;
      this.ExtraDamage = combatLogData.ModifiedDamage;
    }

    public CombatLogData GetData()
    {
      int num1 = this.AttackerAgent != null ? 1 : 0;
      bool flag1 = num1 != 0 && this.AttackerAgent.IsHuman;
      bool flag2 = num1 != 0 && this.AttackerAgent.IsMine;
      bool flag3 = num1 != 0 && this.AttackerAgent.RiderAgent != null;
      bool flag4 = flag3 && this.AttackerAgent.RiderAgent.IsMine;
      bool flag5 = num1 != 0 && this.AttackerAgent.IsMount;
      Agent victimAgent1 = this.VictimAgent;
      bool flag6 = victimAgent1 != null && (double) victimAgent1.Health <= 0.0;
      bool flag7 = this.AttackerAgent != null && this.VictimAgent?.RiderAgent == this.AttackerAgent;
      CombatLogData combatLogData;
      ref CombatLogData local = ref combatLogData;
      int num2 = this.AttackerAgent == this.VictimAgent ? 1 : 0;
      int num3 = flag1 ? 1 : 0;
      int num4 = flag2 ? 1 : 0;
      int num5 = flag3 ? 1 : 0;
      int num6 = flag4 ? 1 : 0;
      int num7 = flag5 ? 1 : 0;
      Agent victimAgent2 = this.VictimAgent;
      int num8 = victimAgent2 != null ? (victimAgent2.IsHuman ? 1 : 0) : 0;
      Agent victimAgent3 = this.VictimAgent;
      int num9 = victimAgent3 != null ? (victimAgent3.IsMine ? 1 : 0) : 0;
      int num10 = flag6 ? 1 : 0;
      int num11 = this.VictimAgent?.RiderAgent != null ? 1 : 0;
      int num12 = (int) this.VictimAgent?.RiderAgent?.IsMine ?? 0;
      Agent victimAgent4 = this.VictimAgent;
      int num13 = victimAgent4 != null ? (victimAgent4.IsMount ? 1 : 0) : 0;
      int num14 = this.IsVictimEntity ? 1 : 0;
      int num15 = flag7 ? 1 : 0;
      int num16 = this.CrushedThrough ? 1 : 0;
      int num17 = this.Chamber ? 1 : 0;
      double distance = (double) this.Distance;
      local = new CombatLogData(num2 != 0, num3 != 0, num4 != 0, num5 != 0, num6 != 0, num7 != 0, num8 != 0, num9 != 0, num10 != 0, num11 != 0, num12 != 0, num13 != 0, num14 != 0, num15 != 0, num16 != 0, num17 != 0, (float) distance);
      combatLogData.DamageType = this.DamageType;
      combatLogData.IsRangedAttack = this.IsRangedAttack;
      combatLogData.IsFriendlyFire = this.IsFriendlyFire;
      combatLogData.IsFatalDamage = this.IsFatalDamage;
      combatLogData.BodyPartHit = this.BodyPartHit;
      combatLogData.HitSpeed = this.HitSpeed;
      combatLogData.InflictedDamage = this.InflictedDamage;
      combatLogData.AbsorbedDamage = this.AbsorbedDamage;
      combatLogData.ModifiedDamage = this.ExtraDamage;
      combatLogData.VictimAgentName = this.VictimAgent?.Name ?? "";
      return combatLogData;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.AttackerAgent);
      GameNetworkMessage.WriteAgentReferenceToPacket(this.VictimAgent);
      GameNetworkMessage.WriteBoolToPacket(this.IsVictimEntity);
      GameNetworkMessage.WriteIntToPacket((int) this.DamageType, CompressionBasic.AgentHitDamageTypeCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.CrushedThrough);
      GameNetworkMessage.WriteBoolToPacket(this.Chamber);
      GameNetworkMessage.WriteBoolToPacket(this.IsRangedAttack);
      GameNetworkMessage.WriteBoolToPacket(this.IsFriendlyFire);
      GameNetworkMessage.WriteBoolToPacket(this.IsFatalDamage);
      GameNetworkMessage.WriteIntToPacket((int) this.BodyPartHit, CompressionBasic.AgentHitBodyPartCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.HitSpeed, CompressionBasic.AgentHitRelativeSpeedCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.AgentHitRelativeSpeedCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AbsorbedDamage, CompressionBasic.AgentHitDamageCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.InflictedDamage, CompressionBasic.AgentHitDamageCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.ExtraDamage, CompressionBasic.AgentHitDamageCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AttackerAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.VictimAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.IsVictimEntity = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.DamageType = (DamageTypes) GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageTypeCompressionInfo, ref bufferReadValid);
      this.CrushedThrough = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.Chamber = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsRangedAttack = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsFriendlyFire = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsFatalDamage = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.BodyPartHit = (BoneBodyPartType) GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitBodyPartCompressionInfo, ref bufferReadValid);
      this.HitSpeed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentHitRelativeSpeedCompressionInfo, ref bufferReadValid);
      this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentHitRelativeSpeedCompressionInfo, ref bufferReadValid);
      this.AbsorbedDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageCompressionInfo, ref bufferReadValid);
      this.InflictedDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageCompressionInfo, ref bufferReadValid);
      this.ExtraDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Agent got hit.";
  }
}
