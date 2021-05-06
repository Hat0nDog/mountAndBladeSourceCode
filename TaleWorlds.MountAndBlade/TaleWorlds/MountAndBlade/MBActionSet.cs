// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBActionSet
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct MBActionSet
  {
    public static readonly MBActionSet InvalidActionSet = new MBActionSet(-1);

    internal int Index { get; private set; }

    internal MBActionSet(int i) => this.Index = i;

    public bool IsValid => this.Index >= 0;

    public bool Equals(MBActionSet a) => this.Index == a.Index;

    public bool Equals(int index) => this.Index == index;

    public override int GetHashCode() => this.Index;

    public string GetName() => this.IsValid ? MBAPI.IMBActionSet.GetNameWithIndex(this.Index) : "Invalid";

    public bool AreActionsAlternatives(ActionIndexCache actionCode1, ActionIndexCache actionCode2) => MBAPI.IMBActionSet.AreActionsAlternatives(this.Index, actionCode1.Index, actionCode2.Index);

    public static int GetNumberOfActionSets() => MBAPI.IMBActionSet.GetNumberOfActionSets();

    public static int GetNumberOfMonsterUsageSets() => MBAPI.IMBActionSet.GetNumberOfMonsterUsageSets();

    public static MBActionSet GetActionSet(string objectID) => MBActionSet.GetActionSetWithIndex(MBAPI.IMBActionSet.GetIndexWithID(objectID));

    public static MBActionSet GetActionSetWithIndex(int index) => new MBActionSet(index);

    public static sbyte GetBoneIndexWithId(string actionSetId, string boneId) => MBAPI.IMBActionSet.GetBoneIndexWithId(actionSetId, boneId);
  }
}
