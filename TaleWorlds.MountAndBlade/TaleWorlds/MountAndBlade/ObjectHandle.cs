// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ObjectHandle
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct ObjectHandle
  {
    internal int Index;
    public static readonly ObjectHandle InvalidHandle = new ObjectHandle(-1);

    internal ObjectHandle(int i) => this.Index = i;

    public bool Equals(ObjectHandle a) => this.Index == a.Index;

    public override int GetHashCode() => this.Index;

    public override string ToString() => this.Index.ToString();
  }
}
