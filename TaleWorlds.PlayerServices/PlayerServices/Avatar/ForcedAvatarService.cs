// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.ForcedAvatarService
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
  internal class ForcedAvatarService : IAvatarService
  {
    private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerForcedAvatars/";
    private readonly List<byte[]> _avatarImagesAsByteArrays;

    public ForcedAvatarService()
    {
      this._avatarImagesAsByteArrays = new List<byte[]>();
      foreach (string file in Directory.GetFiles(this._resourceFolder, "*.png"))
        this._avatarImagesAsByteArrays.Add(File.ReadAllBytes(file));
    }

    public AvatarData GetPlayerAvatar(PlayerId playerId) => (AvatarData) null;

    public AvatarData GetForcedPlayerAvatar(int forcedIndex) => new AvatarData(this._avatarImagesAsByteArrays[forcedIndex]);
  }
}
