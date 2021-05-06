// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.TestAvatarService
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices.Avatar
{
  public class TestAvatarService : IAvatarService
  {
    private readonly Dictionary<ulong, AvatarData> _avatarImageCache;
    private readonly string _resourceFolder = BasePath.Name + "Modules/Native/MultiplayerTestAvatars/";
    private readonly List<byte[]> _avatarImagesAsByteArrays;

    public TestAvatarService()
    {
      this._avatarImageCache = new Dictionary<ulong, AvatarData>();
      this._avatarImagesAsByteArrays = new List<byte[]>();
      foreach (string file in Directory.GetFiles(this._resourceFolder, "*.jpg"))
        this._avatarImagesAsByteArrays.Add(File.ReadAllBytes(file));
    }

    public AvatarData GetPlayerAvatar(PlayerId playerId) => new AvatarData(this._avatarImagesAsByteArrays[(int) ((uint) playerId.Id2 % (uint) this._avatarImagesAsByteArrays.Count)]);
  }
}
