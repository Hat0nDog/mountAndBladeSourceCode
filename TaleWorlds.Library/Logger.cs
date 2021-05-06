// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Logger
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace TaleWorlds.Library
{
  public class Logger
  {
    private Queue<HTMLDebugData> _logQueue;
    private FileStream _fileStream;
    private FileStream _errorsFileStream;
    private static Encoding _logFileEncoding;
    private string _name;
    private bool _writeErrorsToDifferentFile;
    private static List<Logger> _loggers;
    private static Thread _thread;
    private static bool _running = true;
    private static bool _printedOnThisCycle = false;
    private static bool _isOver = false;
    public static string LogsFolder = "";

    public bool LogOnlyErrors { get; set; }

    static Logger()
    {
      Logger._logFileEncoding = Encoding.UTF8;
      Logger.LogsFolder = Environment.CurrentDirectory + "\\logs";
      Logger._loggers = new List<Logger>();
    }

    public Logger(string name)
      : this(name, false, false, false)
    {
    }

    public Logger(
      string name,
      bool writeErrorsToDifferentFile,
      bool logOnlyErrors,
      bool doNotUseProcessId,
      bool overwrite = false)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
      this._name = name;
      this._writeErrorsToDifferentFile = writeErrorsToDifferentFile;
      this.LogOnlyErrors = logOnlyErrors;
      this._logQueue = new Queue<HTMLDebugData>();
      int id = Process.GetCurrentProcess().Id;
      DateTime now = DateTime.Now;
      string path = Logger.LogsFolder;
      if (!doNotUseProcessId)
      {
        string str = withoutExtension + "_" + now.ToString("yyyyMMdd") + "_" + now.ToString("hhmmss") + "_" + (object) id;
        path = path + "/" + str;
      }
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      try
      {
        this._fileStream = Logger.CreateStream(path + "/" + this._name + ".html", overwrite);
      }
      catch (Exception ex)
      {
        this._name += "_2";
        this._fileStream = Logger.CreateStream(path + "/" + this._name + ".html", overwrite);
      }
      if (this._fileStream.Length == 0L)
      {
        string s = "<table></table>";
        byte[] bytes = Logger._logFileEncoding.GetBytes(s);
        this._fileStream.Write(bytes, 0, bytes.Length);
      }
      if (writeErrorsToDifferentFile)
        this._errorsFileStream = Logger.CreateStream(path + "/" + this._name + "_errors.html", overwrite);
      lock (Logger._loggers)
      {
        if (Logger._thread == null)
        {
          Logger._thread = new Thread(new ThreadStart(Logger.ThreadMain));
          Logger._thread.IsBackground = true;
          Logger._thread.Priority = ThreadPriority.BelowNormal;
          Logger._thread.Start();
        }
        Logger._loggers.Add(this);
      }
    }

    private static FileStream CreateStream(string fileName, bool overwrite = false)
    {
      FileStream fileStream = overwrite ? new FileStream(fileName, FileMode.Create) : new FileStream(fileName, FileMode.OpenOrCreate);
      if (fileStream.Length == 0L)
      {
        string s = "<table></table>";
        byte[] bytes = Logger._logFileEncoding.GetBytes(s);
        fileStream.Write(bytes, 0, bytes.Length);
      }
      return fileStream;
    }

    private static void ThreadMain()
    {
      while (Logger._running)
      {
        try
        {
          Logger.Printer();
        }
        catch (Exception ex)
        {
          Console.WriteLine("Exception on network debug thread: " + ex.Message);
        }
      }
      for (int index = 0; index < Logger._loggers.Count; ++index)
      {
        Logger._loggers[index]._fileStream.Close();
        Logger._loggers[index]._errorsFileStream.Close();
      }
      Logger._isOver = true;
    }

    private static void Printer()
    {
      while ((Logger._running || Logger._printedOnThisCycle) && Logger._loggers.Count > 0)
      {
        Logger._printedOnThisCycle = false;
        lock (Logger._loggers)
        {
          foreach (Logger logger in Logger._loggers)
          {
            if (logger.DoLoggingJob())
              Logger._printedOnThisCycle = true;
          }
        }
        if (!Logger._printedOnThisCycle)
          Thread.Sleep(1);
      }
    }

    private bool DoLoggingJob()
    {
      bool flag = false;
      HTMLDebugData htmlDebugData = (HTMLDebugData) null;
      lock (this._logQueue)
      {
        if (this._logQueue.Count > 0)
          htmlDebugData = this._logQueue.Dequeue();
      }
      if (htmlDebugData != null)
      {
        flag = true;
        htmlDebugData.Print(this._fileStream, Logger._logFileEncoding);
        if ((htmlDebugData.Info == HTMLDebugCategory.Error || htmlDebugData.Info == HTMLDebugCategory.Warning) && this._writeErrorsToDifferentFile)
          htmlDebugData.Print(this._errorsFileStream, Logger._logFileEncoding, false);
      }
      return flag;
    }

    public void Print(string log, HTMLDebugCategory debugInfo = HTMLDebugCategory.General) => this.Print(log, debugInfo, true);

    public void Print(string log, HTMLDebugCategory debugInfo, bool printOnGlobal)
    {
      if (this.LogOnlyErrors && (!this.LogOnlyErrors || debugInfo != HTMLDebugCategory.Error) && (!this.LogOnlyErrors || debugInfo != HTMLDebugCategory.Warning))
        return;
      HTMLDebugData htmlDebugData = new HTMLDebugData(log, debugInfo);
      lock (this._logQueue)
        this._logQueue.Enqueue(htmlDebugData);
      if (!printOnGlobal)
        return;
      Debug.Print(log);
    }

    public static void FinishAndCloseAll()
    {
      lock (Logger._loggers)
      {
        Logger._running = false;
        Logger._printedOnThisCycle = true;
      }
      do
        ;
      while (!Logger._isOver);
    }
  }
}
