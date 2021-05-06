// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ImageIdentifier
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
  public class ImageIdentifier
  {
    public string Id;

    public ImageIdentifierType ImageTypeCode { get; private set; }

    public string AdditionalArgs { get; private set; }

    public ImageIdentifier(ImageIdentifierType imageType = ImageIdentifierType.Null)
    {
      this.ImageTypeCode = imageType;
      this.Id = "";
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(ItemObject itemObject)
    {
      this.ImageTypeCode = ImageIdentifierType.Item;
      this.Id = itemObject.StringId;
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(CharacterCode characterCode)
    {
      this.ImageTypeCode = ImageIdentifierType.Character;
      this.Id = characterCode.Code;
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(CraftingPiece craftingPiece, string pieceUsageId)
    {
      this.ImageTypeCode = ImageIdentifierType.CraftingPiece;
      this.Id = craftingPiece != null ? craftingPiece.StringId + "$" + pieceUsageId : "";
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(BannerCode bannerCode, bool nineGrid = false)
    {
      this.ImageTypeCode = nineGrid ? ImageIdentifierType.BannerCodeNineGrid : ImageIdentifierType.BannerCode;
      this.Id = bannerCode != (BannerCode) null ? bannerCode.Code : "";
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(PlayerId playerId)
    {
      this.ImageTypeCode = ImageIdentifierType.MultiplayerAvatar;
      this.Id = playerId.ToString();
      this.AdditionalArgs = "";
    }

    public ImageIdentifier(Banner banner)
    {
      this.ImageTypeCode = ImageIdentifierType.BannerCode;
      this.AdditionalArgs = "";
      if (banner != null)
        this.Id = BannerCode.CreateFrom(banner).Code;
      else
        this.Id = "";
    }

    public ImageIdentifier(ImageIdentifier code)
    {
      this.ImageTypeCode = code.ImageTypeCode;
      this.Id = code.Id;
      this.AdditionalArgs = code.AdditionalArgs;
    }

    public ImageIdentifier(string id, ImageIdentifierType type, string additionalArgs = "")
    {
      this.ImageTypeCode = type;
      this.Id = id;
      this.AdditionalArgs = additionalArgs;
    }

    public bool Equals(ImageIdentifier target) => target != null && this.ImageTypeCode == target.ImageTypeCode && this.Id.Equals(target.Id) && this.AdditionalArgs.Equals(target.AdditionalArgs);
  }
}
