// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.InquiryElement
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class InquiryElement
  {
    public readonly string Title;
    public readonly ImageIdentifier ImageIdentifier;
    public readonly object Identifier;
    public readonly bool IsEnabled;
    public readonly string Hint;

    public InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier)
    {
      this.Identifier = identifier;
      this.Title = title;
      this.ImageIdentifier = imageIdentifier;
      this.IsEnabled = true;
      this.Hint = (string) null;
    }

    public InquiryElement(
      object identifier,
      string title,
      ImageIdentifier imageIdentifier,
      bool isEnabled,
      string hint)
    {
      this.Identifier = identifier;
      this.Title = title;
      this.ImageIdentifier = imageIdentifier;
      this.IsEnabled = isEnabled;
      this.Hint = hint;
    }
  }
}
