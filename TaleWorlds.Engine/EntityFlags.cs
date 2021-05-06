// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.EntityFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum EntityFlags : uint
  {
    ForceLodMask = 240, // 0x000000F0
    ForceLodBits = 4,
    AnimateWhenVisible = 256, // 0x00000100
    NoOcclusionCulling = 512, // 0x00000200
    IsHelper = 1024, // 0x00000400
    ComputePerComponentLod = 2048, // 0x00000800
    DoesNotAffectParentsLocalBb = 4096, // 0x00001000
    ForceAsStatic = 8192, // 0x00002000
    SendInitCallback = 16384, // 0x00004000
    PhysicsDisabled = 32768, // 0x00008000
    AlignToTerrain = 65536, // 0x00010000
    DontSaveToScene = 131072, // 0x00020000
    RecordToSceneReplay = 262144, // 0x00040000
    GroupMeshesAfterLod4 = 524288, // 0x00080000
    SmoothLodTransitions = 1048576, // 0x00100000
    DontCheckHandness = 2097152, // 0x00200000
    NotAffectedBySeason = 4194304, // 0x00400000
    DontTickChildren = 8388608, // 0x00800000
    WaitUntilReady = 16777216, // 0x01000000
    NonModifiableFromEditor = 33554432, // 0x02000000
    DeferredParallelFrameSetup = 67108864, // 0x04000000
    PerComponentVisibility = 134217728, // 0x08000000
    Ignore = 268435456, // 0x10000000
    DoNotTick = 536870912, // 0x20000000
    DoNotRenderToEnvmap = 1073741824, // 0x40000000
    AlignRotationToTerrain = 2147483648, // 0x80000000
  }
}
