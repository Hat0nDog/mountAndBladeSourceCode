// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBException
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Core
{
  public class MBException : ApplicationException
  {
    public MBException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public MBException(string message)
      : base(message)
    {
    }

    public MBException()
    {
    }

    public MBException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
