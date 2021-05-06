// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.HasTableauCache
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Engine
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public class HasTableauCache : Attribute
  {
    public System.Type TableauCacheType { get; set; }

    public System.Type MaterialCacheIDGetType { get; set; }

    internal static Dictionary<System.Type, MaterialCacheIDGetMethodDelegate> TableauCacheTypes { get; private set; }

    public HasTableauCache(System.Type tableauCacheType, System.Type materialCacheIDGetType)
    {
      this.TableauCacheType = tableauCacheType;
      this.MaterialCacheIDGetType = materialCacheIDGetType;
    }

    public static void CollectTableauCacheTypes()
    {
      HasTableauCache.TableauCacheTypes = new Dictionary<System.Type, MaterialCacheIDGetMethodDelegate>();
      HasTableauCache.CollectTableauCacheTypesFrom(typeof (HasTableauCache).Assembly);
      foreach (Assembly viewAssembly in HasTableauCache.GetViewAssemblies())
        HasTableauCache.CollectTableauCacheTypesFrom(viewAssembly);
    }

    private static void CollectTableauCacheTypesFrom(Assembly assembly)
    {
      object[] customAttributes = assembly.GetCustomAttributes(typeof (HasTableauCache), true);
      if (customAttributes.Length == 0)
        return;
      foreach (HasTableauCache hasTableauCache in customAttributes)
      {
        MaterialCacheIDGetMethodDelegate getMethodDelegate = (MaterialCacheIDGetMethodDelegate) Delegate.CreateDelegate(typeof (MaterialCacheIDGetMethodDelegate), hasTableauCache.MaterialCacheIDGetType.GetMethod("GetMaterialCacheID", BindingFlags.Static | BindingFlags.Public));
        HasTableauCache.TableauCacheTypes.Add(hasTableauCache.TableauCacheType, getMethodDelegate);
      }
    }

    private static Assembly[] GetViewAssemblies()
    {
      List<Assembly> assemblyList = new List<Assembly>();
      Assembly assembly1 = typeof (HasTableauCache).Assembly;
      foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (object referencedAssembly in assembly2.GetReferencedAssemblies())
        {
          if (referencedAssembly.ToString() == assembly1.GetName().ToString())
          {
            assemblyList.Add(assembly2);
            break;
          }
        }
      }
      return assemblyList.ToArray();
    }
  }
}
