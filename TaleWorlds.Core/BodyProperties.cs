// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BodyProperties
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  [Serializable]
  public struct BodyProperties
  {
    private readonly DynamicBodyProperties _dynamicBodyProperties;
    private readonly StaticBodyProperties _staticBodyProperties;

    public StaticBodyProperties StaticProperties => this._staticBodyProperties;

    public DynamicBodyProperties DynamicProperties => this._dynamicBodyProperties;

    public float Age => this._dynamicBodyProperties.Age;

    public float Weight => this._dynamicBodyProperties.Weight;

    public float Build => this._dynamicBodyProperties.Build;

    public ulong KeyPart1 => this._staticBodyProperties.KeyPart1;

    public ulong KeyPart2 => this._staticBodyProperties.KeyPart2;

    public ulong KeyPart3 => this._staticBodyProperties.KeyPart3;

    public ulong KeyPart4 => this._staticBodyProperties.KeyPart4;

    public ulong KeyPart5 => this._staticBodyProperties.KeyPart5;

    public ulong KeyPart6 => this._staticBodyProperties.KeyPart6;

    public ulong KeyPart7 => this._staticBodyProperties.KeyPart7;

    public ulong KeyPart8 => this._staticBodyProperties.KeyPart8;

    public BodyProperties(
      DynamicBodyProperties dynamicBodyProperties,
      StaticBodyProperties staticBodyProperties)
    {
      this._dynamicBodyProperties = dynamicBodyProperties;
      this._staticBodyProperties = staticBodyProperties;
    }

    public static bool FromXmlNode(XmlNode node, out BodyProperties bodyProperties)
    {
      XmlAttributeCollection attributes = node.Attributes;
      float result1;
      float result2;
      float result3;
      DynamicBodyProperties dynamicBodyProperties = (attributes != null ? (attributes.Count == 5 ? 1 : 0) : 0) == 0 || node.Attributes["age"].Value == null || (node.Attributes["weight"].Value == null || node.Attributes["build"].Value == null) ? new DynamicBodyProperties() : (!float.TryParse(node.Attributes["age"].Value, out result1) || !float.TryParse(node.Attributes["weight"].Value, out result2) || !float.TryParse(node.Attributes["build"].Value, out result3) ? new DynamicBodyProperties() : new DynamicBodyProperties(result1, result2, result3));
      StaticBodyProperties staticBodyProperties;
      if (StaticBodyProperties.FromXmlNode(node, out staticBodyProperties))
      {
        bodyProperties = new BodyProperties(dynamicBodyProperties, staticBodyProperties);
        return true;
      }
      bodyProperties = new BodyProperties();
      return false;
    }

    public static bool FromString(string keyValue, out BodyProperties bodyProperties)
    {
      if (keyValue.StartsWith("<BodyProperties ", StringComparison.InvariantCultureIgnoreCase) || keyValue.StartsWith("<BodyPropertiesMax ", StringComparison.InvariantCultureIgnoreCase))
      {
        XmlDocument xmlDocument = new XmlDocument();
        try
        {
          xmlDocument.LoadXml(keyValue);
        }
        catch (XmlException ex)
        {
          bodyProperties = new BodyProperties();
          return false;
        }
        if (xmlDocument.FirstChild.Name.Equals(nameof (BodyProperties), StringComparison.InvariantCultureIgnoreCase) || xmlDocument.FirstChild.Name.Equals("BodyPropertiesMax", StringComparison.InvariantCultureIgnoreCase))
        {
          BodyProperties.FromXmlNode(xmlDocument.FirstChild, out bodyProperties);
          float result1 = 20f;
          float result2 = 0.0f;
          float result3 = 0.0f;
          if (xmlDocument.FirstChild.Attributes["age"] != null)
            float.TryParse(xmlDocument.FirstChild.Attributes["age"].Value, out result1);
          if (xmlDocument.FirstChild.Attributes["weight"] != null)
            float.TryParse(xmlDocument.FirstChild.Attributes["weight"].Value, out result2);
          if (xmlDocument.FirstChild.Attributes["build"] != null)
            float.TryParse(xmlDocument.FirstChild.Attributes["build"].Value, out result3);
          bodyProperties = new BodyProperties(new DynamicBodyProperties(result1, result2, result3), bodyProperties.StaticProperties);
          return true;
        }
        bodyProperties = new BodyProperties();
        return false;
      }
      bodyProperties = new BodyProperties();
      return false;
    }

    public static BodyProperties GetRandomBodyProperties(
      bool isFemale,
      BodyProperties bodyPropertiesMin,
      BodyProperties bodyPropertiesMax,
      int hairCoverType,
      int seed,
      string hairTags,
      string beardTags,
      string tattooTags)
    {
      return FaceGen.GetRandomBodyProperties(isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tattooTags);
    }

    public static bool operator ==(BodyProperties a, BodyProperties b)
    {
      if ((ValueType) a == (ValueType) b)
        return true;
      return (ValueType) a != null && (ValueType) b != null && a._staticBodyProperties == b._staticBodyProperties && a._dynamicBodyProperties == b._dynamicBodyProperties;
    }

    public static bool operator !=(BodyProperties a, BodyProperties b) => !(a == b);

    public override string ToString()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(150, nameof (ToString));
      mbStringBuilder.Append<string>("<BodyProperties version=\"4\" ");
      mbStringBuilder.Append<string>(this._dynamicBodyProperties.ToString() + " ");
      mbStringBuilder.Append<string>(this._staticBodyProperties.ToString());
      mbStringBuilder.Append<string>(" />");
      return mbStringBuilder.ToStringAndRelease();
    }

    public override bool Equals(object obj) => obj is BodyProperties bodyProperties && EqualityComparer<DynamicBodyProperties>.Default.Equals(this._dynamicBodyProperties, bodyProperties._dynamicBodyProperties) && EqualityComparer<StaticBodyProperties>.Default.Equals(this._staticBodyProperties, bodyProperties._staticBodyProperties);

    public override int GetHashCode() => (2041866711 * -1521134295 + EqualityComparer<DynamicBodyProperties>.Default.GetHashCode(this._dynamicBodyProperties)) * -1521134295 + EqualityComparer<StaticBodyProperties>.Default.GetHashCode(this._staticBodyProperties);

    public BodyProperties ClampForMultiplayer()
    {
      DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(MathF.Clamp(this.DynamicProperties.Age, 20f, 128f), 0.5f, 0.5f);
      StaticBodyProperties staticBodyProperties1 = this.StaticProperties;
      StaticBodyProperties staticBodyProperties2 = this.ClampHeightMultiplierFaceKey(in staticBodyProperties1);
      return new BodyProperties(dynamicBodyProperties, staticBodyProperties2);
    }

    private StaticBodyProperties ClampHeightMultiplierFaceKey(
      in StaticBodyProperties staticBodyProperties)
    {
      ulong keyPart8_1 = staticBodyProperties.KeyPart8;
      float num1 = (float) BodyProperties.GetBitsValueFromKey(in keyPart8_1, 19, 6) / 63f;
      if ((double) num1 >= 0.25 && (double) num1 <= 0.75)
        return staticBodyProperties;
      int inewValue = (int) (0.5 * 63.0);
      ulong num2 = BodyProperties.SetBits(in keyPart8_1, 19, 6, inewValue);
      StaticBodyProperties staticBodyProperties1 = staticBodyProperties;
      long keyPart1 = (long) staticBodyProperties1.KeyPart1;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart2 = (long) staticBodyProperties1.KeyPart2;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart3 = (long) staticBodyProperties1.KeyPart3;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart4 = (long) staticBodyProperties1.KeyPart4;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart5 = (long) staticBodyProperties1.KeyPart5;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart6 = (long) staticBodyProperties1.KeyPart6;
      long num3 = (long) num2;
      staticBodyProperties1 = staticBodyProperties;
      long keyPart8_2 = (long) staticBodyProperties1.KeyPart8;
      return new StaticBodyProperties((ulong) keyPart1, (ulong) keyPart2, (ulong) keyPart3, (ulong) keyPart4, (ulong) keyPart5, (ulong) keyPart6, (ulong) num3, (ulong) keyPart8_2);
    }

    private static ulong SetBits(in ulong ipart7, int startBit, int numBits, int inewValue) => (ulong) ((long) ipart7 & ~((long) (ulong) Math.Pow(2.0, (double) numBits) - 1L << startBit) | (long) inewValue << startBit);

    private static int GetBitsValueFromKey(in ulong part, int startBit, int numBits) => (int) ((long) (part >> startBit) & (long) ((ulong) Math.Pow(2.0, (double) numBits) - 1UL));

    public static BodyProperties Default => new BodyProperties(new DynamicBodyProperties(20f, 0.0f, 0.0f), new StaticBodyProperties());

    public static void AutoGeneratedStaticCollectObjectsBodyProperties(
      object o,
      List<object> collectedObjects)
    {
      ((BodyProperties) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }
  }
}
