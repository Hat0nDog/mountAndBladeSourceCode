// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.CustomParameter`1
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

namespace TaleWorlds.DotNet
{
  internal class CustomParameter<T> : DotNetObject
  {
    public T Target { get; set; }

    public CustomParameter(T target) => this.Target = target;
  }
}
