// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.DotNetObject
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  public class DotNetObject
  {
    private static readonly object Locker = new object();
    private const int DotnetObjectFirstReferencesTickCount = 200;
    private static readonly List<Dictionary<int, DotNetObject>> DotnetObjectFirstReferences;
    private static readonly Dictionary<int, DotNetObjectReferenceCounter> DotnetObjectReferences = new Dictionary<int, DotNetObjectReferenceCounter>();
    private static int _totalCreatedObjectCount;
    private readonly int _objectId;
    private static int _numberOfAliveDotNetObjects;

    internal static int NumberOfAliveDotNetObjects => DotNetObject._numberOfAliveDotNetObjects;

    static DotNetObject()
    {
      DotNetObject.DotnetObjectFirstReferences = new List<Dictionary<int, DotNetObject>>();
      for (int index = 0; index < 200; ++index)
        DotNetObject.DotnetObjectFirstReferences.Add(new Dictionary<int, DotNetObject>());
    }

    protected DotNetObject()
    {
      lock (DotNetObject.Locker)
      {
        ++DotNetObject._totalCreatedObjectCount;
        this._objectId = DotNetObject._totalCreatedObjectCount;
        DotNetObject.DotnetObjectFirstReferences[0].Add(this._objectId, this);
        ++DotNetObject._numberOfAliveDotNetObjects;
      }
    }

    ~DotNetObject()
    {
      lock (DotNetObject.Locker)
        --DotNetObject._numberOfAliveDotNetObjects;
    }

    [LibraryCallback]
    internal static int GetAliveDotNetObjectCount() => DotNetObject._numberOfAliveDotNetObjects;

    [LibraryCallback]
    internal static void IncreaseReferenceCount(int dotnetObjectId)
    {
      lock (DotNetObject.Locker)
      {
        if (DotNetObject.DotnetObjectReferences.ContainsKey(dotnetObjectId))
        {
          DotNetObjectReferenceCounter dotnetObjectReference = DotNetObject.DotnetObjectReferences[dotnetObjectId];
          ++dotnetObjectReference.ReferenceCount;
          DotNetObject.DotnetObjectReferences[dotnetObjectId] = dotnetObjectReference;
        }
        else
        {
          DotNetObject fromFirstReferences = DotNetObject.GetDotNetObjectFromFirstReferences(dotnetObjectId);
          DotNetObject.DotnetObjectReferences.Add(dotnetObjectId, new DotNetObjectReferenceCounter()
          {
            ReferenceCount = 1,
            DotNetObject = fromFirstReferences
          });
        }
      }
    }

    [LibraryCallback]
    internal static void DecreaseReferenceCount(int dotnetObjectId)
    {
      lock (DotNetObject.Locker)
      {
        DotNetObjectReferenceCounter dotnetObjectReference = DotNetObject.DotnetObjectReferences[dotnetObjectId];
        --dotnetObjectReference.ReferenceCount;
        if (dotnetObjectReference.ReferenceCount == 0)
          DotNetObject.DotnetObjectReferences.Remove(dotnetObjectId);
        else
          DotNetObject.DotnetObjectReferences[dotnetObjectId] = dotnetObjectReference;
      }
    }

    internal static DotNetObject GetManagedObjectWithId(int dotnetObjectId)
    {
      lock (DotNetObject.Locker)
      {
        DotNetObjectReferenceCounter referenceCounter;
        return DotNetObject.DotnetObjectReferences.TryGetValue(dotnetObjectId, out referenceCounter) ? referenceCounter.DotNetObject : DotNetObject.GetDotNetObjectFromFirstReferences(dotnetObjectId);
      }
    }

    private static DotNetObject GetDotNetObjectFromFirstReferences(int dotnetObjectId)
    {
      foreach (Dictionary<int, DotNetObject> objectFirstReference in DotNetObject.DotnetObjectFirstReferences)
      {
        DotNetObject dotNetObject;
        if (objectFirstReference.TryGetValue(dotnetObjectId, out dotNetObject))
          return dotNetObject;
      }
      return (DotNetObject) null;
    }

    internal int GetManagedId() => this._objectId;

    [LibraryCallback]
    internal static string GetAliveDotNetObjectNames()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (GetAliveDotNetObjectNames));
      lock (DotNetObject.Locker)
      {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (DotNetObjectReferenceCounter referenceCounter in DotNetObject.DotnetObjectReferences.Values)
        {
          Type type = referenceCounter.DotNetObject.GetType();
          if (!dictionary.ContainsKey(type.Name))
            dictionary.Add(type.Name, 1);
          else
            dictionary[type.Name] = dictionary[type.Name] + 1;
        }
        foreach (string key in dictionary.Keys)
          mbStringBuilder.Append<string>(key + "," + (object) dictionary[key] + "-");
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    internal static void HandleDotNetObjects()
    {
      lock (DotNetObject.Locker)
      {
        Dictionary<int, DotNetObject> objectFirstReference = DotNetObject.DotnetObjectFirstReferences[199];
        for (int index = 199; index > 0; --index)
          DotNetObject.DotnetObjectFirstReferences[index] = DotNetObject.DotnetObjectFirstReferences[index - 1];
        objectFirstReference.Clear();
        DotNetObject.DotnetObjectFirstReferences[0] = objectFirstReference;
      }
    }
  }
}
