// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.ManagedObject
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
  public abstract class ManagedObject
  {
    private const int ManagedObjectFirstReferencesTickCount = 200;
    private static List<List<ManagedObject>> _managedObjectFirstReferences = new List<List<ManagedObject>>();
    private ManagedObjectOwner _managedObjectOwner;
    private int forcedMemory;

    internal ManagedObjectOwner ManagedObjectOwner => this._managedObjectOwner;

    internal static void FinalizeManagedObjects()
    {
      lock (ManagedObject._managedObjectFirstReferences)
        ManagedObject._managedObjectFirstReferences.Clear();
    }

    protected void AddUnmanagedMemoryPressure(int size)
    {
      GC.AddMemoryPressure((long) size);
      this.forcedMemory = size;
    }

    static ManagedObject()
    {
      for (int index = 0; index < 200; ++index)
        ManagedObject._managedObjectFirstReferences.Add(new List<ManagedObject>());
    }

    protected ManagedObject(UIntPtr ptr, bool createManagedObjectOwner)
    {
      if (createManagedObjectOwner)
        this.SetOwnerManagedObject(ManagedObjectOwner.CreateManagedObjectOwner(ptr, this));
      this.Initialize();
    }

    internal void SetOwnerManagedObject(ManagedObjectOwner owner) => this._managedObjectOwner = owner;

    private void Initialize() => ManagedObject.ManagedObjectFetched(this);

    ~ManagedObject()
    {
      if (this.forcedMemory > 0)
        GC.RemoveMemoryPressure((long) this.forcedMemory);
      ManagedObjectOwner.ManagedObjectGarbageCollected(this._managedObjectOwner, this);
      this._managedObjectOwner = (ManagedObjectOwner) null;
    }

    internal static void HandleManagedObjects()
    {
      lock (ManagedObject._managedObjectFirstReferences)
      {
        List<ManagedObject> objectFirstReference = ManagedObject._managedObjectFirstReferences[199];
        for (int index = 199; index > 0; --index)
          ManagedObject._managedObjectFirstReferences[index] = ManagedObject._managedObjectFirstReferences[index - 1];
        objectFirstReference.Clear();
        ManagedObject._managedObjectFirstReferences[0] = objectFirstReference;
      }
    }

    internal static void ManagedObjectFetched(ManagedObject managedObject)
    {
      lock (ManagedObject._managedObjectFirstReferences)
      {
        if (Managed.Closing)
          return;
        ManagedObject._managedObjectFirstReferences[0].Add(managedObject);
      }
    }

    internal static void FlushManagedObjects()
    {
      lock (ManagedObject._managedObjectFirstReferences)
      {
        for (int index = 0; index < 200; ++index)
          ManagedObject._managedObjectFirstReferences[index].Clear();
      }
    }

    [LibraryCallback]
    internal static int GetAliveManagedObjectCount() => ManagedObjectOwner.NumberOfAliveManagedObjects;

    [LibraryCallback]
    internal static string GetAliveManagedObjectNames() => ManagedObjectOwner.GetAliveManagedObjectNames();

    [LibraryCallback]
    internal static string GetCreationCallstack(string name) => ManagedObjectOwner.GetAliveManagedObjectCreationCallstacks(name);

    internal UIntPtr Pointer
    {
      get => this._managedObjectOwner.Pointer;
      set => this._managedObjectOwner.Pointer = value;
    }

    public int GetManagedId() => this._managedObjectOwner.NativeId;

    [LibraryCallback]
    internal string GetClassOfObject() => this.GetType().Name;

    public override int GetHashCode() => this._managedObjectOwner.NativeId;
  }
}
