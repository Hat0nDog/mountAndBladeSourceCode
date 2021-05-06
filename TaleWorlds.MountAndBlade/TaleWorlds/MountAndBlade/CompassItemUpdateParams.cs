// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CompassItemUpdateParams
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public struct CompassItemUpdateParams
  {
    public readonly object Item;
    public readonly TargetIconType TargetType;
    public readonly Vec3 WorldPosition;
    public readonly uint Color;
    public readonly uint Color2;
    public readonly BannerCode BannerCode;
    public readonly bool IsAttacker;
    public readonly bool IsAlly;

    public CompassItemUpdateParams(
      object item,
      TargetIconType targetType,
      Vec3 worldPosition,
      uint color,
      uint color2)
      : this()
    {
      this.Item = item;
      this.TargetType = targetType;
      this.WorldPosition = worldPosition;
      this.Color = color;
      this.Color2 = color2;
      this.IsAttacker = false;
      this.IsAlly = false;
    }

    public CompassItemUpdateParams(
      object item,
      TargetIconType targetType,
      Vec3 worldPosition,
      BannerCode bannerCode,
      bool isAttacker,
      bool isAlly)
      : this()
    {
      this.Item = item;
      this.TargetType = targetType;
      this.WorldPosition = worldPosition;
      this.BannerCode = bannerCode;
      this.IsAttacker = isAttacker;
      this.IsAlly = isAlly;
    }
  }
}
