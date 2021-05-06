// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.TypeCollector`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
  public class TypeCollector<T> where T : class
  {
    private Dictionary<string, Type> _types;
    private Assembly _currentAssembly;

    public Type BaseType { get; private set; }

    public TypeCollector()
    {
      this.BaseType = typeof (T);
      this._types = new Dictionary<string, Type>();
      this._currentAssembly = this.BaseType.Assembly;
    }

    public void Collect()
    {
      List<Type> typeList = this.CollectTypes();
      this._types.Clear();
      foreach (Type type in typeList)
        this._types.Add(type.Name, type);
    }

    public T Instantiate(string typeName, params object[] parameters)
    {
      T obj = default (T);
      Type type;
      if (this._types.TryGetValue(typeName, out type))
        obj = (T) type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, (Binder) null, new Type[0], (ParameterModifier[]) null).Invoke(parameters);
      return obj;
    }

    public Type GetType(string typeName)
    {
      Type type;
      return this._types.TryGetValue(typeName, out type) ? type : (Type) null;
    }

    private bool CheckAssemblyReferencesThis(Assembly assembly)
    {
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.Name == this._currentAssembly.GetName().Name)
          return true;
      }
      return false;
    }

    private List<Type> CollectTypes()
    {
      List<Type> typeList = new List<Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (this.CheckAssemblyReferencesThis(assembly) || assembly == this._currentAssembly)
        {
          foreach (Type type in assembly.GetTypes())
          {
            if (this.BaseType.IsAssignableFrom(type))
              typeList.Add(type);
          }
        }
      }
      return typeList;
    }
  }
}
