// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleTroopSupplier
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleTroopSupplier : IMissionTroopSupplier
  {
    private readonly CustomBattleCombatant _customBattleCombatant;
    private PriorityQueue<float, BasicCharacterObject> _characters;
    private int _numAllocated;
    private int _numWounded;
    private int _numKilled;
    private int _numRouted;
    private bool _anyTroopRemainsToBeSupplied = true;
    private readonly bool _isPlayerSide;

    public CustomBattleTroopSupplier(CustomBattleCombatant customBattleCombatant, bool isPlayerSide)
    {
      this._customBattleCombatant = customBattleCombatant;
      this._isPlayerSide = isPlayerSide;
      this.ArrangePriorities();
    }

    private void ArrangePriorities()
    {
      this._characters = new PriorityQueue<float, BasicCharacterObject>((IComparer<float>) new GenericComparer<float>());
      int[] numArray = new int[8];
      for (int i = 0; i < 8; i++)
        numArray[i] = this._customBattleCombatant.Characters.Count<BasicCharacterObject>((Func<BasicCharacterObject, bool>) (character => character.DefaultFormationClass == (FormationClass) i));
      int num = 1000;
      foreach (BasicCharacterObject character in this._customBattleCombatant.Characters)
      {
        FormationClass formationClass = character.GetFormationClass((IBattleCombatant) this._customBattleCombatant);
        this._characters.Enqueue(character.IsHero ? (float) num-- : (float) (numArray[(int) formationClass] / ((IEnumerable<int>) numArray).Sum()), character);
        --numArray[(int) formationClass];
      }
    }

    public IEnumerable<IAgentOriginBase> SupplyTroops(
      int numberToAllocate)
    {
      List<BasicCharacterObject> basicCharacterObjectList = this.AllocateTroops(numberToAllocate);
      CustomBattleAgentOrigin[] battleAgentOriginArray = new CustomBattleAgentOrigin[basicCharacterObjectList.Count];
      this._numAllocated += basicCharacterObjectList.Count;
      for (int index = 0; index < battleAgentOriginArray.Length; ++index)
      {
        UniqueTroopDescriptor uniqueNo = new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed);
        battleAgentOriginArray[index] = new CustomBattleAgentOrigin(this._customBattleCombatant, basicCharacterObjectList[index], this, this._isPlayerSide, index, uniqueNo);
      }
      if (battleAgentOriginArray.Length < numberToAllocate)
        this._anyTroopRemainsToBeSupplied = false;
      return (IEnumerable<IAgentOriginBase>) battleAgentOriginArray;
    }

    private List<BasicCharacterObject> AllocateTroops(int numberToAllocate)
    {
      if (numberToAllocate > this._characters.Count)
        numberToAllocate = this._characters.Count;
      List<BasicCharacterObject> basicCharacterObjectList = new List<BasicCharacterObject>();
      for (int index = 0; index < numberToAllocate; ++index)
        basicCharacterObjectList.Add(this._characters.DequeueValue());
      return basicCharacterObjectList;
    }

    public void OnTroopWounded() => ++this._numWounded;

    public void OnTroopKilled() => ++this._numKilled;

    public void OnTroopRouted() => ++this._numRouted;

    public int NumActiveTroops => this._numAllocated - (this._numWounded + this._numKilled + this._numRouted);

    public int NumRemovedTroops => this._numWounded + this._numKilled + this._numRouted;

    public int NumTroopsNotSupplied => this._characters.Count - this._numAllocated;

    public bool AnyTroopRemainsToBeSupplied => this._anyTroopRemainsToBeSupplied;
  }
}
