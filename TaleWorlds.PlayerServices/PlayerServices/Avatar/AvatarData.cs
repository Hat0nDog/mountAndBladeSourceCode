// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.AvatarData
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

namespace TaleWorlds.PlayerServices.Avatar
{
  public class AvatarData
  {
    public AvatarData() => this.Status = AvatarData.DataStatus.NotReady;

    public AvatarData(byte[] image, uint width, uint height) => this.SetImageData(image, width, height);

    public AvatarData(byte[] image) => this.SetImageData(image);

    public void SetImageData(byte[] image, uint width, uint height)
    {
      this.Image = image;
      this.Width = width;
      this.Height = height;
      this.Type = AvatarData.ImageType.Raw;
      this.Status = AvatarData.DataStatus.Ready;
    }

    public void SetImageData(byte[] image)
    {
      this.Image = image;
      this.Type = AvatarData.ImageType.Image;
      this.Status = AvatarData.DataStatus.Ready;
    }

    public void SetFailed() => this.Status = AvatarData.DataStatus.Failed;

    public byte[] Image { get; private set; }

    public uint Width { get; private set; }

    public uint Height { get; private set; }

    public AvatarData.ImageType Type { get; private set; }

    public AvatarData.DataStatus Status { get; private set; }

    public enum ImageType
    {
      Image,
      Raw,
    }

    public enum DataStatus
    {
      NotReady,
      Ready,
      Failed,
    }
  }
}
