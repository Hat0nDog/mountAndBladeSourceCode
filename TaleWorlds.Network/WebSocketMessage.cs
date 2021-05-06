// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.WebSocketMessage
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.IO;
using System.Text;

namespace TaleWorlds.Network
{
  [Obsolete]
  public class WebSocketMessage
  {
    public static Encoding Encoding = Encoding.UTF8;

    public byte[] Payload { get; set; }

    public WebSocketMessage() => this.MessageInfo = new MessageInfo();

    public void SetTextPayload(string payload) => this.Payload = WebSocketMessage.Encoding.GetBytes(payload);

    public MessageInfo MessageInfo { get; set; }

    public int Cursor { get; set; }

    public MessageTypes MessageType { get; set; }

    public void WriteTo(bool fromServer, Stream stream)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      this.MessageInfo.WriteTo(stream, fromServer);
      binaryWriter.Write(this.Payload.Length);
      binaryWriter.Write(this.Payload, 0, this.Payload.Length);
      binaryWriter.Write(this.Cursor);
      binaryWriter.Write((byte) this.MessageType);
    }

    public static WebSocketMessage ReadFrom(bool fromServer, byte[] payload)
    {
      using (MemoryStream memoryStream = new MemoryStream(payload))
        return WebSocketMessage.ReadFrom(fromServer, (Stream) memoryStream);
    }

    public static WebSocketMessage ReadFrom(bool fromServer, Stream stream)
    {
      WebSocketMessage webSocketMessage = new WebSocketMessage();
      BinaryReader binaryReader = new BinaryReader(stream);
      webSocketMessage.MessageInfo = MessageInfo.ReadFrom(stream, fromServer);
      int count = binaryReader.ReadInt32();
      webSocketMessage.Payload = binaryReader.ReadBytes(count);
      webSocketMessage.Cursor = binaryReader.ReadInt32();
      webSocketMessage.MessageType = (MessageTypes) binaryReader.ReadByte();
      return webSocketMessage;
    }

    public static WebSocketMessage CreateCursorMessage(int cursor) => new WebSocketMessage()
    {
      MessageType = MessageTypes.Cursor,
      MessageInfo = {
        DestinationPostBox = ""
      },
      Payload = BitConverter.GetBytes(cursor)
    };

    public static WebSocketMessage CreateCloseMessage() => new WebSocketMessage()
    {
      MessageType = MessageTypes.Close
    };

    public int GetCursor()
    {
      if (this.MessageType == MessageTypes.Cursor)
        return BitConverter.ToInt32(this.Payload, 0);
      throw new Exception("not a cursor message");
    }
  }
}
