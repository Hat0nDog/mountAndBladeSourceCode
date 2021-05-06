// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MessageManagerBase
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  public abstract class MessageManagerBase : DotNetObject
  {
    [EngineCallback]
    protected internal abstract void PostWarningLine(string text);

    [EngineCallback]
    protected internal abstract void PostSuccessLine(string text);

    [EngineCallback]
    protected internal abstract void PostMessageLineFormatted(string text, uint color);

    [EngineCallback]
    protected internal abstract void PostMessageLine(string text, uint color);
  }
}
