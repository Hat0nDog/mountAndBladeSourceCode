// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.ClientWebSocketHandler
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
  [Obsolete]
  public class ClientWebSocketHandler
  {
    private int _messageSentCursor = -1;
    private int _messageQueueCursor = -1;
    private int _lastReceivedMessage = -1;
    private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageQueue;
    private ConcurrentQueue<WebSocketMessage> _outgoingSocketMessageLog;
    private int logBufferSize = 100;
    private static object consoleLock = new object();
    private const int sendChunkSize = 256;
    private const int receiveChunkSize = 256;
    private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000.0);
    private static ClientWebSocket _webSocket = (ClientWebSocket) null;

    public event ClientWebSocketHandler.MessageReceivedDelegate MessageReceived;

    public bool IsConnected => ClientWebSocketHandler._webSocket.State == WebSocketState.Open;

    public event ClientWebSocketHandler.OnErrorDelegate OnError;

    public event ClientWebSocketHandler.DisconnectedDelegate Disconnected;

    public event ClientWebSocketHandler.ConnectedDelegate Connected;

    public ClientWebSocketHandler()
    {
      this._outgoingSocketMessageQueue = new ConcurrentQueue<WebSocketMessage>();
      this._outgoingSocketMessageLog = new ConcurrentQueue<WebSocketMessage>();
      this.Connected += new ClientWebSocketHandler.ConnectedDelegate(this.ClientWebSocketConnected);
      ClientWebSocketHandler._webSocket = new ClientWebSocket();
    }

    public async Task Connect(
      string uri,
      string token,
      List<KeyValuePair<string, string>> headers = null)
    {
      ClientWebSocketHandler sender = this;
      try
      {
        if (ClientWebSocketHandler._webSocket.State == WebSocketState.Closed || ClientWebSocketHandler._webSocket.State == WebSocketState.Aborted)
        {
          ClientWebSocketHandler._webSocket.Dispose();
          ClientWebSocketHandler._webSocket = new ClientWebSocket();
        }
        if (ClientWebSocketHandler._webSocket.State == WebSocketState.None)
        {
          ClientWebSocketHandler._webSocket.Options.SetRequestHeader("Authorization", "Bearer " + token);
          if (headers != null)
          {
            foreach (KeyValuePair<string, string> header in headers)
              ClientWebSocketHandler._webSocket.Options.SetRequestHeader(header.Key, header.Value);
          }
        }
        await ClientWebSocketHandler._webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        if (ClientWebSocketHandler._webSocket.State == WebSocketState.Open)
        {
          if (sender.Connected != null)
            await sender.Connected(sender);
          Debug.Print("WebSocket connected");
        }
        await Task.WhenAll(sender.Receive(ClientWebSocketHandler._webSocket), sender.Send(ClientWebSocketHandler._webSocket));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception: {0}", (object) ex);
        sender.OnError(sender, ex);
      }
      finally
      {
        Console.WriteLine("WebSocket closed.");
      }
    }

    private async Task Receive(ClientWebSocket webSocket)
    {
      ClientWebSocketHandler webSocketHandler = this;
      ArraySegment<byte> inputSegment = new ArraySegment<byte>(new byte[65536]);
      using (MemoryStream ms = new MemoryStream())
      {
        while (webSocket.State == WebSocketState.Open)
        {
          try
          {
            WebSocketReceiveResult async = await webSocket.ReceiveAsync(inputSegment, CancellationToken.None);
            if (async.MessageType == WebSocketMessageType.Close)
            {
              try
              {
                if (webSocketHandler.Disconnected != null)
                  await webSocketHandler.Disconnected(webSocketHandler, true);
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Endpoint demanded closure", CancellationToken.None);
                Console.WriteLine("Endpoint demanded closure");
                break;
              }
              catch
              {
                break;
              }
            }
            else
            {
              ms.Write(inputSegment.Array, inputSegment.Offset, async.Count);
              if (async.EndOfMessage)
              {
                ms.GetBuffer();
                ms.Seek(0L, SeekOrigin.Begin);
                WebSocketMessage message = WebSocketMessage.ReadFrom(true, (Stream) ms);
                Console.WriteLine("Message:" + (object) ms.Length + " " + (object) message);
                if (webSocketHandler.MessageReceived != null)
                  webSocketHandler.MessageReceived(message, webSocketHandler);
                ms.Seek(0L, SeekOrigin.Begin);
              }
            }
          }
          catch (WebSocketException ex)
          {
            if (webSocketHandler.Disconnected == null)
              break;
            await webSocketHandler.Disconnected(webSocketHandler, false);
            break;
          }
        }
      }
    }

    private async Task Send(ClientWebSocket webSocket)
    {
      while (webSocket.State == WebSocketState.Open)
      {
        if (this._outgoingSocketMessageQueue.Count > 0)
        {
          WebSocketMessage webSocketMessage;
          if (this._outgoingSocketMessageQueue.TryDequeue(out webSocketMessage))
          {
            MemoryStream memoryStream = new MemoryStream();
            webSocketMessage.WriteTo(false, (Stream) memoryStream);
            await webSocket.SendAsync(new ArraySegment<byte>(memoryStream.GetBuffer()), WebSocketMessageType.Binary, true, CancellationToken.None);
            this._messageSentCursor = webSocketMessage.Cursor;
            this.AddMessageToBuffer(webSocketMessage);
            Debug.Print("message sent to: " + (webSocketMessage.MessageInfo.DestinationPostBox != null ? webSocketMessage.MessageInfo.DestinationPostBox : webSocketMessage.MessageInfo.DestinationClientId.ToString()));
          }
          webSocketMessage = (WebSocketMessage) null;
        }
        await Task.Delay(10);
      }
    }

    private void AddMessageToBuffer(WebSocketMessage message)
    {
      this._outgoingSocketMessageLog.Enqueue(message);
      while (this._outgoingSocketMessageLog.Count > this.logBufferSize)
      {
        WebSocketMessage result = (WebSocketMessage) null;
        this._outgoingSocketMessageLog.TryDequeue(out result);
      }
    }

    private void ResetMessageQueueByCursor(int serverCursor)
    {
      while (this._outgoingSocketMessageLog.Count > 0)
      {
        WebSocketMessage result = (WebSocketMessage) null;
        if (this._outgoingSocketMessageLog.TryDequeue(out result) && result.Cursor > serverCursor)
          this._outgoingSocketMessageQueue.Enqueue(result);
      }
    }

    private Task ClientWebSocketConnected(ClientWebSocketHandler sender)
    {
      this.SendCursorMessage();
      return (Task) Task.FromResult<int>(0);
    }

    private void SendCursorMessage() => this._outgoingSocketMessageQueue.Enqueue(WebSocketMessage.CreateCursorMessage(this._lastReceivedMessage));

    public async Task Disconnect(string reason, bool onDisconnectCommand)
    {
      ClientWebSocketHandler sender = this;
      try
      {
        if (ClientWebSocketHandler._webSocket != null)
        {
          if (ClientWebSocketHandler._webSocket.State != WebSocketState.Open)
          {
            if (ClientWebSocketHandler._webSocket.State != WebSocketState.Connecting)
              goto label_7;
          }
          await ClientWebSocketHandler._webSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, reason, CancellationToken.None);
        }
      }
      catch (ObjectDisposedException ex)
      {
        Debug.Print(ex.Message);
      }
label_7:
      if (sender.Disconnected == null)
        return;
      await sender.Disconnected(sender, onDisconnectCommand);
    }

    public void SendTextMessage(string postBoxId, string text)
    {
      ++this._messageQueueCursor;
      WebSocketMessage webSocketMessage = new WebSocketMessage();
      webSocketMessage.SetTextPayload(text);
      webSocketMessage.Cursor = this._messageQueueCursor;
      webSocketMessage.MessageType = MessageTypes.Rest;
      webSocketMessage.MessageInfo.DestinationPostBox = postBoxId;
      this._outgoingSocketMessageQueue.Enqueue(webSocketMessage);
    }

    public delegate void MessageReceivedDelegate(
      WebSocketMessage message,
      ClientWebSocketHandler socket);

    public delegate void OnErrorDelegate(ClientWebSocketHandler sender, Exception ex);

    public delegate Task DisconnectedDelegate(
      ClientWebSocketHandler sender,
      bool onDisconnectCommand);

    public delegate Task ConnectedDelegate(ClientWebSocketHandler sender);
  }
}
