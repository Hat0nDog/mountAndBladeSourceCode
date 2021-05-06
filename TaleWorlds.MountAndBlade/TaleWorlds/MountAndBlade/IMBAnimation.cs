// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBAnimation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBAnimation
  {
    [EngineMethod("get_id_with_index", false)]
    string GetIDWithIndex(int index);

    [EngineMethod("get_index_with_id", false)]
    int GetIndexWithID(string id);

    [EngineMethod("get_displacement_vector", false)]
    Vec3 GetDisplacementVector(int actionSetNo, int actionIndex);

    [EngineMethod("check_animation_clip_exists", false)]
    bool CheckAnimationClipExists(int actionSetNo, int actionIndex);

    [EngineMethod("get_animation_index_of_action_code", false)]
    int AnimationIndexOfActionCode(int actionSetNo, int actionIndex);

    [EngineMethod("get_animation_flags", false)]
    AnimFlags GetAnimationFlags(int actionSetNo, int actionIndex);

    [EngineMethod("get_action_type", false)]
    Agent.ActionCodeType GetActionType(int actionIndex);

    [EngineMethod("get_animation_duration", false)]
    float GetAnimationDuration(int animationIndex);

    [EngineMethod("get_animation_parameter1", false)]
    float GetAnimationParameter1(int animationIndex);

    [EngineMethod("get_animation_parameter2", false)]
    float GetAnimationParameter2(int animationIndex);

    [EngineMethod("get_animation_parameter3", false)]
    float GetAnimationParameter3(int animationIndex);

    [EngineMethod("get_action_animation_duration", false)]
    float GetActionAnimationDuration(int actionSetNo, int actionIndex);

    [EngineMethod("get_animation_name", false)]
    string GetAnimationName(int actionSetNo, int actionIndex);

    [EngineMethod("get_animation_continue_to_action", false)]
    int GetAnimationContinueToAction(int actionSetNo, int actionIndex);

    [EngineMethod("get_animation_blend_in_period", false)]
    float GetAnimationBlendInPeriod(int animationIndex);

    [EngineMethod("get_action_blend_out_start_progress", false)]
    float GetActionBlendOutStartProgress(int actionSetNo, int actionIndex);

    [EngineMethod("get_action_code_with_name", false)]
    int GetActionCodeWithName(string name);

    [EngineMethod("get_action_name_with_code", false)]
    string GetActionNameWithCode(int index);

    [EngineMethod("get_num_action_codes", false)]
    int GetNumActionCodes();

    [EngineMethod("get_num_animations", false)]
    int GetNumAnimations();
  }
}
