// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentSpawnData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Agent_spawn_data")]
  public struct AgentSpawnData
  {
    public int HitPoints;
    public int MonsterUsageIndex;
    public int Weight;
    public float StandingEyeHeight;
    public float CrouchEyeHeight;
    public float MountedEyeHeight;
    public float RiderEyeHeightAdder;
    public float JumpAcceleration;
    public Vec3 EyeOffsetWrtHead;
    public Vec3 FirstPersonCameraOffsetWrtHead;
    public float RiderCameraHeightAdder;
    public float RiderBodyCapsuleHeightAdder;
    public float RiderBodyCapsuleForwardAdder;
    public float ArmLength;
    public float ArmWeight;
    public float JumpSpeedLimit;
    public float RelativeSpeedLimitForCharge;
  }
}
