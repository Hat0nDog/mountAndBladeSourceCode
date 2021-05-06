// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.HTMLDebugData
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.IO;
using System.Text;

namespace TaleWorlds.Library
{
  internal class HTMLDebugData
  {
    private string _log;
    private string _currentTime;

    internal HTMLDebugCategory Info { get; private set; }

    internal HTMLDebugData(string log, HTMLDebugCategory info)
    {
      this._log = log;
      this.Info = info;
      this._currentTime = DateTime.Now.ToString("yyyy/M/d h:mm:ss.fff");
    }

    private string Color
    {
      get
      {
        string str = "000000";
        switch (this.Info)
        {
          case HTMLDebugCategory.General:
            str = "000000";
            break;
          case HTMLDebugCategory.Connection:
            str = "FF00FF";
            break;
          case HTMLDebugCategory.IncomingMessage:
            str = "EE8800";
            break;
          case HTMLDebugCategory.OutgoingMessage:
            str = "AA6600";
            break;
          case HTMLDebugCategory.Database:
            str = "00008B";
            break;
          case HTMLDebugCategory.Warning:
            str = "0000FF";
            break;
          case HTMLDebugCategory.Error:
            str = "FF0000";
            break;
          case HTMLDebugCategory.Other:
            str = "000000";
            break;
        }
        return str;
      }
    }

    private ConsoleColor ConsoleColor
    {
      get
      {
        ConsoleColor consoleColor = ConsoleColor.Green;
        switch (this.Info)
        {
          case HTMLDebugCategory.Warning:
            consoleColor = ConsoleColor.Yellow;
            break;
          case HTMLDebugCategory.Error:
            consoleColor = ConsoleColor.Red;
            break;
        }
        return consoleColor;
      }
    }

    internal void Print(FileStream fileStream, Encoding encoding, bool writeToConsole = true)
    {
      if (writeToConsole)
      {
        Console.ForegroundColor = this.ConsoleColor;
        Console.WriteLine(this._log);
        Console.ForegroundColor = this.ConsoleColor;
      }
      int byteCount = encoding.GetByteCount("</table>");
      string color = this.Color;
      string s = "<tr>" + this.TableCell(this._log.Replace("\n", "<br/>"), color) + this.TableCell(this.Info.ToString(), color) + this.TableCell(this._currentTime, color) + "</tr></table>";
      byte[] bytes = encoding.GetBytes(s);
      fileStream.Seek((long) -byteCount, SeekOrigin.End);
      fileStream.Write(bytes, 0, bytes.Length);
    }

    private string TableCell(string innerText, string color) => "<td><font color=\"#" + color + "\">" + innerText + "</font></td><td>";
  }
}
