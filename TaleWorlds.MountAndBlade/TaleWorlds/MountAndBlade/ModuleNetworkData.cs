// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ModuleNetworkData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public static class ModuleNetworkData
  {
    public static EquipmentElement ReadItemReferenceFromPacket(
      MBObjectManager objectManager,
      ref bool bufferReadValid)
    {
      return new EquipmentElement(GameNetworkMessage.ReadObjectReferenceFromPacket(objectManager, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid) as ItemObject);
    }

    public static void WriteItemReferenceToPacket(EquipmentElement equipElement) => GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) equipElement.Item, CompressionBasic.GUIDCompressionInfo);

    public static MissionWeapon ReadWeaponReferenceFromPacket(
      MBObjectManager objectManager,
      ref bool bufferReadValid)
    {
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        return MissionWeapon.Invalid;
      MBObjectBase mbObjectBase1 = GameNetworkMessage.ReadObjectReferenceFromPacket(objectManager, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
      int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.ItemDataValueCompressionInfo, ref bufferReadValid);
      int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponReloadPhaseCompressionInfo, ref bufferReadValid);
      short num3 = (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref bufferReadValid);
      int num4 = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid) ? 1 : 0;
      Banner banner = (Banner) null;
      if (num4 != 0)
      {
        string bannerKey = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
        if (bufferReadValid)
          banner = new Banner(bannerKey);
      }
      ItemObject primaryItem1 = mbObjectBase1 as ItemObject;
      bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      MissionWeapon missionWeapon;
      if (bufferReadValid & flag)
      {
        MBObjectBase mbObjectBase2 = GameNetworkMessage.ReadObjectReferenceFromPacket(objectManager, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
        int num5 = GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.ItemDataValueCompressionInfo, ref bufferReadValid);
        ItemObject primaryItem2 = mbObjectBase2 as ItemObject;
        missionWeapon = new MissionWeapon(primaryItem1, (ItemModifier) null, banner, (short) num1, (short) num2, new MissionWeapon?(new MissionWeapon(primaryItem2, (ItemModifier) null, banner, (short) num5)));
      }
      else
        missionWeapon = new MissionWeapon(primaryItem1, (ItemModifier) null, banner, (short) num1, (short) num2, new MissionWeapon?());
      missionWeapon.CurrentUsageIndex = (int) num3;
      return missionWeapon;
    }

    public static void WriteWeaponReferenceToPacket(MissionWeapon weapon)
    {
      GameNetworkMessage.WriteBoolToPacket(weapon.IsEmpty);
      if (weapon.IsEmpty)
        return;
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) weapon.Item, CompressionBasic.GUIDCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) weapon.RawDataForNetwork, CompressionGeneric.ItemDataValueCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) weapon.ReloadPhase, CompressionMission.WeaponReloadPhaseCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(weapon.CurrentUsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
      int num1 = weapon.Banner != null ? 1 : 0;
      GameNetworkMessage.WriteBoolToPacket(num1 != 0);
      if (num1 != 0)
        GameNetworkMessage.WriteStringToPacket(weapon.Banner.Serialize());
      MissionWeapon ammoWeapon = weapon.AmmoWeapon;
      int num2 = !ammoWeapon.IsEmpty ? 1 : 0;
      GameNetworkMessage.WriteBoolToPacket(num2 != 0);
      if (num2 == 0)
        return;
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) ammoWeapon.Item, CompressionBasic.GUIDCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) ammoWeapon.RawDataForNetwork, CompressionGeneric.ItemDataValueCompressionInfo);
    }

    public static MissionWeapon ReadMissileWeaponReferenceFromPacket(
      MBObjectManager objectManager,
      ref bool bufferReadValid)
    {
      MBObjectBase mbObjectBase = GameNetworkMessage.ReadObjectReferenceFromPacket(objectManager, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
      short num = (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref bufferReadValid);
      return new MissionWeapon(mbObjectBase as ItemObject, (ItemModifier) null, (Banner) null, (short) 1)
      {
        CurrentUsageIndex = (int) num
      };
    }

    public static void WriteMissileWeaponReferenceToPacket(MissionWeapon weapon)
    {
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) weapon.Item, CompressionBasic.GUIDCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(weapon.CurrentUsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
    }
  }
}
