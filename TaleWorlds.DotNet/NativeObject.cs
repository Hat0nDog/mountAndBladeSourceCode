// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.NativeObject
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.DotNet
{
  public abstract class NativeObject
  {
    private static List<EngineClassTypeDefinition> _typeDefinitions;
    private static List<ConstructorInfo> _constructors;
    private const int NativeObjectFirstReferencesTickCount = 10;
    private static List<List<NativeObject>> _nativeObjectFirstReferences;
    private static volatile int _numberOfAliveNativeObjects;
    private readonly UIntPtr _pointer;
    private bool manualInvalidated;

    internal static int NumberOfAliveNativeObjects => NativeObject._numberOfAliveNativeObjects;

    internal UIntPtr Pointer => this._pointer;

    protected NativeObject(UIntPtr pointer)
    {
      this._pointer = pointer;
      LibraryApplicationInterface.IManaged.IncreaseReferenceCount(this.Pointer);
      lock (NativeObject._nativeObjectFirstReferences)
        NativeObject._nativeObjectFirstReferences[0].Add(this);
    }

    ~NativeObject()
    {
      if (this.manualInvalidated)
        return;
      LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
    }

    public void ManualInvalidate()
    {
      if (this.manualInvalidated)
        return;
      LibraryApplicationInterface.IManaged.DecreaseReferenceCount(this.Pointer);
      this.manualInvalidated = true;
    }

    static NativeObject()
    {
      int typeDefinitionCount = LibraryApplicationInterface.IManaged.GetClassTypeDefinitionCount();
      NativeObject._typeDefinitions = new List<EngineClassTypeDefinition>();
      NativeObject._constructors = new List<ConstructorInfo>();
      for (int index = 0; index < typeDefinitionCount; ++index)
      {
        EngineClassTypeDefinition engineClassTypeDefinition = new EngineClassTypeDefinition();
        LibraryApplicationInterface.IManaged.GetClassTypeDefinition(index, ref engineClassTypeDefinition);
        NativeObject._typeDefinitions.Add(engineClassTypeDefinition);
        NativeObject._constructors.Add((ConstructorInfo) null);
      }
      List<Type> typeList = new List<Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        try
        {
          if (NativeObject.DoesNativeObjectDefinedAssembly(assembly))
          {
            foreach (Type type in assembly.GetTypes())
            {
              if (type.GetCustomAttributes(typeof (EngineClass), false).Length == 1)
                typeList.Add(type);
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      foreach (Type type in typeList)
      {
        EngineClass customAttribute = (EngineClass) type.GetCustomAttributes(typeof (EngineClass), false)[0];
        if (!type.IsAbstract)
        {
          ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, new Type[1]
          {
            typeof (UIntPtr)
          }, (ParameterModifier[]) null);
          int typeDefinitionId = NativeObject.GetTypeDefinitionId(customAttribute.EngineType);
          if (typeDefinitionId != -1)
            NativeObject._constructors[typeDefinitionId] = constructor;
        }
      }
      NativeObject._nativeObjectFirstReferences = new List<List<NativeObject>>();
      for (int index = 0; index < 10; ++index)
        NativeObject._nativeObjectFirstReferences.Add(new List<NativeObject>());
    }

    internal static void HandleNativeObjects()
    {
      lock (NativeObject._nativeObjectFirstReferences)
      {
        List<NativeObject> objectFirstReference = NativeObject._nativeObjectFirstReferences[9];
        for (int index = 9; index > 0; --index)
          NativeObject._nativeObjectFirstReferences[index] = NativeObject._nativeObjectFirstReferences[index - 1];
        objectFirstReference.Clear();
        NativeObject._nativeObjectFirstReferences[0] = objectFirstReference;
      }
    }

    [LibraryCallback]
    internal static int GetAliveNativeObjectCount() => NativeObject._numberOfAliveNativeObjects;

    [LibraryCallback]
    internal static string GetAliveNativeObjectNames() => "";

    private static int GetTypeDefinitionId(string typeName)
    {
      foreach (EngineClassTypeDefinition typeDefinition in NativeObject._typeDefinitions)
      {
        if (typeDefinition.TypeName == typeName)
          return typeDefinition.TypeId;
      }
      return -1;
    }

    private static bool DoesNativeObjectDefinedAssembly(Assembly assembly)
    {
      if (typeof (NativeObject).Assembly.FullName == assembly.FullName)
        return true;
      AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
      string fullName = typeof (NativeObject).Assembly.FullName;
      foreach (AssemblyName assemblyName in referencedAssemblies)
      {
        if (assemblyName.FullName == fullName)
          return true;
      }
      return false;
    }

    [Obsolete]
    protected void AddUnmanagedMemoryPressure(int size)
    {
    }

    internal static NativeObject CreateNativeObjectWrapper(
      NativeObjectPointer nativeObjectPointer)
    {
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        ConstructorInfo constructor = NativeObject._constructors[nativeObjectPointer.TypeId];
        if (constructor != (ConstructorInfo) null)
          return (NativeObject) constructor.Invoke(new object[1]
          {
            (object) nativeObjectPointer.Pointer
          });
      }
      return (NativeObject) null;
    }

    internal static T CreateNativeObjectWrapper<T>(NativeObjectPointer nativeObjectPointer) where T : NativeObject => (T) NativeObject.CreateNativeObjectWrapper(nativeObjectPointer);

    public override int GetHashCode() => this._pointer.GetHashCode();

    public override bool Equals(object obj) => obj != null && !(obj.GetType() != this.GetType()) && ((NativeObject) obj)._pointer == this._pointer;

    public static bool operator ==(NativeObject a, NativeObject b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals((object) b);
    }

    public static bool operator !=(NativeObject a, NativeObject b) => !(a == b);
  }
}
