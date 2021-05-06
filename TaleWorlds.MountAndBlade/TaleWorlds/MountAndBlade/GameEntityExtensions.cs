// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameEntityExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class GameEntityExtensions
  {
    public static GameEntity Instantiate(
      Scene scene,
      MissionWeapon weapon,
      bool showHolsterWithWeapon,
      bool needBatchedVersion)
    {
      WeaponData weaponData = weapon.GetWeaponData(needBatchedVersion);
      WeaponStatsData[] weaponStatsData = weapon.GetWeaponStatsData();
      WeaponData ammoWeaponData = weapon.GetAmmoWeaponData(needBatchedVersion);
      WeaponStatsData[] ammoWeaponStatsData = weapon.GetAmmoWeaponStatsData();
      GameEntity fromWeapon = MBAPI.IMBGameEntityExtensions.CreateFromWeapon(scene.Pointer, in weaponData, weaponStatsData, weaponStatsData.Length, in ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData.Length, showHolsterWithWeapon);
      weaponData.DeinitializeManagedPointers();
      return fromWeapon;
    }

    public static void CreateSimpleSkeleton(this GameEntity gameEntity, string skeletonName) => gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateSimpleSkeleton(skeletonName);

    public static void CreateAgentSkeleton(
      this GameEntity gameEntity,
      string skeletonName,
      bool isHumanoid,
      MBActionSet actionSet,
      string monsterUsageSetName,
      Monster monster)
    {
      AgentVisualsNativeData agentVisualsNativeData = new AgentVisualsNativeData()
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
      gameEntity.Skeleton = MBAPI.IMBSkeletonExtensions.CreateAgentSkeleton(skeletonName, isHumanoid, actionSet.Index, monsterUsageSetName, ref agentVisualsNativeData);
    }

    public static void CreateSkeletonWithActionSet(
      this GameEntity gameEntity,
      ref AgentVisualsNativeData agentVisualsNativeData,
      ref AnimationSystemData animationSystemData)
    {
      gameEntity.Skeleton = MBSkeletonExtensions.CreateWithActionSet(ref agentVisualsNativeData, ref animationSystemData);
    }

    public static void FadeOut(
      this GameEntity gameEntity,
      float interval,
      bool isRemovingFromScene)
    {
      MBAPI.IMBGameEntityExtensions.FadeOut(gameEntity.Pointer, interval, isRemovingFromScene);
    }

    public static void FadeIn(this GameEntity gameEntity, bool resetAlpha = true) => MBAPI.IMBGameEntityExtensions.FadeIn(gameEntity.Pointer, resetAlpha);

    public static void HideIfNotFadingOut(this GameEntity gameEntity) => MBAPI.IMBGameEntityExtensions.HideIfNotFadingOut(gameEntity.Pointer);
  }
}
