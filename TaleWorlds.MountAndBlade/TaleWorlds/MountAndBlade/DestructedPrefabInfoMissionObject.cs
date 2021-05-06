// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DestructedPrefabInfoMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [Obsolete]
  public class DestructedPrefabInfoMissionObject : MissionObject
  {
    public string DestructedPrefabName;
    public Vec3 Translate = new Vec3();
    public Vec3 Rotation = new Vec3();
    public Vec3 Scale = new Vec3(1f, 1f, 1f);
  }
}
