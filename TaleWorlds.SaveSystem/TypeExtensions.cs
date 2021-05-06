// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.TypeExtensions
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.SaveSystem
{
  internal static class TypeExtensions
  {
    internal static bool IsContainer(this Type type) => type.IsContainer(out ContainerType _);

    internal static bool IsContainer(this Type type, out ContainerType containerType)
    {
      containerType = ContainerType.None;
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
      {
        Type genericTypeDefinition = type.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (Dictionary<,>))
        {
          containerType = ContainerType.Dictionary;
          return true;
        }
        if (genericTypeDefinition == typeof (List<>))
        {
          containerType = ContainerType.List;
          return true;
        }
        if (genericTypeDefinition == typeof (Queue<>))
        {
          containerType = ContainerType.Queue;
          return true;
        }
      }
      else if (type.IsArray)
      {
        containerType = ContainerType.Array;
        return true;
      }
      return false;
    }
  }
}
