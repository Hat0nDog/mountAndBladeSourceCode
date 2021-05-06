// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextProcessor.CaseInsensitiveComparer
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Localization.TextProcessor
{
  internal class CaseInsensitiveComparer : IEqualityComparer<string>
  {
    public bool Equals(string x, string y) => x.Equals(y, StringComparison.InvariantCultureIgnoreCase);

    public int GetHashCode(string x) => x.ToLowerInvariant().GetHashCode();
  }
}
