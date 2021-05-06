// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.MBGUID
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
  [SaveableStruct(11003)]
  public struct MBGUID : IComparable, IEquatable<MBGUID>
  {
    private const int ObjectIdNumBits = 26;
    private const int ObjectIdBitFlag = 67108863;
    [SaveableField(1)]
    private readonly uint _internalValue;

    public MBGUID(uint id) => this._internalValue = id;

    internal MBGUID(uint objType, uint subId)
    {
      if (subId < 0U || subId > 67108863U)
        throw new MBOutOfRangeException(nameof (subId));
      this._internalValue = objType << 26 | subId;
    }

    public uint InternalValue => this._internalValue;

    public uint SubId => this._internalValue & 67108863U;

    public static bool operator ==(MBGUID id1, MBGUID id2) => (int) id1._internalValue == (int) id2._internalValue;

    public static bool operator !=(MBGUID id1, MBGUID id2) => (int) id1._internalValue != (int) id2._internalValue;

    public static bool operator <(MBGUID id1, MBGUID id2) => id1._internalValue < id2._internalValue;

    public static bool operator >(MBGUID id1, MBGUID id2) => id1._internalValue > id2._internalValue;

    public static bool operator <=(MBGUID id1, MBGUID id2) => id1._internalValue <= id2._internalValue;

    public static bool operator >=(MBGUID id1, MBGUID id2) => id1._internalValue >= id2._internalValue;

    public static long GetHash2(MBGUID id1, MBGUID id2)
    {
      if (id1 > id2)
      {
        MBGUID mbguid = id1;
        id1 = id2;
        id2 = mbguid;
      }
      return (long) id1.GetHashCode() * 1046527L + (long) id2.GetHashCode();
    }

    public int CompareTo(object a)
    {
      if (!(a is MBGUID mbguid))
        throw new MBTypeMismatchException("CompareTo function called with an invalid argument!");
      if ((int) this._internalValue == (int) mbguid._internalValue)
        return 0;
      return this._internalValue > ((MBGUID) a)._internalValue ? 1 : -1;
    }

    public uint GetTypeIndex() => this._internalValue >> 26;

    public override int GetHashCode() => (int) this._internalValue;

    public override string ToString() => this.InternalValue.ToString();

    public override bool Equals(object obj)
    {
      if (!(obj is MBGUID mbguid))
        throw new MBTypeMismatchException("CompareTo function called with an invalid argument!");
      return (int) this._internalValue == (int) mbguid._internalValue;
    }

    public bool Equals(MBGUID other) => (int) this._internalValue == (int) other._internalValue;
  }
}
