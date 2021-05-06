﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.ObsoleteObjectManager
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
  [SaveableClass(10027)]
  public sealed class ObsoleteObjectManager
  {
    [SaveableField(9)]
    private int _lastGeneratedId;
    [SaveableField(10)]
    internal List<ObsoleteObjectManager.IObjectTypeRecord> ObjectTypeRecords = new List<ObsoleteObjectManager.IObjectTypeRecord>();

    internal static void AutoGeneratedStaticCollectObjectsObsoleteObjectManager(
      object o,
      List<object> collectedObjects)
    {
      ((ObsoleteObjectManager) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => collectedObjects.Add((object) this.ObjectTypeRecords);

    internal static object AutoGeneratedGetMemberValueObjectTypeRecords(object o) => (object) ((ObsoleteObjectManager) o).ObjectTypeRecords;

    internal static object AutoGeneratedGetMemberValue_lastGeneratedId(object o) => (object) ((ObsoleteObjectManager) o)._lastGeneratedId;

    public static ObsoleteObjectManager Instance { get; set; }

    private ObsoleteObjectManager()
    {
    }

    internal int GetLastGeneratedId() => this._lastGeneratedId;

    internal MBObjectBase GetObject(MBGUID objectId)
    {
      foreach (ObsoleteObjectManager.IObjectTypeRecord objectTypeRecord in this.ObjectTypeRecords)
      {
        if (objectTypeRecord != null && (int) objectTypeRecord.TypeNo == (int) objectId.GetTypeIndex())
          return objectTypeRecord.GetMBObject(objectId);
      }
      return (MBObjectBase) null;
    }

    [SaveableClass(10028)]
    public class ObjectTypeRecord<T> : ObsoleteObjectManager.IObjectTypeRecord where T : MBObjectBase
    {
      [SaveableField(6)]
      internal Dictionary<string, T> RegisteredObjects;
      [SaveableField(7)]
      internal Dictionary<MBGUID, T> RegisteredObjectsWithGuid;
      [SaveableField(8)]
      public readonly List<T> RegisteredObjectsList;

      protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
      {
        collectedObjects.Add((object) this.RegisteredObjects);
        collectedObjects.Add((object) this.RegisteredObjectsWithGuid);
        collectedObjects.Add((object) this.RegisteredObjectsList);
      }

      [SaveableProperty(1)]
      public bool AutoCreate { get; protected set; }

      [SaveableProperty(2)]
      public string ElementName { get; protected set; }

      [SaveableProperty(3)]
      public string ElementListName { get; protected set; }

      public Type ObjectClass => typeof (T);

      [SaveableProperty(4)]
      public uint ObjCount { get; set; }

      [SaveableProperty(5)]
      public uint TypeNo { get; protected set; }

      private ObjectTypeRecord()
      {
        this.RegisteredObjects = new Dictionary<string, T>();
        this.RegisteredObjectsWithGuid = new Dictionary<MBGUID, T>();
        this.RegisteredObjectsList = new List<T>();
      }

      MBObjectBase ObsoleteObjectManager.IObjectTypeRecord.GetMBObject(
        MBGUID objId)
      {
        T obj = default (T);
        this.RegisteredObjectsWithGuid.TryGetValue(objId, out obj);
        return (MBObjectBase) obj;
      }
    }

    [SaveableInterface(10610)]
    internal interface IObjectTypeRecord
    {
      bool AutoCreate { get; }

      string ElementName { get; }

      string ElementListName { get; }

      Type ObjectClass { get; }

      uint ObjCount { get; }

      uint TypeNo { get; }

      MBObjectBase GetMBObject(MBGUID objId);
    }
  }
}