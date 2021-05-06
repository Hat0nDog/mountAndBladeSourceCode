// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MachineId
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace TaleWorlds.Library
{
  public static class MachineId
  {
    private static string MachineIdString;

    public static void Initialize()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        MachineId.MachineIdString = "nonwindows";
      else
        MachineId.MachineIdString = MachineId.ProcessId();
    }

    public static int AsInteger() => !MachineId.MachineIdString.IsStringNoneOrEmpty() ? BitConverter.ToInt32(Encoding.ASCII.GetBytes(MachineId.MachineIdString), 0) : 0;

    private static string ProcessId() => "" + MachineId.GetMotherboardIdentifier() + MachineId.GetCpuIdentifier() + MachineId.GetDiskIdentifier();

    private static string GetMotherboardIdentifier()
    {
      string str1 = "";
      try
      {
        using (ManagementObjectCollection instances = new ManagementClass("win32_baseboard").GetInstances())
        {
          foreach (ManagementBaseObject managementBaseObject in instances)
          {
            string str2 = (managementBaseObject["SerialNumber"] as string).Trim(' ');
            str1 += str2.Replace("-", "");
          }
        }
      }
      catch (Exception ex)
      {
        return "";
      }
      return str1;
    }

    private static string GetCpuIdentifier()
    {
      string str1 = "";
      try
      {
        using (ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances())
        {
          foreach (ManagementBaseObject managementBaseObject in instances)
          {
            string str2 = (managementBaseObject["ProcessorId"] as string).Trim(' ');
            str1 += str2.Replace("-", "");
          }
        }
      }
      catch (Exception ex)
      {
        return "";
      }
      return str1;
    }

    private static string GetDiskIdentifier()
    {
      string str1 = "";
      try
      {
        using (ManagementObjectCollection instances = new ManagementClass("win32_diskdrive").GetInstances())
        {
          foreach (ManagementObject managementObject in instances)
          {
            if (string.Compare(managementObject["InterfaceType"] as string, "IDE", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
              string str2 = (managementObject["SerialNumber"] as string).Trim(' ');
              str1 += str2.Replace("-", "");
            }
          }
        }
      }
      catch (Exception ex)
      {
        return "";
      }
      return str1;
    }
  }
}
