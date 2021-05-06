// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.ICallbackManager
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
  public interface ICallbackManager
  {
    void Initialize();

    Delegate[] GetDelegates();

    Dictionary<string, object> GetScriptingInterfaceObjects();

    void SetFunctionPointer(int id, IntPtr pointer);

    void CheckSharedStructureSizes();
  }
}
