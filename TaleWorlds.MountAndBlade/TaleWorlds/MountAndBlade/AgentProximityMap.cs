// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentProximityMap
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AgentProximityMap
  {
    public static bool CanSearchRadius(float searchRadius)
    {
      float num = Mission.Current.ProximityMapMaxSearchRadius();
      return (double) searchRadius <= (double) num;
    }

    public static AgentProximityMap.ProximityMapSearchStruct BeginSearch(
      Vec2 searchPos,
      float searchRadius)
    {
      AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = new AgentProximityMap.ProximityMapSearchStruct();
      proximityMapSearchStruct.SearchStructInternal = Mission.Current.ProximityMapBeginSearch(searchPos, searchRadius);
      proximityMapSearchStruct.RefreshLastFoundAgent();
      return proximityMapSearchStruct;
    }

    public static void FindNext(
      ref AgentProximityMap.ProximityMapSearchStruct searchStruct)
    {
      Mission.Current.ProximityMapFindNext(ref searchStruct.SearchStructInternal);
      searchStruct.RefreshLastFoundAgent();
    }

    public struct ProximityMapSearchStruct
    {
      internal AgentProximityMap.ProximityMapSearchStructInternal SearchStructInternal;

      public Agent LastFoundAgent { get; private set; }

      internal void RefreshLastFoundAgent() => this.LastFoundAgent = this.SearchStructInternal.GetCurrentAgent();
    }

    [EngineStruct("Managed_proximity_map_search_struct")]
    [Serializable]
    internal struct ProximityMapSearchStructInternal
    {
      internal int CurrentElementIndex;
      internal Vec2i Loc;
      internal Vec2i GridMin;
      internal Vec2i GridMax;
      internal Vec2 SearchPos;
      internal float SearchDistSq;

      internal Agent GetCurrentAgent() => Mission.Current.FindAgentWithIndex(this.CurrentElementIndex);
    }
  }
}
