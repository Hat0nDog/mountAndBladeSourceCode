// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.ServiceException
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  public class ServiceException : Exception
  {
    public ServiceException(string type, string message)
      : base(nameof (ServiceException))
    {
      this.ExceptionType = type;
      this.ExceptionMessage = message;
    }

    public string ExceptionMessage { get; set; }

    public string ExceptionType { get; set; }
  }
}
