// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MonsterExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public static class MonsterExtensions
  {
    public static AnimationSystemData FillAnimationSystemData(
      this Monster monster,
      float stepSize,
      bool hasClippingPlane)
    {
      MonsterMissionData monsterMissionData = (MonsterMissionData) monster.MonsterMissionData;
      return new AnimationSystemData()
      {
        ActionSet = monsterMissionData.ActionSet,
        NumPaces = monster.NumPaces,
        MonsterUsageSetIndex = Agent.GetMonsterUsageIndex(monster.MonsterUsage),
        WalkingSpeedLimit = monster.WalkingSpeedLimit,
        CrouchWalkingSpeedLimit = monster.CrouchWalkingSpeedLimit,
        StepSize = stepSize,
        HasClippingPlane = hasClippingPlane
      };
    }

    public static AnimationSystemData FillAnimationSystemData(
      this Monster monster,
      MBActionSet actionSet,
      float stepSize,
      bool hasClippingPlane)
    {
      return new AnimationSystemData()
      {
        ActionSet = actionSet,
        NumPaces = monster.NumPaces,
        MonsterUsageSetIndex = Agent.GetMonsterUsageIndex(monster.MonsterUsage),
        WalkingSpeedLimit = monster.WalkingSpeedLimit,
        CrouchWalkingSpeedLimit = monster.CrouchWalkingSpeedLimit,
        StepSize = stepSize,
        HasClippingPlane = hasClippingPlane
      };
    }

    public static AgentVisualsNativeData FillAgentVisualsNativeData(
      this Monster monster)
    {
      return new AgentVisualsNativeData()
      {
        MainHandItemBoneIndex = monster.MainHandItemBoneIndex,
        OffHandItemBoneIndex = monster.OffHandItemBoneIndex,
        MainHandItemSecondaryBoneIndex = monster.MainHandItemSecondaryBoneIndex,
        OffHandItemSecondaryBoneIndex = monster.OffHandItemSecondaryBoneIndex,
        RiderSitBoneIndex = monster.RiderSitBoneIndex,
        ReinHandleBoneIndex = monster.ReinHandleBoneIndex,
        ReinCollision1BoneIndex = monster.ReinCollision1BoneIndex,
        ReinCollision2BoneIndex = monster.ReinCollision2BoneIndex,
        HeadLookDirectionBoneIndex = monster.HeadLookDirectionBoneIndex,
        ThoraxLookDirectionBoneIndex = monster.ThoraxLookDirectionBoneIndex,
        MainHandNumBonesForIk = monster.MainHandNumBonesForIk,
        OffHandNumBonesForIk = monster.OffHandNumBonesForIk,
        ReinHandleLeftLocalPosition = monster.ReinHandleLeftLocalPosition,
        ReinHandleRightLocalPosition = monster.ReinHandleRightLocalPosition
      };
    }

    public static AgentCapsuleData FillCapsuleData(this Monster monster)
    {
      MonsterMissionData monsterMissionData = (MonsterMissionData) monster.MonsterMissionData;
      return new AgentCapsuleData()
      {
        BodyCap = monsterMissionData.BodyCapsule,
        CrouchedBodyCap = monsterMissionData.CrouchedBodyCapsule
      };
    }

    public static AgentSpawnData FillSpawnData(this Monster monster) => new AgentSpawnData()
    {
      HitPoints = monster.HitPoints,
      MonsterUsageIndex = Agent.GetMonsterUsageIndex(monster.MonsterUsage),
      Weight = monster.Weight,
      StandingEyeHeight = monster.StandingEyeHeight,
      CrouchEyeHeight = monster.CrouchEyeHeight,
      MountedEyeHeight = monster.MountedEyeHeight,
      RiderEyeHeightAdder = monster.RiderEyeHeightAdder,
      JumpAcceleration = monster.JumpAcceleration,
      EyeOffsetWrtHead = monster.EyeOffsetWrtHead,
      FirstPersonCameraOffsetWrtHead = monster.FirstPersonCameraOffsetWrtHead,
      RiderCameraHeightAdder = monster.RiderCameraHeightAdder,
      RiderBodyCapsuleHeightAdder = monster.RiderBodyCapsuleHeightAdder,
      RiderBodyCapsuleForwardAdder = monster.RiderBodyCapsuleForwardAdder,
      ArmLength = monster.ArmLength,
      ArmWeight = monster.ArmWeight,
      JumpSpeedLimit = monster.JumpSpeedLimit,
      RelativeSpeedLimitForCharge = monster.RelativeSpeedLimitForCharge
    };
  }
}
