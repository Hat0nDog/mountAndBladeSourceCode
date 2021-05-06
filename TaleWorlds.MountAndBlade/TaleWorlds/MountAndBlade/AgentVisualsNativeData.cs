// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentVisualsNativeData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Agent_visuals_native_data")]
  public struct AgentVisualsNativeData
  {
    public sbyte MainHandItemBoneIndex;
    public sbyte OffHandItemBoneIndex;
    public sbyte MainHandItemSecondaryBoneIndex;
    public sbyte OffHandItemSecondaryBoneIndex;
    public sbyte RiderSitBoneIndex;
    public sbyte ReinHandleBoneIndex;
    public sbyte ReinCollision1BoneIndex;
    public sbyte ReinCollision2BoneIndex;
    public sbyte HeadLookDirectionBoneIndex;
    public sbyte ThoraxLookDirectionBoneIndex;
    public sbyte MainHandNumBonesForIk;
    public sbyte OffHandNumBonesForIk;
    public Vec3 ReinHandleLeftLocalPosition;
    public Vec3 ReinHandleRightLocalPosition;
  }
}
