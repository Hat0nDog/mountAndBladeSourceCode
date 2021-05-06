// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.IAvatarService
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

namespace TaleWorlds.PlayerServices.Avatar
{
  public interface IAvatarService
  {
    AvatarData GetPlayerAvatar(PlayerId playerId);
  }
}
