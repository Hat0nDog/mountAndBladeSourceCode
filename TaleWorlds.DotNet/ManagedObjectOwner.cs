// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.ManagedObjectOwner
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  internal class ManagedObjectOwner
  {
    private const int PooledManagedObjectOwnerCount = 8192;
    private static readonly List<ManagedObjectOwner> _pool;
    private static readonly List<WeakReference> _managedObjectOwnerWeakReferences;
    private static readonly Dictionary<int, WeakReference> _managedObjectOwners;
    private static readonly HashSet<ManagedObjectOwner> _managedObjectOwnerReferences;
    private static int _lastId = 10;
    private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjects;
    private static int _numberOfAliveManagedObjects = 0;
    private static readonly List<ManagedObjectOwner> _lastframedeletedManagedObjectBuffer;
    private Type _typeInfo;
    private int _nativeId;
    private UIntPtr _ptr;
    private readonly WeakReference _managedObject;
    private readonly WeakReference _managedObjectLongReference;

    internal static int NumberOfAliveManagedObjects => ManagedObjectOwner._numberOfAliveManagedObjects;

    static ManagedObjectOwner()
    {
      ManagedObjectOwner._managedObjectOwners = new Dictionary<int, WeakReference>();
      ManagedObjectOwner._lastframedeletedManagedObjects = new List<ManagedObjectOwner>();
      ManagedObjectOwner._managedObjectOwnerReferences = new HashSet<ManagedObjectOwner>();
      ManagedObjectOwner._lastframedeletedManagedObjectBuffer = new List<ManagedObjectOwner>(1024);
      ManagedObjectOwner._pool = new List<ManagedObjectOwner>(8192);
      ManagedObjectOwner._managedObjectOwnerWeakReferences = new List<WeakReference>(8192);
      for (int index = 0; index < 8192; ++index)
      {
        ManagedObjectOwner managedObjectOwner = new ManagedObjectOwner();
        ManagedObjectOwner._pool.Add(managedObjectOwner);
        WeakReference weakReference = new WeakReference((object) null);
        ManagedObjectOwner._managedObjectOwnerWeakReferences.Add(weakReference);
      }
    }

    internal static void GarbageCollect()
    {
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        ManagedObjectOwner._lastframedeletedManagedObjectBuffer.AddRange((IEnumerable<ManagedObjectOwner>) ManagedObjectOwner._lastframedeletedManagedObjects);
        ManagedObjectOwner._lastframedeletedManagedObjects.Clear();
        foreach (ManagedObjectOwner managedObjectOwner1 in ManagedObjectOwner._lastframedeletedManagedObjectBuffer)
        {
          if (managedObjectOwner1._ptr != UIntPtr.Zero)
          {
            LibraryApplicationInterface.IManaged.ReleaseManagedObject(managedObjectOwner1._ptr);
            managedObjectOwner1._ptr = UIntPtr.Zero;
          }
          --ManagedObjectOwner._numberOfAliveManagedObjects;
          WeakReference managedObjectOwner2 = ManagedObjectOwner._managedObjectOwners[managedObjectOwner1.NativeId];
          ManagedObjectOwner._managedObjectOwners.Remove(managedObjectOwner1.NativeId);
          managedObjectOwner2.Target = (object) null;
          ManagedObjectOwner._managedObjectOwnerWeakReferences.Add(managedObjectOwner2);
        }
      }
      foreach (ManagedObjectOwner managedObjectOwner in ManagedObjectOwner._lastframedeletedManagedObjectBuffer)
      {
        managedObjectOwner.Destruct();
        ManagedObjectOwner._pool.Add(managedObjectOwner);
      }
      ManagedObjectOwner._lastframedeletedManagedObjectBuffer.Clear();
    }

    internal static void LogFinalize()
    {
      Debug.Print("Checking if any managed object still lives...");
      int num = 0;
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        foreach (KeyValuePair<int, WeakReference> managedObjectOwner in ManagedObjectOwner._managedObjectOwners)
        {
          if (managedObjectOwner.Value.Target != null)
          {
            Debug.Print("An object with type of " + managedObjectOwner.Value.Target.GetType().Name + " still lives");
            ++num;
          }
        }
      }
      if (num == 0)
        Debug.Print("There are no living managed objects.");
      else
        Debug.Print("There are " + (object) num + " living managed objects.");
    }

    internal static void PreFinalizeManagedObjects() => ManagedObjectOwner.GarbageCollect();

    internal static ManagedObject GetManagedObjectWithId(int id)
    {
      if (id == 0)
        return (ManagedObject) null;
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        if (ManagedObjectOwner._managedObjectOwners[id].Target is ManagedObjectOwner target1)
        {
          ManagedObject managedObject = target1.TryGetManagedObject();
          ManagedObject.ManagedObjectFetched(managedObject);
          return managedObject;
        }
      }
      return (ManagedObject) null;
    }

    internal static void ManagedObjectGarbageCollected(
      ManagedObjectOwner owner,
      ManagedObject managedObject)
    {
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        if (owner == null || owner._managedObjectLongReference.Target as ManagedObject != managedObject)
          return;
        ManagedObjectOwner._lastframedeletedManagedObjects.Add(owner);
        ManagedObjectOwner._managedObjectOwnerReferences.Remove(owner);
      }
    }

    internal static ManagedObjectOwner CreateManagedObjectOwner(
      UIntPtr ptr,
      ManagedObject managedObject)
    {
      ManagedObjectOwner managedObjectOwner;
      if (ManagedObjectOwner._pool.Count > 0)
      {
        managedObjectOwner = ManagedObjectOwner._pool[ManagedObjectOwner._pool.Count - 1];
        ManagedObjectOwner._pool.RemoveAt(ManagedObjectOwner._pool.Count - 1);
      }
      else
        managedObjectOwner = new ManagedObjectOwner();
      managedObjectOwner.Construct(ptr, managedObject);
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        ++ManagedObjectOwner._numberOfAliveManagedObjects;
        ManagedObjectOwner._managedObjectOwnerReferences.Add(managedObjectOwner);
      }
      return managedObjectOwner;
    }

    internal static string GetAliveManagedObjectNames()
    {
      string str = "";
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        foreach (WeakReference weakReference in ManagedObjectOwner._managedObjectOwners.Values)
        {
          ManagedObjectOwner target = weakReference.Target as ManagedObjectOwner;
          if (!dictionary.ContainsKey(target._typeInfo.Name))
            dictionary.Add(target._typeInfo.Name, 1);
          else
            dictionary[target._typeInfo.Name] = dictionary[target._typeInfo.Name] + 1;
        }
        foreach (string key in dictionary.Keys)
          str = str + key + "," + (object) dictionary[key] + "-";
      }
      return str;
    }

    internal static string GetAliveManagedObjectCreationCallstacks(string name) => "";

    internal int NativeId => this._nativeId;

    internal UIntPtr Pointer
    {
      get => this._ptr;
      set
      {
        if (value != UIntPtr.Zero)
          LibraryApplicationInterface.IManaged.IncreaseReferenceCount(value);
        this._ptr = value;
      }
    }

    private ManagedObjectOwner()
    {
      this._ptr = UIntPtr.Zero;
      this._managedObject = new WeakReference((object) null, false);
      this._managedObjectLongReference = new WeakReference((object) null, true);
    }

    private void Construct(UIntPtr ptr, ManagedObject managedObject)
    {
      this._typeInfo = managedObject.GetType();
      this._managedObject.Target = (object) managedObject;
      this._managedObjectLongReference.Target = (object) managedObject;
      this.Pointer = ptr;
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        this._nativeId = ManagedObjectOwner._lastId;
        ++ManagedObjectOwner._lastId;
        WeakReference weakReference;
        if (ManagedObjectOwner._managedObjectOwnerWeakReferences.Count > 0)
        {
          weakReference = ManagedObjectOwner._managedObjectOwnerWeakReferences[ManagedObjectOwner._managedObjectOwnerWeakReferences.Count - 1];
          ManagedObjectOwner._managedObjectOwnerWeakReferences.RemoveAt(ManagedObjectOwner._managedObjectOwnerWeakReferences.Count - 1);
          weakReference.Target = (object) this;
        }
        else
          weakReference = new WeakReference((object) this);
        ManagedObjectOwner._managedObjectOwners.Add(this.NativeId, weakReference);
      }
    }

    private void Destruct()
    {
      this._managedObject.Target = (object) null;
      this._managedObjectLongReference.Target = (object) null;
      this._typeInfo = (Type) null;
      this._ptr = UIntPtr.Zero;
      this._nativeId = 0;
    }

    ~ManagedObjectOwner()
    {
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
        ;
    }

    private ManagedObject TryGetManagedObject()
    {
      managedObject1 = (ManagedObject) null;
      lock (ManagedObjectOwner._managedObjectOwnerReferences)
      {
        if (!(this._managedObject.Target is ManagedObject managedObject1))
        {
          managedObject1 = (ManagedObject) this._typeInfo.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[2]
          {
            typeof (UIntPtr),
            typeof (bool)
          }, (ParameterModifier[]) null).Invoke(new object[2]
          {
            (object) this._ptr,
            (object) false
          });
          managedObject1.SetOwnerManagedObject(this);
          this._managedObject.Target = (object) managedObject1;
          this._managedObjectLongReference.Target = (object) managedObject1;
          if (!ManagedObjectOwner._managedObjectOwnerReferences.Contains(this))
            ManagedObjectOwner._managedObjectOwnerReferences.Add(this);
          ManagedObjectOwner._lastframedeletedManagedObjects.Remove(this);
        }
      }
      return managedObject1;
    }
  }
}
