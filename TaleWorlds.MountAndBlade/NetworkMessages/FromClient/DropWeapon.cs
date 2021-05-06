// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.DropWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class DropWeapon : GameNetworkMessage
  {
    public bool IsDefendPressed { get; private set; }

    public EquipmentIndex ForcedSlotIndexToDropWeaponFrom { get; private set; }

    public DropWeapon(bool isDefendPressed, EquipmentIndex forcedSlotIndexToDropWeaponFrom)
    {
      this.IsDefendPressed = isDefendPressed;
      this.ForcedSlotIndexToDropWeaponFrom = forcedSlotIndexToDropWeaponFrom;
    }

    public DropWeapon()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.IsDefendPressed = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.ForcedSlotIndexToDropWeaponFrom = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteBoolToPacket(this.IsDefendPressed);
      GameNetworkMessage.WriteIntToPacket((int) this.ForcedSlotIndexToDropWeaponFrom, CompressionMission.WieldSlotCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat()
    {
      bool flag = this.ForcedSlotIndexToDropWeaponFrom != EquipmentIndex.None;
      return "Dropping " + (!flag ? "equipped" : "") + " weapon" + (flag ? " " + (object) (int) this.ForcedSlotIndexToDropWeaponFrom : "");
    }
  }
}
