// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Shader
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  public sealed class Shader : Resource
  {
    internal Shader(UIntPtr ptr)
      : base(ptr)
    {
    }

    public static Shader GetFromResource(string shaderName) => EngineApplicationInterface.IShader.GetFromResource(shaderName);

    public string Name => EngineApplicationInterface.IShader.GetName(this.Pointer);

    public ulong GetMaterialShaderFlagMask(string flagName, bool showErrors = true) => EngineApplicationInterface.IShader.GetMaterialShaderFlagMask(this.Pointer, flagName, showErrors);
  }
}
