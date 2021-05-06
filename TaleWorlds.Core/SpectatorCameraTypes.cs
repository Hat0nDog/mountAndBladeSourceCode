// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.SpectatorCameraTypes
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public enum SpectatorCameraTypes
  {
    Invalid = -1, // 0xFFFFFFFF
    Free = 0,
    LockToMainPlayer = 1,
    LockToAnyAgent = 2,
    LockToAnyPlayer = 3,
    LockToPlayerFormation = 4,
    LockToTeamMembers = 5,
    LockToTeamMembersView = 6,
    LockToPosition = 7,
    Count = 8,
  }
}
