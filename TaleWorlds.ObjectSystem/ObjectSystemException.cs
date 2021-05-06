// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.ObjectSystemException
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System;

namespace TaleWorlds.ObjectSystem
{
  public class ObjectSystemException : Exception
  {
    internal ObjectSystemException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal ObjectSystemException(string message)
      : base(message)
    {
    }

    internal ObjectSystemException()
    {
    }
  }
}
