// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CompassMarker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class CompassMarker
  {
    public string Id { get; private set; }

    public float Angle { get; private set; }

    public bool IsPrimary { get; private set; }

    public CompassMarker(string id, float angle, bool isPrimary)
    {
      this.Id = id;
      this.Angle = angle % 360f;
      this.IsPrimary = isPrimary;
    }
  }
}
