// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CustomRPCDataTypes
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  internal static class CustomRPCDataTypes
  {
    private static Dictionary<byte, Type> _typesOfIds;
    private static Dictionary<Type, byte> _idsOfTypes;
    private static byte _lastId = 127;

    static CustomRPCDataTypes()
    {
      CustomRPCDataTypes._typesOfIds = new Dictionary<byte, Type>();
      CustomRPCDataTypes._idsOfTypes = new Dictionary<Type, byte>();
      CustomRPCDataTypes.AddCustomTypes();
    }

    private static bool CheckAssemblyForCustomRPCData(Assembly assembly)
    {
      Assembly assembly1 = Assembly.GetAssembly(typeof (ISerializableObject));
      if (assembly == assembly1)
        return true;
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.FullName == assembly1.FullName)
          return true;
      }
      return false;
    }

    private static void AddCustomTypes()
    {
      Debug.Print("Searching Custom RPC Datas");
      List<Type> typeList = new List<Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (CustomRPCDataTypes.CheckAssemblyForCustomRPCData(assembly))
        {
          foreach (Type type in assembly.GetTypes())
          {
            if (typeof (ISerializableObject).IsAssignableFrom(type))
              typeList.Add(type);
          }
        }
      }
      Debug.Print("Found " + (object) typeList.Count + " custom rpc datas");
      foreach (Type type in typeList)
        CustomRPCDataTypes.AddType(type);
    }

    private static void AddType(Type type)
    {
      ++CustomRPCDataTypes._lastId;
      byte lastId = CustomRPCDataTypes._lastId;
      CustomRPCDataTypes._typesOfIds.Add(lastId, type);
      CustomRPCDataTypes._idsOfTypes.Add(type, lastId);
    }

    internal static byte GetId(Type valueType)
    {
      foreach (KeyValuePair<Type, byte> idsOfType in CustomRPCDataTypes._idsOfTypes)
      {
        Type key = idsOfType.Key;
        int num = (int) idsOfType.Value;
        Type type = valueType;
        if (key == type)
          return (byte) num;
      }
      return 0;
    }

    internal static bool IsCustomId(byte id) => CustomRPCDataTypes._typesOfIds.ContainsKey(id);

    internal static Type GetType(byte id) => CustomRPCDataTypes._typesOfIds.ContainsKey(id) ? CustomRPCDataTypes._typesOfIds[id] : (Type) null;
  }
}
