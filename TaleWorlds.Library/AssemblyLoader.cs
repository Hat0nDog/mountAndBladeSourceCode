// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.AssemblyLoader
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
  public static class AssemblyLoader
  {
    private static List<Assembly> _loadedAssemblies = new List<Assembly>();

    static AssemblyLoader() => AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyLoader.OnAssemblyResolve);

    public static void Initialize()
    {
    }

    public static Assembly LoadFrom(string assemblyFile, bool show_error = true)
    {
      Assembly assembly = (Assembly) null;
      Debug.Print("Try to load dll " + assemblyFile + "\n");
      try
      {
        if (ApplicationPlatform.CurrentRuntimeLibrary == Runtime.DotNetCore)
        {
          try
          {
            assembly = Assembly.LoadFrom(assemblyFile);
          }
          catch (Exception ex)
          {
            assembly = (Assembly) null;
          }
          if (assembly != (Assembly) null)
          {
            if (!AssemblyLoader._loadedAssemblies.Contains(assembly))
            {
              AssemblyLoader._loadedAssemblies.Add(assembly);
              foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
              {
                string assemblyFile1 = referencedAssembly.Name + ".dll";
                if (!assemblyFile1.StartsWith("System") && !assemblyFile1.StartsWith("mscorlib"))
                  AssemblyLoader.LoadFrom(assemblyFile1);
              }
            }
          }
        }
        else
          assembly = Assembly.LoadFrom(assemblyFile);
      }
      catch
      {
        if (show_error)
          Debug.ShowMessageBox("Cannot load: " + assemblyFile, "ERROR", 4U);
      }
      return assembly;
    }

    private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (assembly.FullName == args.Name)
          return assembly;
      }
      if (ApplicationPlatform.CurrentRuntimeLibrary != Runtime.Mono || !ApplicationPlatform.IsPlatformWindows())
        return (Assembly) null;
      return AssemblyLoader.LoadFrom(args.Name.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)[0] + ".dll", false);
    }
  }
}
