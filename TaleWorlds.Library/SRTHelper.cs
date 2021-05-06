// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.SRTHelper
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleWorlds.Library
{
  public static class SRTHelper
  {
    public static class SrtParser
    {
      private static readonly string[] _delimiters = new string[3]
      {
        "-->",
        "- >",
        "->"
      };

      public static List<SRTHelper.SubtitleItem> ParseStream(
        Stream srtStream,
        Encoding encoding)
      {
        if (!srtStream.CanRead || !srtStream.CanSeek)
          throw new ArgumentException(string.Format("Stream must be seekable and readable in a subtitles parser. Operation interrupted; isSeekable: {0} - isReadable: {1}", (object) srtStream.CanSeek, (object) srtStream.CanSeek));
        srtStream.Position = 0L;
        StreamReader streamReader = new StreamReader(srtStream, encoding, true);
        List<SRTHelper.SubtitleItem> subtitleItemList = new List<SRTHelper.SubtitleItem>();
        List<string> list1 = SRTHelper.SrtParser.GetSrtSubTitleParts((TextReader) streamReader).ToList<string>();
        if (list1.Count <= 0)
          throw new FormatException("Parsing as srt returned no srt part.");
        foreach (string str in list1)
        {
          string[] separator = new string[1]
          {
            Environment.NewLine
          };
          List<string> list2 = ((IEnumerable<string>) str.Split(separator, StringSplitOptions.None)).Select<string, string>((Func<string, string>) (s => s.Trim())).Where<string>((Func<string, bool>) (l => !string.IsNullOrEmpty(l))).ToList<string>();
          SRTHelper.SubtitleItem subtitleItem = new SRTHelper.SubtitleItem();
          foreach (string line in list2)
          {
            if (subtitleItem.StartTime == 0 && subtitleItem.EndTime == 0)
            {
              int startTc;
              int endTc;
              if (SRTHelper.SrtParser.TryParseTimecodeLine(line, out startTc, out endTc))
              {
                subtitleItem.StartTime = startTc;
                subtitleItem.EndTime = endTc;
              }
            }
            else
              subtitleItem.Lines.Add(line);
          }
          if ((subtitleItem.StartTime != 0 || subtitleItem.EndTime != 0) && subtitleItem.Lines.Count > 0)
            subtitleItemList.Add(subtitleItem);
        }
        return subtitleItemList.Count > 0 ? subtitleItemList : throw new ArgumentException("Stream is not in a valid Srt format");
      }

      private static IEnumerable<string> GetSrtSubTitleParts(TextReader reader)
      {
        MBStringBuilder sb = new MBStringBuilder();
        sb.Initialize(callerMemberName: nameof (GetSrtSubTitleParts));
        string str1;
        while ((str1 = reader.ReadLine()) != null)
        {
          if (string.IsNullOrEmpty(str1.Trim()))
          {
            string str2 = sb.ToStringAndRelease().TrimEnd();
            if (!string.IsNullOrEmpty(str2))
              yield return str2;
            sb.Initialize(callerMemberName: nameof (GetSrtSubTitleParts));
          }
          else
            sb.AppendLine<string>(str1);
        }
        if (sb.Length > 0)
          yield return sb.ToStringAndRelease();
        else
          sb.Release();
      }

      private static bool TryParseTimecodeLine(string line, out int startTc, out int endTc)
      {
        string[] strArray = line.Split(SRTHelper.SrtParser._delimiters, StringSplitOptions.None);
        if (strArray.Length != 2)
        {
          startTc = -1;
          endTc = -1;
          return false;
        }
        startTc = SRTHelper.SrtParser.ParseSrtTimecode(strArray[0]);
        endTc = SRTHelper.SrtParser.ParseSrtTimecode(strArray[1]);
        return true;
      }

      private static int ParseSrtTimecode(string s)
      {
        Match match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+([,\\.][0-9]+)?");
        if (match.Success)
        {
          s = match.Value;
          TimeSpan result;
          if (TimeSpan.TryParse(s.Replace(',', '.'), out result))
            return (int) result.TotalMilliseconds;
        }
        return -1;
      }
    }

    public static class StreamHelpers
    {
      public static Stream CopyStream(Stream inputStream)
      {
        MemoryStream memoryStream = new MemoryStream();
        int count;
        do
        {
          byte[] buffer = new byte[1024];
          count = inputStream.Read(buffer, 0, 1024);
          memoryStream.Write(buffer, 0, count);
        }
        while (inputStream.CanRead && count > 0);
        memoryStream.ToArray();
        return (Stream) memoryStream;
      }
    }

    public class SubtitleItem
    {
      public int StartTime { get; set; }

      public int EndTime { get; set; }

      public List<string> Lines { get; set; }

      public SubtitleItem() => this.Lines = new List<string>();

      public override string ToString()
      {
        TimeSpan timeSpan1 = new TimeSpan(0, 0, 0, 0, this.StartTime);
        TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, this.EndTime);
        return string.Format("{0} --> {1}: {2}", (object) timeSpan1.ToString("G"), (object) timeSpan2.ToString("G"), (object) string.Join(Environment.NewLine, (IEnumerable<string>) this.Lines));
      }
    }
  }
}
