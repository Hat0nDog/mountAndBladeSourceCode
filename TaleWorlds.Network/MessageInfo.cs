// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageInfo
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.IO;

namespace TaleWorlds.Network
{
  public class MessageInfo
  {
    public bool DestinationIsPostBox = true;

    public string SourceIPAddress { get; set; }

    public Guid SourceClientId { get; set; }

    public string SourceUserName { get; set; }

    public string SourcePlatform { get; set; }

    public string SourcePlatformId { get; set; }

    public string DestinationPostBox { get; set; }

    public Guid DestinationClientId { get; set; }

    public void WriteTo(Stream stream, bool fromServer)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      binaryWriter.Write(this.DestinationIsPostBox);
      if (this.DestinationIsPostBox)
        binaryWriter.Write(this.DestinationPostBox);
      else
        binaryWriter.Write(this.DestinationClientId.ToByteArray());
      if (!fromServer)
        return;
      binaryWriter.Write(this.SourceIPAddress);
      binaryWriter.Write(this.SourceClientId.ToByteArray());
      binaryWriter.Write(this.SourceUserName);
      binaryWriter.Write(this.SourcePlatform);
      binaryWriter.Write(this.SourcePlatformId);
    }

    public static MessageInfo ReadFrom(Stream stream, bool fromServer)
    {
      MessageInfo messageInfo = new MessageInfo();
      BinaryReader binaryReader = new BinaryReader(stream);
      messageInfo.DestinationIsPostBox = binaryReader.ReadBoolean();
      if (messageInfo.DestinationIsPostBox)
        messageInfo.DestinationPostBox = binaryReader.ReadString();
      else
        messageInfo.DestinationClientId = new Guid(binaryReader.ReadBytes(16));
      if (fromServer)
      {
        messageInfo.SourceIPAddress = binaryReader.ReadString();
        messageInfo.SourceClientId = new Guid(binaryReader.ReadBytes(16));
        messageInfo.SourceUserName = binaryReader.ReadString();
        messageInfo.SourcePlatform = binaryReader.ReadString();
        messageInfo.SourcePlatformId = binaryReader.ReadString();
      }
      return messageInfo;
    }
  }
}
