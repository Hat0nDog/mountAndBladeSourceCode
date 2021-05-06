// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPRandomOnSpawnPerkEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MPRandomOnSpawnPerkEffect : MPOnSpawnPerkEffectBase
  {
    protected static Dictionary<string, Type> Registered = new Dictionary<string, Type>();

    static MPRandomOnSpawnPerkEffect()
    {
      foreach (Type type in ((IEnumerable<Type>) Assembly.GetAssembly(typeof (MPRandomOnSpawnPerkEffect)).GetTypes()).Where<Type>((Func<Type, bool>) (t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof (MPRandomOnSpawnPerkEffect)))))
      {
        string key = (string) type.GetField("StringType", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue((object) null);
        MPRandomOnSpawnPerkEffect.Registered.Add(key, type);
      }
    }

    public static MPRandomOnSpawnPerkEffect CreateFrom(XmlNode node)
    {
      string key = node?.Attributes?["type"]?.Value;
      MPRandomOnSpawnPerkEffect instance = (MPRandomOnSpawnPerkEffect) Activator.CreateInstance(MPRandomOnSpawnPerkEffect.Registered[key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, (object[]) null, CultureInfo.InvariantCulture);
      instance.Deserialize(node);
      return instance;
    }
  }
}
