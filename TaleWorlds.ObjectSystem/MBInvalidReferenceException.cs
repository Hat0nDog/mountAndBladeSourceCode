﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.MBInvalidReferenceException
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

namespace TaleWorlds.ObjectSystem
{
  public class MBInvalidReferenceException : ObjectSystemException
  {
    internal MBInvalidReferenceException(string exceptionString)
      : base("Reference structure is not valid. " + exceptionString)
    {
    }
  }
}
