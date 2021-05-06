// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBAnimation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public struct MBAnimation
  {
    internal readonly int Index;

    public MBAnimation(MBAnimation animation) => this.Index = animation.Index;

    internal MBAnimation(int i) => this.Index = i;

    private bool IsValid => this.Index >= 0;

    public bool Equals(MBAnimation a) => this.Index == a.Index;

    public override int GetHashCode() => this.Index;

    public static Vec3 GetDisplacementVector(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.GetDisplacementVector(actionSet.Index, actionIndexCache.Index);
    }

    public static AnimFlags GetAnimationFlags(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.GetAnimationFlags(actionSet.Index, actionIndexCache.Index);
    }

    public static int GetAnimationIndexWithName(string animationName) => string.IsNullOrEmpty(animationName) ? -1 : MBAPI.IMBAnimation.GetIndexWithID(animationName);

    public static Agent.ActionCodeType GetActionType(ActionIndexCache actionIndex) => !(actionIndex == ActionIndexCache.act_none) ? MBAPI.IMBAnimation.GetActionType(actionIndex.Index) : Agent.ActionCodeType.Other;

    public static bool CheckAnimationClipExists(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.CheckAnimationClipExists(actionSet.Index, actionIndexCache.Index);
    }

    public static int GetAnimationIndexOfActionCode(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.AnimationIndexOfActionCode(actionSet.Index, actionIndexCache.Index);
    }

    public static float GetAnimationDuration(string animationName)
    {
      int indexWithId = MBAPI.IMBAnimation.GetIndexWithID(animationName);
      return MBAPI.IMBAnimation.GetAnimationDuration(indexWithId);
    }

    public static float GetAnimationDuration(int animationIndex) => MBAPI.IMBAnimation.GetAnimationDuration(animationIndex);

    public static float GetAnimationParameter1(string animationName)
    {
      int indexWithId = MBAPI.IMBAnimation.GetIndexWithID(animationName);
      return MBAPI.IMBAnimation.GetAnimationParameter1(indexWithId);
    }

    public static float GetAnimationParameter1(int animationIndex) => MBAPI.IMBAnimation.GetAnimationParameter1(animationIndex);

    public static float GetAnimationParameter2(string animationName)
    {
      int indexWithId = MBAPI.IMBAnimation.GetIndexWithID(animationName);
      return MBAPI.IMBAnimation.GetAnimationParameter2(indexWithId);
    }

    public static float GetAnimationParameter3(string animationName)
    {
      int indexWithId = MBAPI.IMBAnimation.GetIndexWithID(animationName);
      return MBAPI.IMBAnimation.GetAnimationParameter3(indexWithId);
    }

    public static string GetAnimationName(MBActionSet actionSet, ActionIndexCache actionIndexCache) => MBAPI.IMBAnimation.GetAnimationName(actionSet.Index, actionIndexCache.Index);

    public static float GetActionAnimationDuration(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.GetActionAnimationDuration(actionSet.Index, actionIndexCache.Index);
    }

    public static ActionIndexCache GetAnimationContinueToAction(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return new ActionIndexCache(MBAPI.IMBAnimation.GetAnimationContinueToAction(actionSet.Index, actionIndexCache.Index));
    }

    public static float GetTotalAnimationDurationWithContinueToAction(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      float num = 0.0f;
      for (; actionIndexCache != ActionIndexCache.act_none; actionIndexCache = MBAnimation.GetAnimationContinueToAction(actionSet, actionIndexCache))
        num += MBAnimation.GetActionAnimationDuration(actionSet, actionIndexCache);
      return num;
    }

    public static float GetAnimationBlendInPeriod(string animationName)
    {
      int indexWithId = MBAPI.IMBAnimation.GetIndexWithID(animationName);
      return MBAPI.IMBAnimation.GetAnimationBlendInPeriod(indexWithId);
    }

    public static float GetAnimationBlendInPeriod(int animationIndex) => MBAPI.IMBAnimation.GetAnimationBlendInPeriod(animationIndex);

    public static float GetActionBlendOutStartProgress(
      MBActionSet actionSet,
      ActionIndexCache actionIndexCache)
    {
      return MBAPI.IMBAnimation.GetActionBlendOutStartProgress(actionSet.Index, actionIndexCache.Index);
    }

    public static int GetActionCodeWithName(string name) => string.IsNullOrEmpty(name) ? ActionIndexCache.act_none.Index : MBAPI.IMBAnimation.GetActionCodeWithName(name);

    public static string GetActionNameWithCode(int actionIndex) => actionIndex == -1 ? "act_none" : MBAPI.IMBAnimation.GetActionNameWithCode(actionIndex);

    public static int GetNumActionCodes() => MBAPI.IMBAnimation.GetNumActionCodes();

    public static int GetNumAnimations() => MBAPI.IMBAnimation.GetNumAnimations();
  }
}
