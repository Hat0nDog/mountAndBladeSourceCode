// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DeformKeyData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Deform_Key_Data")]
  public struct DeformKeyData
  {
    public int GroupId;
    public int KeyTimePoint;
    public float Min;
    public float Max;
    public float Value;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
    public string Id;
  }
}
