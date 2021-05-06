// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ApplicationVersion
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Xml;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct ApplicationVersion
  {
    public static readonly ApplicationVersion Empty = new ApplicationVersion(ApplicationVersionType.Invalid, -1, -1, -1, -1, ApplicationVersionGameType.Singleplayer);
    public const int DefaultChangeSet = 268540;

    public ApplicationVersionType ApplicationVersionType { get; private set; }

    public int Major { get; private set; }

    public int Minor { get; private set; }

    public int Revision { get; private set; }

    public int ChangeSet { get; private set; }

    public ApplicationVersionGameType VersionGameType { get; private set; }

    public ApplicationVersion(
      ApplicationVersionType applicationVersionType,
      int major,
      int minor,
      int revision,
      int changeSet,
      ApplicationVersionGameType versionGameType)
    {
      this.ApplicationVersionType = applicationVersionType;
      this.Major = major;
      this.Minor = minor;
      this.Revision = revision;
      this.ChangeSet = changeSet;
      this.VersionGameType = versionGameType;
    }

    public static ApplicationVersion FromParametersFile(
      ApplicationVersionGameType versionGameType)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(VirtualFolders.GetFileContent(BasePath.Name + "Parameters/Version.xml"));
      string versionAsString = "";
      switch (versionGameType)
      {
        case ApplicationVersionGameType.Singleplayer:
          versionAsString = xmlDocument.ChildNodes[0].ChildNodes[0].Attributes["Value"].InnerText;
          break;
        case ApplicationVersionGameType.Multiplayer:
          versionAsString = xmlDocument.ChildNodes[0].ChildNodes[1].Attributes["Value"].InnerText;
          break;
      }
      return ApplicationVersion.FromString(versionAsString, versionGameType);
    }

    public static ApplicationVersion FromString(
      string versionAsString,
      ApplicationVersionGameType versionGameType)
    {
      string[] strArray = versionAsString.Split('.');
      int num1 = strArray.Length == 3 || strArray.Length == 4 ? (int) ApplicationVersion.ApplicationVersionTypeFromString(strArray[0][0].ToString()) : throw new Exception("Wrong version as string");
      string str1 = strArray[0].Substring(1);
      string str2 = strArray[1];
      string str3 = strArray[2];
      int int32_1 = Convert.ToInt32(str1);
      int int32_2 = Convert.ToInt32(str2);
      int int32_3 = Convert.ToInt32(str3);
      int num2 = strArray.Length > 3 ? Convert.ToInt32(strArray[3]) : 268540;
      int major = int32_1;
      int minor = int32_2;
      int revision = int32_3;
      int changeSet = num2;
      int num3 = (int) versionGameType;
      return new ApplicationVersion((ApplicationVersionType) num1, major, minor, revision, changeSet, (ApplicationVersionGameType) num3);
    }

    public bool IsSame(ApplicationVersion other) => this.ApplicationVersionType == other.ApplicationVersionType && this.Major == other.Major && (this.Minor == other.Minor && this.Revision == other.Revision) && this.VersionGameType == other.VersionGameType;

    public static ApplicationVersionType ApplicationVersionTypeFromString(
      string applicationVersionTypeAsString)
    {
      return applicationVersionTypeAsString == "a" ? ApplicationVersionType.Alpha : (applicationVersionTypeAsString == "b" ? ApplicationVersionType.Beta : (applicationVersionTypeAsString == "e" ? ApplicationVersionType.EarlyAccess : (applicationVersionTypeAsString == "v" ? ApplicationVersionType.Release : (applicationVersionTypeAsString == "d" ? ApplicationVersionType.Development : (applicationVersionTypeAsString == "m" ? ApplicationVersionType.Multiplayer : ApplicationVersionType.Invalid)))));
    }

    public static string GetPrefix(ApplicationVersionType applicationVersionType)
    {
      string str;
      switch (applicationVersionType)
      {
        case ApplicationVersionType.Alpha:
          str = "a";
          break;
        case ApplicationVersionType.Beta:
          str = "b";
          break;
        case ApplicationVersionType.EarlyAccess:
          str = "e";
          break;
        case ApplicationVersionType.Release:
          str = "v";
          break;
        case ApplicationVersionType.Development:
          str = "d";
          break;
        case ApplicationVersionType.Multiplayer:
          str = "m";
          break;
        default:
          str = "i";
          break;
      }
      return str;
    }

    public override string ToString() => ApplicationVersion.GetPrefix(this.ApplicationVersionType) + (object) this.Major + "." + (object) this.Minor + "." + (object) this.Revision + "." + (object) this.ChangeSet;

    public static bool operator ==(ApplicationVersion a, ApplicationVersion b) => a.Major == b.Major && a.Minor == b.Minor && (a.Revision == b.Revision && a.ApplicationVersionType == b.ApplicationVersionType) && a.VersionGameType == b.VersionGameType;

    public static bool operator !=(ApplicationVersion a, ApplicationVersion b) => !(a == b);

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && (ApplicationVersion) obj == this;

    public static bool operator >(ApplicationVersion a, ApplicationVersion b) => a.ApplicationVersionType > b.ApplicationVersionType || a.ApplicationVersionType == b.ApplicationVersionType && (a.Major > b.Major || a.Major == b.Major && (a.Minor > b.Minor || a.Minor == b.Minor && a.ChangeSet > b.ChangeSet));

    public static bool operator <(ApplicationVersion a, ApplicationVersion b) => !(a == b) && !(a > b);

    public static bool operator >=(ApplicationVersion a, ApplicationVersion b) => a == b || a > b;

    public static bool operator <=(ApplicationVersion a, ApplicationVersion b) => a == b || a < b;
  }
}
