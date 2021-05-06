// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBUnusedResourceManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class MBUnusedResourceManager
  {
    public static void SetMeshUsed(string meshName) => MBAPI.IMBWorld.SetMeshUsed(meshName);

    public static void SetMaterialUsed(string meshName) => MBAPI.IMBWorld.SetMaterialUsed(meshName);

    public static void SetBodyUsed(string bodyName) => MBAPI.IMBWorld.SetBodyUsed(bodyName);
  }
}
