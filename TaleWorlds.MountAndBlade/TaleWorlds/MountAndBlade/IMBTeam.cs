// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBTeam
  {
    [EngineMethod("is_enemy", false)]
    bool IsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex);

    [EngineMethod("set_is_enemy", false)]
    void SetIsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex, bool isEnemy);
  }
}
