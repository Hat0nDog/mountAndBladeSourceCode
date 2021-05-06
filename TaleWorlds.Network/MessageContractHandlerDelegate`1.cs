// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageContractHandlerDelegate`1
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

namespace TaleWorlds.Network
{
  public delegate void MessageContractHandlerDelegate<T>(T message) where T : MessageContract;
}
