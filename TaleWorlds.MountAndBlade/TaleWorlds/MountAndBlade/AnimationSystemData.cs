// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AnimationSystemData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Animation_system_data")]
  [Serializable]
  public struct AnimationSystemData
  {
    public MBActionSet ActionSet;
    public int NumPaces;
    public int MonsterUsageSetIndex;
    public float WalkingSpeedLimit;
    public float CrouchWalkingSpeedLimit;
    public float StepSize;
    public bool HasClippingPlane;
  }
}
