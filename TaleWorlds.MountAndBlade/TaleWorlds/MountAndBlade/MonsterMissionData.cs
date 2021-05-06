// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MonsterMissionData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class MonsterMissionData : IMonsterMissionData
  {
    private MBActionSet _actionSet;

    public Monster Monster { get; private set; }

    public CapsuleData BodyCapsule => new CapsuleData()
    {
      Radius = this.Monster.BodyCapsuleRadius,
      P1 = this.Monster.BodyCapsulePoint1,
      P2 = this.Monster.BodyCapsulePoint2
    };

    public CapsuleData CrouchedBodyCapsule => new CapsuleData()
    {
      Radius = this.Monster.CrouchedBodyCapsuleRadius,
      P1 = this.Monster.CrouchedBodyCapsulePoint1,
      P2 = this.Monster.CrouchedBodyCapsulePoint2
    };

    public MBActionSet ActionSet
    {
      get
      {
        if (!this._actionSet.IsValid && !string.IsNullOrEmpty(this.Monster.ActionSetCode))
          this._actionSet = MBActionSet.GetActionSet(this.Monster.ActionSetCode);
        return this._actionSet;
      }
    }

    public MonsterMissionData(Monster monster)
    {
      this._actionSet = MBActionSet.InvalidActionSet;
      this.Monster = monster;
    }
  }
}
