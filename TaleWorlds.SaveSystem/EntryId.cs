// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.EntryId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;

namespace TaleWorlds.SaveSystem
{
  public struct EntryId : IEquatable<EntryId>
  {
    public int Id { get; private set; }

    public SaveEntryExtension Extension { get; private set; }

    public EntryId(int id, SaveEntryExtension extension)
    {
      this.Id = id;
      this.Extension = extension;
    }

    public override bool Equals(object obj) => obj is EntryId entryId && entryId.Id == this.Id && entryId.Extension == this.Extension;

    public bool Equals(EntryId other) => other.Id == this.Id && other.Extension == this.Extension;

    public override int GetHashCode()
    {
      int num1 = this.Id;
      int num2 = num1.GetHashCode() * 397;
      num1 = (int) this.Extension;
      int hashCode = num1.GetHashCode();
      return num2 ^ hashCode;
    }

    public static bool operator ==(EntryId a, EntryId b) => a.Id == b.Id && a.Extension == b.Extension;

    public static bool operator !=(EntryId a, EntryId b) => !(a == b);
  }
}
