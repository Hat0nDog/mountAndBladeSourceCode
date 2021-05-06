// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.FolderId
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;

namespace TaleWorlds.SaveSystem
{
  public struct FolderId : IEquatable<FolderId>
  {
    public int LocalId { get; private set; }

    public SaveFolderExtension Extension { get; private set; }

    public FolderId(int localId, SaveFolderExtension extension)
    {
      this.LocalId = localId;
      this.Extension = extension;
    }

    public override bool Equals(object obj) => obj is FolderId folderId && folderId.LocalId == this.LocalId && folderId.Extension == this.Extension;

    public bool Equals(FolderId other) => other.LocalId == this.LocalId && other.Extension == this.Extension;

    public override int GetHashCode()
    {
      int num1 = this.LocalId;
      int num2 = num1.GetHashCode() * 397;
      num1 = (int) this.Extension;
      int hashCode = num1.GetHashCode();
      return num2 ^ hashCode;
    }

    public static bool operator ==(FolderId a, FolderId b) => a.LocalId == b.LocalId && a.Extension == b.Extension;

    public static bool operator !=(FolderId a, FolderId b) => !(a == b);
  }
}
