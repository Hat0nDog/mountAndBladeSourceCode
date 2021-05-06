// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MBExtensions
  {
    private static Vec2 GetGlobalOrganicDirectionAux(
      ColumnFormation columnFormation,
      int depthCount = -1)
    {
      IEnumerable<Agent> unitsAtVanguardFile = columnFormation.GetUnitsAtVanguardFile<Agent>();
      Vec2 zero = Vec2.Zero;
      int num = 0;
      Agent agent1 = (Agent) null;
      foreach (Agent agent2 in unitsAtVanguardFile)
      {
        if (agent1 != null)
        {
          Vec2 vec2 = (agent1.Position - agent2.Position).AsVec2.Normalized();
          zero += vec2;
          ++num;
        }
        agent1 = agent2;
        if (depthCount > 0)
        {
          if (num >= depthCount)
            break;
        }
      }
      return num == 0 ? Vec2.Invalid : zero * (1f / (float) num);
    }

    public static Vec2 GetGlobalOrganicDirection(this ColumnFormation columnFormation) => MBExtensions.GetGlobalOrganicDirectionAux(columnFormation);

    public static Vec2 GetGlobalHeadDirection(this ColumnFormation columnFormation) => MBExtensions.GetGlobalOrganicDirectionAux(columnFormation, 3);

    public static float GetTime(this MBCommon.TimeType timeType) => MBCommon.GetTime(timeType);

    public static IEnumerable<T> FindAllWithType<T>(this IEnumerable<GameEntity> entities) where T : ScriptComponentBehaviour => entities.SelectMany<GameEntity, T>((Func<GameEntity, IEnumerable<T>>) (e => e.GetScriptComponents<T>()));

    public static IEnumerable<T> FindAllWithType<T>(
      this IEnumerable<MissionObject> missionObjects)
      where T : MissionObject
    {
      return missionObjects.Where<MissionObject>((Func<MissionObject, bool>) (e => e != null && e is T)).Select<MissionObject, T>((Func<MissionObject, T>) (e => e as T));
    }

    public static List<GameEntity> FindAllWithCompatibleType(
      this IEnumerable<GameEntity> sceneProps,
      params System.Type[] types)
    {
      List<GameEntity> gameEntityList = new List<GameEntity>();
      foreach (GameEntity sceneProp in sceneProps)
      {
        foreach (object scriptComponent in sceneProp.GetScriptComponents())
        {
          System.Type type1 = scriptComponent.GetType();
          foreach (System.Type type2 in types)
          {
            if (type2.IsAssignableFrom(type1))
              gameEntityList.Add(sceneProp);
          }
        }
      }
      return gameEntityList;
    }

    public static List<MissionObject> FindAllWithCompatibleType(
      this IEnumerable<MissionObject> missionObjects,
      params System.Type[] types)
    {
      List<MissionObject> missionObjectList = new List<MissionObject>();
      foreach (MissionObject missionObject in missionObjects)
      {
        if (missionObject != null)
        {
          System.Type type1 = missionObject.GetType();
          foreach (System.Type type2 in types)
          {
            if (type2.IsAssignableFrom(type1))
              missionObjectList.Add(missionObject);
          }
        }
      }
      return missionObjectList;
    }

    private static void CollectObjectsAux<T>(GameEntity entity, List<T> list) where T : ScriptComponentBehaviour
    {
      IEnumerable<T> scriptComponents = entity.GetScriptComponents<T>();
      list.AddRange(scriptComponents);
      foreach (GameEntity child in entity.GetChildren())
        MBExtensions.CollectObjectsAux<T>(child, list);
    }

    public static List<T> CollectObjects<T>(this GameEntity entity) where T : ScriptComponentBehaviour
    {
      List<T> list = new List<T>();
      MBExtensions.CollectObjectsAux<T>(entity, list);
      return list;
    }

    public static List<T> CollectObjectsWithTag<T>(this GameEntity entity, string tag) where T : ScriptComponentBehaviour
    {
      List<T> objList = new List<T>();
      foreach (GameEntity child in entity.GetChildren())
      {
        if (child.HasTag(tag))
        {
          IEnumerable<T> scriptComponents = child.GetScriptComponents<T>();
          objList.AddRange(scriptComponents);
        }
        if (child.ChildCount > 0)
          objList.AddRange((IEnumerable<T>) child.CollectObjectsWithTag<T>(tag));
      }
      return objList;
    }

    public static List<GameEntity> CollectChildrenEntitiesWithTag(
      this GameEntity entity,
      string tag)
    {
      List<GameEntity> gameEntityList = new List<GameEntity>();
      foreach (GameEntity child in entity.GetChildren())
      {
        if (child.HasTag(tag))
          gameEntityList.Add(child);
        if (child.ChildCount > 0)
          gameEntityList.AddRange((IEnumerable<GameEntity>) child.CollectChildrenEntitiesWithTag(tag));
      }
      return gameEntityList;
    }

    public static GameEntity GetFirstChildEntityWithTag(
      this GameEntity entity,
      string tag)
    {
      foreach (GameEntity child in entity.GetChildren())
      {
        if (child.HasTag(tag))
          return child;
      }
      return (GameEntity) null;
    }

    public static bool HasParentOfType(this GameEntity e, System.Type t)
    {
      do
      {
        e = e.Parent;
        if (e.GetScriptComponents().Any<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (sc => sc.GetType() == t)))
          return true;
      }
      while ((NativeObject) e != (NativeObject) null);
      return false;
    }

    public static TSource ElementAtOrValue<TSource>(
      this IEnumerable<TSource> source,
      int index,
      TSource value)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (index >= 0)
      {
        if (source is IList<TSource> sourceList2)
        {
          if (index < sourceList2.Count)
            return sourceList2[index];
        }
        else
        {
          foreach (TSource source1 in source)
          {
            if (index == 0)
              return source1;
            --index;
          }
        }
      }
      return value;
    }

    public static bool IsOpponentOf(this BattleSideEnum s, BattleSideEnum side)
    {
      if (s == BattleSideEnum.Attacker && side == BattleSideEnum.Defender)
        return true;
      return s == BattleSideEnum.Defender && side == BattleSideEnum.Attacker;
    }
  }
}
