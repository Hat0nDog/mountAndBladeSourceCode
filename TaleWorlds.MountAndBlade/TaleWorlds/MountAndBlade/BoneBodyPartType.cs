// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BoneBodyPartType
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public enum BoneBodyPartType : sbyte
  {
    None = -1, // 0xFF
    CriticalBodyPartsBegin = 0,
    Head = 0,
    Neck = 1,
    Chest = 2,
    Abdomen = 3,
    ShoulderLeft = 4,
    ShoulderRight = 5,
    BipedalArmLeft = 6,
    CriticalBodyPartsEnd = 6,
    BipedalArmRight = 7,
    BipedalLegs = 8,
    QuadrupedalArmLeft = 9,
    QuadrupedalArmRight = 10, // 0x0A
    QuadrupedalLegs = 11, // 0x0B
    NumOfBodyPartTypes = 12, // 0x0C
  }
}
