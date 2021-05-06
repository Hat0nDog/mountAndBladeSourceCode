// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.InformationMessage
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class InformationMessage
  {
    public string Information { get; set; }

    public string Detail { get; set; }

    public Color Color { get; set; }

    public string SoundEventPath { get; set; }

    public string Category { get; set; }

    public InformationMessage(string information)
    {
      this.Information = information;
      this.Color = Color.White;
    }

    public InformationMessage(string information, Color color)
    {
      this.Information = information;
      this.Color = color;
    }

    public InformationMessage(string information, Color color, string category)
    {
      this.Information = information;
      this.Color = color;
      this.Category = category;
    }

    public InformationMessage(string information, string soundEventPath)
    {
      this.Information = information;
      this.SoundEventPath = soundEventPath;
      this.Color = Color.White;
    }

    public InformationMessage() => this.Information = "";
  }
}
