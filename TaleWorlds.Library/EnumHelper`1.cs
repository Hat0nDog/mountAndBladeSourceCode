// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.EnumHelper`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Reflection;

namespace TaleWorlds.Library
{
  internal static class EnumHelper<T1>
  {
    public static Func<T1, T1, bool> HasAnyFlag = new Func<T1, T1, bool>(EnumHelper<T1>.initProc);
    public static Func<T1, T1, bool> HasAllFlags = new Func<T1, T1, bool>(EnumHelper<T1>.initAllProc);

    public static bool Overlaps(sbyte p1, sbyte p2) => ((uint) p1 & (uint) p2) > 0U;

    public static bool Overlaps(byte p1, byte p2) => ((uint) p1 & (uint) p2) > 0U;

    public static bool Overlaps(short p1, short p2) => ((uint) p1 & (uint) p2) > 0U;

    public static bool Overlaps(ushort p1, ushort p2) => ((uint) p1 & (uint) p2) > 0U;

    public static bool Overlaps(int p1, int p2) => (uint) (p1 & p2) > 0U;

    public static bool Overlaps(uint p1, uint p2) => (p1 & p2) > 0U;

    public static bool Overlaps(long p1, long p2) => (ulong) (p1 & p2) > 0UL;

    public static bool Overlaps(ulong p1, ulong p2) => (p1 & p2) > 0UL;

    public static bool ContainsAll(sbyte p1, sbyte p2) => ((int) p1 & (int) p2) == (int) p2;

    public static bool ContainsAll(byte p1, byte p2) => ((int) p1 & (int) p2) == (int) p2;

    public static bool ContainsAll(short p1, short p2) => ((int) p1 & (int) p2) == (int) p2;

    public static bool ContainsAll(ushort p1, ushort p2) => ((int) p1 & (int) p2) == (int) p2;

    public static bool ContainsAll(int p1, int p2) => (p1 & p2) == p2;

    public static bool ContainsAll(uint p1, uint p2) => ((int) p1 & (int) p2) == (int) p2;

    public static bool ContainsAll(long p1, long p2) => (p1 & p2) == p2;

    public static bool ContainsAll(ulong p1, ulong p2) => ((long) p1 & (long) p2) == (long) p2;

    public static bool initProc(T1 p1, T1 p2)
    {
      Type enumType = typeof (T1);
      if (enumType.IsEnum)
        enumType = Enum.GetUnderlyingType(enumType);
      Type[] types = new Type[2]{ enumType, enumType };
      MethodInfo method = typeof (EnumHelper<T1>).GetMethod("Overlaps", types);
      if (method == (MethodInfo) null)
        method = typeof (T1).GetMethod("Overlaps", types);
      EnumHelper<T1>.HasAnyFlag = !(method == (MethodInfo) null) ? (Func<T1, T1, bool>) Delegate.CreateDelegate(typeof (Func<T1, T1, bool>), method) : throw new MissingMethodException("Unknown type of enum");
      return EnumHelper<T1>.HasAnyFlag(p1, p2);
    }

    public static bool initAllProc(T1 p1, T1 p2)
    {
      Type enumType = typeof (T1);
      if (enumType.IsEnum)
        enumType = Enum.GetUnderlyingType(enumType);
      Type[] types = new Type[2]{ enumType, enumType };
      MethodInfo method = typeof (EnumHelper<T1>).GetMethod("ContainsAll", types);
      if (method == (MethodInfo) null)
        method = typeof (T1).GetMethod("ContainsAll", types);
      EnumHelper<T1>.HasAllFlags = !(method == (MethodInfo) null) ? (Func<T1, T1, bool>) Delegate.CreateDelegate(typeof (Func<T1, T1, bool>), method) : throw new MissingMethodException("Unknown type of enum");
      return EnumHelper<T1>.HasAllFlags(p1, p2);
    }
  }
}
