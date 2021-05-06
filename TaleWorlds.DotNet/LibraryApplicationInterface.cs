// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.LibraryApplicationInterface
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
  internal class LibraryApplicationInterface
  {
    internal static IManaged IManaged;
    internal static ITelemetry ITelemetry;
    internal static ILibrarySizeChecker ILibrarySizeChecker;
    internal static INativeArray INativeArray;
    internal static INativeObjectArray INativeObjectArray;
    private static Dictionary<string, object> _objects;

    private static T GetObject<T>() where T : class
    {
      object obj;
      return LibraryApplicationInterface._objects.TryGetValue(typeof (T).FullName, out obj) ? obj as T : default (T);
    }

    internal static void SetObjects(Dictionary<string, object> objects)
    {
      LibraryApplicationInterface._objects = objects;
      LibraryApplicationInterface.IManaged = LibraryApplicationInterface.GetObject<IManaged>();
      LibraryApplicationInterface.ITelemetry = LibraryApplicationInterface.GetObject<ITelemetry>();
      LibraryApplicationInterface.ILibrarySizeChecker = LibraryApplicationInterface.GetObject<ILibrarySizeChecker>();
      LibraryApplicationInterface.INativeArray = LibraryApplicationInterface.GetObject<INativeArray>();
      LibraryApplicationInterface.INativeObjectArray = LibraryApplicationInterface.GetObject<INativeObjectArray>();
    }
  }
}
