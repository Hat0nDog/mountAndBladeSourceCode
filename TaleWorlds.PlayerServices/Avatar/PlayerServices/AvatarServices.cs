// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Avatar.PlayerServices.AvatarServices
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System.Collections.Generic;
using TaleWorlds.PlayerServices;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.Avatar.PlayerServices
{
  public static class AvatarServices
  {
    private static Dictionary<PlayerIdProvidedTypes, IAvatarService> _allAvatarServices = new Dictionary<PlayerIdProvidedTypes, IAvatarService>();

    static AvatarServices()
    {
      AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Steam, (IAvatarService) new SteamAvatarService());
      AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Test, (IAvatarService) new TestAvatarService());
      AvatarServices.AddAvatarService(PlayerIdProvidedTypes.Forced, (IAvatarService) new ForcedAvatarService());
      AvatarServices.AddAvatarService(PlayerIdProvidedTypes.GOG, (IAvatarService) new GOGAvatarService());
    }

    public static AvatarData GetPlayerAvatar(PlayerId playerId, int forcedIndex)
    {
      AvatarData avatarData = (AvatarData) null;
      if (forcedIndex >= 0)
      {
        IAvatarService avatarService;
        if (AvatarServices._allAvatarServices.TryGetValue(PlayerIdProvidedTypes.Forced, out avatarService) && avatarService is ForcedAvatarService forcedAvatarService2)
          avatarData = forcedAvatarService2.GetForcedPlayerAvatar(forcedIndex);
      }
      else
      {
        IAvatarService avatarService;
        if (AvatarServices._allAvatarServices.TryGetValue(playerId.ProvidedType, out avatarService))
          avatarData = avatarService.GetPlayerAvatar(playerId);
      }
      return avatarData;
    }

    public static void AddAvatarService(PlayerIdProvidedTypes type, IAvatarService avatarService)
    {
      if (AvatarServices._allAvatarServices.ContainsKey(type))
        AvatarServices._allAvatarServices[type] = avatarService;
      else
        AvatarServices._allAvatarServices.Add(type, avatarService);
    }
  }
}
