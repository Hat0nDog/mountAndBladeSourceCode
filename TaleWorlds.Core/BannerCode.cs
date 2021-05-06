// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BannerCode
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class BannerCode
  {
    public string Code { get; private set; }

    public Banner CalculateBanner() => new Banner(this.Code);

    public static BannerCode CreateFrom(Banner banner)
    {
      BannerCode bannerCode = new BannerCode();
      if (banner != null)
        bannerCode.Code = banner.Serialize();
      return bannerCode;
    }

    public static BannerCode CreateFrom(string bannerCodeCode) => new BannerCode()
    {
      Code = bannerCodeCode
    };

    public override int GetHashCode() => this.Code.GetHashCode();

    public override bool Equals(object obj) => obj != null && (object) (obj as BannerCode) != null && !(((BannerCode) obj).Code != this.Code);

    public static bool operator ==(BannerCode a, BannerCode b)
    {
      if ((object) a == (object) b)
        return true;
      return (object) a != null && (object) b != null && a.Equals((object) b);
    }

    public static bool operator !=(BannerCode a, BannerCode b) => !(a == b);
  }
}
