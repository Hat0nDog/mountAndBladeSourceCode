// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.BodyFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum BodyFlags : uint
  {
    None = 0,
    Disabled = 1,
    NotDestructible = 2,
    TwoSided = 4,
    Dynamic = 8,
    Moveable = 16, // 0x00000010
    DynamicConvexHull = 32, // 0x00000020
    Ladder = 64, // 0x00000040
    OnlyCollideWithRaycast = 128, // 0x00000080
    AILimiter = 256, // 0x00000100
    Barrier = 512, // 0x00000200
    Barrier3D = 1024, // 0x00000400
    HasSteps = 2048, // 0x00000800
    Ragdoll = 4096, // 0x00001000
    RagdollLimiter = 8192, // 0x00002000
    DestructibleDoor = 16384, // 0x00004000
    DroppedItem = 32768, // 0x00008000
    DoNotCollideWithRaycast = 65536, // 0x00010000
    DontTransferToPhysicsEngine = 131072, // 0x00020000
    DontCollideWithCamera = 262144, // 0x00040000
    ExcludePathSnap = 524288, // 0x00080000
    IsOpoed = 1048576, // 0x00100000
    AfterAddFlags = IsOpoed, // 0x00100000
    AgentOnly = 2097152, // 0x00200000
    MissileOnly = 4194304, // 0x00400000
    HasMaterial = 8388608, // 0x00800000
    BodyFlagFilter = HasMaterial | MissileOnly | AgentOnly | AfterAddFlags | ExcludePathSnap | DontCollideWithCamera | DontTransferToPhysicsEngine | DoNotCollideWithRaycast | DroppedItem | DestructibleDoor | RagdollLimiter | Ragdoll | HasSteps | Barrier3D | Barrier | AILimiter | OnlyCollideWithRaycast | Ladder | DynamicConvexHull | Moveable | Dynamic | TwoSided | NotDestructible | Disabled, // 0x00FFFFFF
    CommonCollisionExcludeFlags = MissileOnly | AgentOnly | DoNotCollideWithRaycast | DroppedItem | RagdollLimiter | Ragdoll | AILimiter | OnlyCollideWithRaycast | Dynamic | Disabled, // 0x0061B189
    CameraCollisionRayCastExludeFlags = CommonCollisionExcludeFlags | Barrier3D | Barrier | Ladder, // 0x0061B7C9
    CommonCollisionExcludeFlagsForAgent = MissileOnly | DoNotCollideWithRaycast | DroppedItem | RagdollLimiter | Ragdoll | AILimiter | OnlyCollideWithRaycast | Dynamic | Disabled, // 0x0041B189
    CommonCollisionExcludeFlagsForMissile = AgentOnly | DoNotCollideWithRaycast | DroppedItem | RagdollLimiter | Ragdoll | Barrier3D | Barrier | AILimiter | OnlyCollideWithRaycast | Dynamic | Disabled, // 0x0021B789
    CommonCollisionExcludeFlagsForCombat = AgentOnly | DoNotCollideWithRaycast | DroppedItem | RagdollLimiter | Ragdoll | AILimiter | OnlyCollideWithRaycast | Dynamic | Disabled, // 0x0021B189
    CommonCollisionExcludeFlagsForEditor = CommonCollisionExcludeFlagsForCombat, // 0x0021B189
    CommonFlagsThatDoNotBlocksRay = HasMaterial | MissileOnly | AgentOnly | AfterAddFlags | ExcludePathSnap | DontCollideWithCamera | DontTransferToPhysicsEngine | DoNotCollideWithRaycast | RagdollLimiter | Ragdoll | HasSteps | Barrier3D | Barrier | AILimiter | DynamicConvexHull | Moveable | Dynamic | TwoSided | NotDestructible | Disabled, // 0x00FF3F3F
    CommonFocusRayCastExcludeFlags = DoNotCollideWithRaycast | RagdollLimiter | Ragdoll | Barrier3D | Barrier | AILimiter | Disabled, // 0x00013701
    BodyOwnerNone = 0,
    BodyOwnerEntity = 16777216, // 0x01000000
    BodyOwnerTerrain = 33554432, // 0x02000000
    BodyOwnerFlora = 67108864, // 0x04000000
    BodyOwnerFilter = 251658240, // 0x0F000000
  }
}
