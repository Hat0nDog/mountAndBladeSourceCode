// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.GameApplicationDomainController
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  public class GameApplicationDomainController : MarshalByRefObject
  {
    private static Delegate _passManagedInitializeMethod;
    private static Delegate _passManagedCallbackMethod;
    private static GameApplicationDomainController _instance;
    private bool _newApplicationDomain;

    public GameApplicationDomainController(bool newApplicationDomain)
    {
      Debug.Print("Constructing GameApplicationDomainController.");
      GameApplicationDomainController._instance = this;
      this._newApplicationDomain = newApplicationDomain;
    }

    public GameApplicationDomainController()
    {
      Debug.Print("Constructing GameApplicationDomainController.");
      GameApplicationDomainController._instance = this;
      this._newApplicationDomain = true;
    }

    public void LoadAsMono(
      IntPtr passManagedInitializeMethodPointer,
      IntPtr passManagedCallbackMethodPointer,
      string gameApiDllName,
      string gameApiTypeName,
      Platform currentPlatform)
    {
      this.Load(Marshal.GetDelegateForFunctionPointer(passManagedInitializeMethodPointer, typeof (OneMethodPasserDelegate)), Marshal.GetDelegateForFunctionPointer(passManagedCallbackMethodPointer, typeof (OneMethodPasserDelegate)), gameApiDllName, gameApiTypeName, currentPlatform);
    }

    public void Load(
      Delegate passManagedInitializeMethod,
      Delegate passManagedCallbackMethod,
      string gameApiDllName,
      string gameApiTypeName,
      Platform currentPlatform)
    {
      try
      {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        GameApplicationDomainController._passManagedInitializeMethod = passManagedInitializeMethod;
        GameApplicationDomainController._passManagedCallbackMethod = passManagedCallbackMethod;
        Assembly assembly1 = !this._newApplicationDomain ? this.GetType().Assembly : AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.DotNet.dll");
        Assembly assembly2 = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + gameApiDllName);
        Type type1 = assembly1.GetType("TaleWorlds.DotNet.Managed");
        Type type2 = assembly2.GetType(gameApiTypeName);
        type1.GetMethod("SetAsDotNet").Invoke((object) null, new object[0]);
        type1.GetMethod("PassInitializationMethodPointersForDotNet").Invoke((object) null, new object[2]
        {
          (object) GameApplicationDomainController._passManagedInitializeMethod,
          (object) GameApplicationDomainController._passManagedCallbackMethod
        });
        type2.GetMethod("Start").Invoke((object) null, new object[0]);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.GetType().Name);
        Console.WriteLine(ex.Message);
        if (ex.InnerException == null)
          return;
        Console.WriteLine("-");
        Console.WriteLine(ex.InnerException.Message);
        if (ex.InnerException.InnerException == null)
          return;
        Console.WriteLine("-");
        Console.WriteLine(ex.InnerException.InnerException.Message);
      }
    }

    private delegate void InitializerDelegate(Delegate argument);
  }
}
