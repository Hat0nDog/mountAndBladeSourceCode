// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BannerColor
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public struct BannerColor
  {
    public uint Color { get; private set; }

    public bool PlayerCanChooseForSigil { get; private set; }

    public bool PlayerCanChooseForBackground { get; private set; }

    public BannerColor(uint color, bool playerCanChooseForSigil, bool playerCanChooseForBackground)
    {
      this.Color = color;
      this.PlayerCanChooseForSigil = playerCanChooseForSigil;
      this.PlayerCanChooseForBackground = playerCanChooseForBackground;
    }
  }
}
