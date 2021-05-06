// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.Controller
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  public class Controller
  {
    private static Delegate _passControllerMethods;
    private static Delegate _passManagedInitializeMethod;
    private static Delegate _passManagedCallbackMethod;
    private static IntPtr _passManagedInitializeMethodPointer;
    private static IntPtr _passManagedCallbackMethodPointer;
    private static Controller.CreateApplicationDomainMethodDelegate _loadOnCurrentApplicationDomainMethod;

    private static TaleWorlds.Library.Runtime RuntimeLibrary { get; set; } = TaleWorlds.Library.Runtime.DotNet;

    [MonoPInvokeCallback(typeof (Controller.CreateApplicationDomainMethodDelegate))]
    public static void LoadOnCurrentApplicationDomain(
      IntPtr gameDllNameAsPointer,
      IntPtr gameTypeNameAsPointer,
      int currentPlatformAsInteger)
    {
      Platform currentPlatform = (Platform) currentPlatformAsInteger;
      ApplicationPlatform.Initialize(currentPlatform, Controller.RuntimeLibrary);
      string stringAnsi1 = Marshal.PtrToStringAnsi(gameDllNameAsPointer);
      string stringAnsi2 = Marshal.PtrToStringAnsi(gameTypeNameAsPointer);
      Debug.Print("Appending private path to current application domain.");
      AppDomain.CurrentDomain.AppendPrivatePath(ManagedDllFolder.Name);
      Debug.Print("Creating GameApplicationDomainController on current application domain.");
      GameApplicationDomainController domainController = new GameApplicationDomainController(false);
      if (domainController == null)
      {
        Console.WriteLine("GameApplicationDomainController is NULL!");
        Console.WriteLine("Press a key to continue...");
        Console.ReadKey();
      }
      if (Controller.RuntimeLibrary == TaleWorlds.Library.Runtime.Mono)
      {
        Debug.Print("Initializing GameApplicationDomainController as Mono.");
        domainController.LoadAsMono(Controller._passManagedInitializeMethodPointer, Controller._passManagedCallbackMethodPointer, stringAnsi1, stringAnsi2, currentPlatform);
      }
      else
      {
        Debug.Print("Initializing GameApplicationDomainController as Dot Net.");
        domainController.Load(Controller._passManagedInitializeMethod, Controller._passManagedCallbackMethod, stringAnsi1, stringAnsi2, currentPlatform);
      }
    }

    public static void SetEngineMethodsAsMono(
      IntPtr passControllerMethods,
      IntPtr passManagedInitializeMethod,
      IntPtr passManagedCallbackMethod)
    {
      Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono");
      Debug.Print("Beginning...");
      Controller.RuntimeLibrary = TaleWorlds.Library.Runtime.Mono;
      Controller._passControllerMethods = Marshal.GetDelegateForFunctionPointer(passControllerMethods, typeof (OneMethodPasserDelegate));
      Controller._passManagedInitializeMethodPointer = passManagedInitializeMethod;
      Controller._passManagedCallbackMethodPointer = passManagedCallbackMethod;
      Debug.Print("Starting controller...");
      Controller.Start();
      Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono - Done");
    }

    public static void SetEngineMethodsAsDotNet(
      Delegate passControllerMethods,
      Delegate passManagedInitializeMethod,
      Delegate passManagedCallbackMethod)
    {
      Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsDotNet");
      if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
        Controller.RuntimeLibrary = TaleWorlds.Library.Runtime.DotNet;
      else if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core"))
        Controller.RuntimeLibrary = TaleWorlds.Library.Runtime.DotNetCore;
      Controller._passControllerMethods = passControllerMethods;
      Controller._passManagedInitializeMethod = passManagedInitializeMethod;
      Controller._passManagedCallbackMethod = passManagedCallbackMethod;
      Controller.Start();
    }

    private static void Start()
    {
      Controller._loadOnCurrentApplicationDomainMethod = new Controller.CreateApplicationDomainMethodDelegate(Controller.LoadOnCurrentApplicationDomain);
      Controller.PassControllerMethods((Delegate) Controller._loadOnCurrentApplicationDomainMethod);
    }

    private static void PassControllerMethods(Delegate loadOnCurrentApplicationDomainMethod)
    {
      if ((object) Controller._passControllerMethods != null)
        Controller._passControllerMethods.DynamicInvoke((object) loadOnCurrentApplicationDomainMethod);
      else
        Debug.Print("Could not find _passControllerMethods");
    }

    [MonoNativeFunctionWrapper]
    private delegate void ControllerMethodDelegate();

    [MonoNativeFunctionWrapper]
    private delegate void CreateApplicationDomainMethodDelegate(
      IntPtr gameDllNameAsPointer,
      IntPtr gameTypeNameAsPointer,
      int currentPlatformAsInteger);
  }
}
