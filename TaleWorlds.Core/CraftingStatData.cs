// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CraftingStatData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
  public struct CraftingStatData
  {
    public readonly TextObject DescriptionText;
    public readonly float CurValue;
    public readonly float MaxValue;
    public readonly CraftingTemplate.CraftingStatTypes Type;

    public bool IsValid => (double) this.MaxValue >= 0.0;

    public CraftingStatData(
      TextObject descriptionText,
      float curValue,
      float maxValue,
      CraftingTemplate.CraftingStatTypes type)
    {
      this.DescriptionText = descriptionText;
      this.CurValue = curValue;
      this.MaxValue = maxValue;
      this.Type = type;
    }
  }
}
