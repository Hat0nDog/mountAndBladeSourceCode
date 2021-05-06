// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.TWException
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Library
{
  public class TWException : ApplicationException
  {
    public TWException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public TWException(string message)
      : base(message)
    {
    }

    public TWException()
    {
    }

    public TWException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
