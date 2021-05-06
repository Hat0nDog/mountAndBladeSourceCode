// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.CallbackStringBufferManager
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;

namespace TaleWorlds.DotNet
{
  public static class CallbackStringBufferManager
  {
    internal const int CallbackStringBufferMaxSize = 32768;
    [ThreadStatic]
    private static byte[] _stringBuffer0;
    [ThreadStatic]
    private static byte[] _stringBuffer1;
    [ThreadStatic]
    private static byte[] _stringBuffer2;

    public static byte[] StringBuffer0
    {
      get
      {
        if (CallbackStringBufferManager._stringBuffer0 == null)
          CallbackStringBufferManager._stringBuffer0 = new byte[32768];
        return CallbackStringBufferManager._stringBuffer0;
      }
    }

    public static byte[] StringBuffer1
    {
      get
      {
        if (CallbackStringBufferManager._stringBuffer1 == null)
          CallbackStringBufferManager._stringBuffer1 = new byte[32768];
        return CallbackStringBufferManager._stringBuffer1;
      }
    }

    public static byte[] StringBuffer2
    {
      get
      {
        if (CallbackStringBufferManager._stringBuffer2 == null)
          CallbackStringBufferManager._stringBuffer2 = new byte[32768];
        return CallbackStringBufferManager._stringBuffer2;
      }
    }
  }
}
