// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.XmlHelper
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Globalization;
using System.Xml;

namespace TaleWorlds.Core
{
  public static class XmlHelper
  {
    public static int ReadInt(XmlNode node, string str)
    {
      XmlAttribute attribute = node.Attributes[str];
      return attribute == null ? 0 : int.Parse(attribute.Value);
    }

    public static void ReadInt(ref int val, XmlNode node, string str)
    {
      XmlAttribute attribute = node.Attributes[str];
      if (attribute == null)
        return;
      val = int.Parse(attribute.Value);
    }

    public static float ReadFloat(XmlNode node, string str, float defaultValue = 0.0f)
    {
      XmlAttribute attribute = node.Attributes[str];
      return attribute == null ? defaultValue : float.Parse(attribute.Value);
    }

    public static string ReadString(XmlNode node, string str)
    {
      XmlAttribute attribute = node.Attributes[str];
      return attribute == null ? "" : attribute.Value;
    }

    public static void ReadHexCode(ref uint val, XmlNode node, string str)
    {
      XmlAttribute attribute = node.Attributes[str];
      if (attribute == null)
        return;
      string s = attribute.Value.Substring(2);
      val = uint.Parse(s, NumberStyles.HexNumber);
    }

    public static bool ReadBool(XmlNode node, string str)
    {
      XmlAttribute attribute = node.Attributes[str];
      return attribute != null && Convert.ToBoolean(attribute.InnerText);
    }
  }
}
