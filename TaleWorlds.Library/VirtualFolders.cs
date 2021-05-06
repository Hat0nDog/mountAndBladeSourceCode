// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.VirtualFolders
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.IO;
using System.Reflection;

namespace TaleWorlds.Library
{
  public class VirtualFolders
  {
    private static readonly bool _useVirtualFolders = true;

    public static string GetFileContent(string filePath) => !VirtualFolders._useVirtualFolders ? File.ReadAllText(filePath) : VirtualFolders.GetVirtualFileContent(filePath);

    private static string GetVirtualFileContent(string filePath)
    {
      string fileName = Path.GetFileName(filePath);
      string[] strArray = Path.GetDirectoryName(filePath).Split(Path.DirectorySeparatorChar);
      Type type = typeof (VirtualFolders);
      for (int index = 0; type != (Type) null && index != strArray.Length; ++index)
      {
        if (!strArray[index].IsStringNoneOrEmpty())
          type = VirtualFolders.GetNestedDirectory(strArray[index], type);
      }
      if (type != (Type) null)
      {
        foreach (MemberInfo field in type.GetFields())
        {
          VirtualFileAttribute[] customAttributes = (VirtualFileAttribute[]) field.GetCustomAttributes(typeof (VirtualFileAttribute), false);
          if (customAttributes[0].Name == fileName)
            return customAttributes[0].Content;
        }
      }
      return "";
    }

    private static Type GetNestedDirectory(string name, Type type)
    {
      foreach (Type nestedType in type.GetNestedTypes())
      {
        if (((VirtualDirectoryAttribute[]) nestedType.GetCustomAttributes(typeof (VirtualDirectoryAttribute), false))[0].Name == name)
          return nestedType;
      }
      return (Type) null;
    }

    [VirtualDirectory("..")]
    public class Win64_Shipping_Client
    {
      [VirtualDirectory("..")]
      public class bin
      {
        [VirtualDirectory("Parameters")]
        public class Parameters
        {
          [VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"e1.5.9\"/>\t<Multiplayer Value=\"m1.5.9\"/></Version>")]
          public string Version;
          [VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Odd\"/>")]
          public string ClientProfile;

          [VirtualDirectory("ClientProfiles")]
          public class ClientProfiles
          {
            [VirtualDirectory("DigitalOcean.Odd")]
            public class DigitalOceanOdd
            {
              [VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"bannerlord-odd-lobby.bannerlord-services-2.net\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"80\" />\t</Parameters></Configuration>")]
              public string LobbyClient;
            }
          }
        }
      }
    }
  }
}
