// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBWorkspace`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public class MBWorkspace<T> where T : IMBCollection, new()
  {
    private bool _isBeingUsed;
    private T _workspace;

    public T StartUsingWorkspace()
    {
      this._isBeingUsed = true;
      if ((object) this._workspace == null)
        this._workspace = new T();
      return this._workspace;
    }

    public void StopUsingWorkspace()
    {
      this._isBeingUsed = false;
      this._workspace.Clear();
    }

    public T GetWorkspace() => this._workspace;
  }
}
