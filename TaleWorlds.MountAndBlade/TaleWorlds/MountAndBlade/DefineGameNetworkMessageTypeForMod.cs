// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DefineGameNetworkMessageTypeForMod
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class DefineGameNetworkMessageTypeForMod : Attribute
  {
    public readonly GameNetworkMessageSendType SendType;

    public DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType sendType) => this.SendType = sendType;
  }
}
