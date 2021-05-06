﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.StaticBodyProperties
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableStruct(20670)]
  [Serializable]
  public struct StaticBodyProperties : ISerializableObject
  {
    public const int WeightKeyNo = 59;
    public const int BuildKeyNo = 60;

    public static void AutoGeneratedStaticCollectObjectsStaticBodyProperties(
      object o,
      List<object> collectedObjects)
    {
      ((StaticBodyProperties) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }

    internal static object AutoGeneratedGetMemberValueKeyPart1(object o) => (object) ((StaticBodyProperties) o).KeyPart1;

    internal static object AutoGeneratedGetMemberValueKeyPart2(object o) => (object) ((StaticBodyProperties) o).KeyPart2;

    internal static object AutoGeneratedGetMemberValueKeyPart3(object o) => (object) ((StaticBodyProperties) o).KeyPart3;

    internal static object AutoGeneratedGetMemberValueKeyPart4(object o) => (object) ((StaticBodyProperties) o).KeyPart4;

    internal static object AutoGeneratedGetMemberValueKeyPart5(object o) => (object) ((StaticBodyProperties) o).KeyPart5;

    internal static object AutoGeneratedGetMemberValueKeyPart6(object o) => (object) ((StaticBodyProperties) o).KeyPart6;

    internal static object AutoGeneratedGetMemberValueKeyPart7(object o) => (object) ((StaticBodyProperties) o).KeyPart7;

    internal static object AutoGeneratedGetMemberValueKeyPart8(object o) => (object) ((StaticBodyProperties) o).KeyPart8;

    [SaveableProperty(1)]
    public ulong KeyPart1 { private set; get; }

    [SaveableProperty(2)]
    public ulong KeyPart2 { private set; get; }

    [SaveableProperty(3)]
    public ulong KeyPart3 { private set; get; }

    [SaveableProperty(4)]
    public ulong KeyPart4 { private set; get; }

    [SaveableProperty(5)]
    public ulong KeyPart5 { private set; get; }

    [SaveableProperty(6)]
    public ulong KeyPart6 { private set; get; }

    [SaveableProperty(7)]
    public ulong KeyPart7 { private set; get; }

    [SaveableProperty(8)]
    public ulong KeyPart8 { private set; get; }

    public StaticBodyProperties(
      ulong keyPart1,
      ulong keyPart2,
      ulong keyPart3,
      ulong keyPart4,
      ulong keyPart5,
      ulong keyPart6,
      ulong keyPart7,
      ulong keyPart8)
    {
      this.KeyPart1 = keyPart1;
      this.KeyPart2 = keyPart2;
      this.KeyPart3 = keyPart3;
      this.KeyPart4 = keyPart4;
      this.KeyPart5 = keyPart5;
      this.KeyPart6 = keyPart6;
      this.KeyPart7 = keyPart7;
      this.KeyPart8 = keyPart8;
    }

    public static bool FromXmlNode(XmlNode node, out StaticBodyProperties staticBodyProperties)
    {
      string str = node.Attributes["key"].Value;
      if (str.Length != 128)
      {
        staticBodyProperties = new StaticBodyProperties();
        return false;
      }
      ulong result1;
      ulong.TryParse(str.Substring(0, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result1);
      ulong result2;
      ulong.TryParse(str.Substring(16, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result2);
      ulong result3;
      ulong.TryParse(str.Substring(32, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result3);
      ulong result4;
      ulong.TryParse(str.Substring(48, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result4);
      ulong result5;
      ulong.TryParse(str.Substring(64, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result5);
      ulong result6;
      ulong.TryParse(str.Substring(80, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result6);
      ulong result7;
      ulong.TryParse(str.Substring(96, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result7);
      ulong result8;
      ulong.TryParse(str.Substring(112, 16), NumberStyles.AllowHexSpecifier, (IFormatProvider) CultureInfo.CurrentCulture, out result8);
      staticBodyProperties = new StaticBodyProperties(result1, result2, result3, result4, result5, result6, result7, result8);
      return true;
    }

    public override int GetHashCode()
    {
      int num1 = ((((((((((((((-2128831035 ^ (int) (uint) (this.KeyPart1 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart2 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart3 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart4 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart5 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart6 << 32)) * 16777619 ^ (int) (uint) (this.KeyPart7 << 32)) * 16777619 ^ (int) (uint) this.KeyPart1) * 16777619 ^ (int) (uint) this.KeyPart2) * 16777619 ^ (int) (uint) this.KeyPart3) * 16777619 ^ (int) (uint) this.KeyPart4) * 16777619 ^ (int) (uint) this.KeyPart5) * 16777619 ^ (int) (uint) this.KeyPart6) * 16777619 ^ (int) (uint) this.KeyPart7) * 16777619;
      int num2 = num1 + (num1 << 13);
      int num3 = num2 ^ (int) ((uint) num2 >> 7);
      int num4 = num3 + (num3 << 3);
      int num5 = num4 ^ (int) ((uint) num4 >> 17);
      return num5 + (num5 << 5);
    }

    public override bool Equals(object obj) => base.Equals(obj);

    public static bool operator ==(StaticBodyProperties a, StaticBodyProperties b)
    {
      if ((ValueType) a == (ValueType) b)
        return true;
      return (ValueType) a != null && (ValueType) b != null && ((long) a.KeyPart1 == (long) b.KeyPart1 && (long) a.KeyPart2 == (long) b.KeyPart2) && ((long) a.KeyPart3 == (long) b.KeyPart3 && (long) a.KeyPart4 == (long) b.KeyPart4 && ((long) a.KeyPart5 == (long) b.KeyPart5 && (long) a.KeyPart6 == (long) b.KeyPart6)) && (long) a.KeyPart7 == (long) b.KeyPart7 && (long) a.KeyPart8 == (long) b.KeyPart8;
    }

    public static bool operator !=(StaticBodyProperties a, StaticBodyProperties b) => !(a == b);

    public override string ToString()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(150, nameof (ToString));
      mbStringBuilder.Append<string>("key=\"");
      mbStringBuilder.Append<string>(this.KeyPart1.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart2.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart3.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart4.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart5.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart6.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart7.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>(this.KeyPart8.ToString("X").PadLeft(16, '0'));
      mbStringBuilder.Append<string>("\" ");
      return mbStringBuilder.ToStringAndRelease();
    }

    void ISerializableObject.DeserializeFrom(IReader reader)
    {
      this.KeyPart1 = reader.ReadULong();
      this.KeyPart2 = reader.ReadULong();
      this.KeyPart3 = reader.ReadULong();
      this.KeyPart4 = reader.ReadULong();
      this.KeyPart5 = reader.ReadULong();
      this.KeyPart6 = reader.ReadULong();
      this.KeyPart7 = reader.ReadULong();
      this.KeyPart8 = reader.ReadULong();
    }

    void ISerializableObject.SerializeTo(IWriter writer)
    {
      writer.WriteULong(this.KeyPart1);
      writer.WriteULong(this.KeyPart2);
      writer.WriteULong(this.KeyPart3);
      writer.WriteULong(this.KeyPart4);
      writer.WriteULong(this.KeyPart5);
      writer.WriteULong(this.KeyPart6);
      writer.WriteULong(this.KeyPart7);
      writer.WriteULong(this.KeyPart8);
    }

    public static StaticBodyProperties GetRandomStaticBodyProperties()
    {
      Random random = new Random((int) DateTime.Now.Ticks);
      StaticBodyProperties staticBodyProperties = new StaticBodyProperties();
      staticBodyProperties.KeyPart1 = (ulong) random.Next();
      staticBodyProperties.KeyPart1 <<= 32;
      staticBodyProperties.KeyPart1 += (ulong) random.Next();
      staticBodyProperties.KeyPart2 = (ulong) random.Next();
      staticBodyProperties.KeyPart2 <<= 32;
      staticBodyProperties.KeyPart2 += (ulong) random.Next();
      staticBodyProperties.KeyPart3 = (ulong) random.Next();
      staticBodyProperties.KeyPart3 <<= 32;
      staticBodyProperties.KeyPart3 += (ulong) random.Next();
      staticBodyProperties.KeyPart4 = (ulong) random.Next();
      staticBodyProperties.KeyPart4 <<= 32;
      staticBodyProperties.KeyPart4 += (ulong) random.Next();
      staticBodyProperties.KeyPart5 = (ulong) random.Next();
      staticBodyProperties.KeyPart5 <<= 32;
      staticBodyProperties.KeyPart5 += (ulong) random.Next();
      staticBodyProperties.KeyPart6 = (ulong) random.Next();
      staticBodyProperties.KeyPart6 <<= 32;
      staticBodyProperties.KeyPart6 += (ulong) random.Next();
      staticBodyProperties.KeyPart7 = (ulong) random.Next();
      staticBodyProperties.KeyPart7 <<= 32;
      staticBodyProperties.KeyPart7 += (ulong) random.Next();
      staticBodyProperties.KeyPart8 = (ulong) random.Next();
      staticBodyProperties.KeyPart8 <<= 32;
      staticBodyProperties.KeyPart8 += (ulong) random.Next();
      return staticBodyProperties;
    }
  }
}
