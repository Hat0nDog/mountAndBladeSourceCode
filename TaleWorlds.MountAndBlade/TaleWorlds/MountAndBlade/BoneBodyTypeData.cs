// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BoneBodyTypeData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Bone_body_type_data")]
  public struct BoneBodyTypeData
  {
    private const byte SweepFlagBit = 128;
    public BoneBodyPartType BodyPartType;
    private readonly byte DataFlags;

    public sbyte GetPriority() => (sbyte) ((int) this.DataFlags & -129);

    public bool SweepCollisionCheckActive() => ((uint) this.DataFlags & 128U) > 0U;
  }
}
