// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ItemFlags
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  [Flags]
  public enum ItemFlags : uint
  {
    ForceAttachOffHandPrimaryItemBone = 256, // 0x00000100
    ForceAttachOffHandSecondaryItemBone = 512, // 0x00000200
    AttachmentMask = ForceAttachOffHandSecondaryItemBone | ForceAttachOffHandPrimaryItemBone, // 0x00000300
    NotUsableByFemale = 1024, // 0x00000400
    NotUsableByMale = 2048, // 0x00000800
    DropOnWeaponChange = 4096, // 0x00001000
    DropOnAnyAction = 8192, // 0x00002000
    CannotBePickedUp = 16384, // 0x00004000
    CanBePickedUpFromCorpse = 32768, // 0x00008000
    QuickFadeOut = 65536, // 0x00010000
    WoodenAttack = 131072, // 0x00020000
    WoodenParry = 262144, // 0x00040000
    HeldInOffHand = 524288, // 0x00080000
    HasToBeHeldUp = 1048576, // 0x00100000
    UseTeamColor = 2097152, // 0x00200000
    Civilian = 4194304, // 0x00400000
    DoNotScaleBodyAccordingToWeaponLength = 8388608, // 0x00800000
    DoesNotHideChest = 16777216, // 0x01000000
  }
}
