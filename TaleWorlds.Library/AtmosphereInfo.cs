// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.AtmosphereInfo
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Runtime.InteropServices;

namespace TaleWorlds.Library
{
  [StructLayout(LayoutKind.Sequential)]
  public class AtmosphereInfo
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string AtmosphereName;
    public SunInformation SunInfo;
    public RainInformation RainInfo;
    public SnowInformation SnowInfo;
    public AmbientInformation AmbientInfo;
    public FogInformation FogInfo;
    public SkyInformation SkyInfo;
    public TimeInformation TimeInfo;
    public AreaInformation AreaInfo;
    public PostProcessInformation PostProInfo;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string AtmosphereTypeName;

    public AtmosphereInfo() => this.AtmosphereName = "";

    public void DeserializeFrom(IReader reader)
    {
      this.SunInfo.DeserializeFrom(reader);
      this.RainInfo.DeserializeFrom(reader);
      this.SnowInfo.DeserializeFrom(reader);
      this.AmbientInfo.DeserializeFrom(reader);
      this.FogInfo.DeserializeFrom(reader);
      this.SkyInfo.DeserializeFrom(reader);
      this.TimeInfo.DeserializeFrom(reader);
      this.AreaInfo.DeserializeFrom(reader);
      this.PostProInfo.DeserializeFrom(reader);
    }

    public void SerializeTo(IWriter writer)
    {
      this.SunInfo.SerializeTo(writer);
      this.RainInfo.SerializeTo(writer);
      this.SnowInfo.SerializeTo(writer);
      this.AmbientInfo.SerializeTo(writer);
      this.FogInfo.SerializeTo(writer);
      this.SkyInfo.SerializeTo(writer);
      this.TimeInfo.SerializeTo(writer);
      this.AreaInfo.SerializeTo(writer);
      this.PostProInfo.SerializeTo(writer);
    }
  }
}
