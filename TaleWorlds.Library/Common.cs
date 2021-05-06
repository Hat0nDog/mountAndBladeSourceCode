// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Common
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
  public static class Common
  {
    private static IPlatformFileHelper _fileHelper = (IPlatformFileHelper) null;
    private static IPlatformWebHelper _webHelper = (IPlatformWebHelper) new PlatformWebHelperDefault();
    private static ParallelOptions _parallelOptions = (ParallelOptions) null;

    public static IPlatformFileHelper PlatformFileHelper
    {
      get => Common._fileHelper;
      set => Common._fileHelper = value;
    }

    public static IPlatformWebHelper PlatformWebHelper
    {
      get => Common._webHelper;
      set => Common._webHelper = value;
    }

    public static byte[] CombineBytes(
      byte[] arr1,
      byte[] arr2,
      byte[] arr3 = null,
      byte[] arr4 = null,
      byte[] arr5 = null)
    {
      byte[] numArray = new byte[arr1.Length + arr2.Length + (arr3 != null ? arr3.Length : 0) + (arr4 != null ? arr4.Length : 0) + (arr5 != null ? arr5.Length : 0)];
      int dstOffset = 0;
      if (arr1.Length != 0)
      {
        Buffer.BlockCopy((Array) arr1, 0, (Array) numArray, dstOffset, arr1.Length);
        dstOffset += arr1.Length;
      }
      if (arr2.Length != 0)
      {
        Buffer.BlockCopy((Array) arr2, 0, (Array) numArray, dstOffset, arr2.Length);
        dstOffset += arr2.Length;
      }
      if (arr3 != null && arr3.Length != 0)
      {
        Buffer.BlockCopy((Array) arr3, 0, (Array) numArray, dstOffset, arr3.Length);
        dstOffset += arr3.Length;
      }
      if (arr4 != null && arr4.Length != 0)
      {
        Buffer.BlockCopy((Array) arr4, 0, (Array) numArray, dstOffset, arr4.Length);
        dstOffset += arr4.Length;
      }
      if (arr5 != null && arr5.Length != 0)
        Buffer.BlockCopy((Array) arr5, 0, (Array) numArray, dstOffset, arr5.Length);
      return numArray;
    }

    public static string CalculateMD5Hash(string input)
    {
      MD5 md5 = MD5.Create();
      byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
      md5.Dispose();
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(32, nameof (CalculateMD5Hash));
      for (int index = 0; index < hash.Length; ++index)
        mbStringBuilder.Append<string>(hash[index].ToString("X2"));
      return mbStringBuilder.ToStringAndRelease();
    }

    public static string ToRoman(int number)
    {
      if (number >= 0)
        ;
      if (number < 1)
        return string.Empty;
      if (number >= 1000)
        return "M" + Common.ToRoman(number - 1000);
      if (number >= 900)
        return "CM" + Common.ToRoman(number - 900);
      if (number >= 500)
        return "D" + Common.ToRoman(number - 500);
      if (number >= 400)
        return "CD" + Common.ToRoman(number - 400);
      if (number >= 100)
        return "C" + Common.ToRoman(number - 100);
      if (number >= 90)
        return "XC" + Common.ToRoman(number - 90);
      if (number >= 50)
        return "L" + Common.ToRoman(number - 50);
      if (number >= 40)
        return "XL" + Common.ToRoman(number - 40);
      if (number >= 10)
        return "X" + Common.ToRoman(number - 10);
      if (number >= 9)
        return "IX" + Common.ToRoman(number - 9);
      if (number >= 5)
        return "V" + Common.ToRoman(number - 5);
      if (number >= 4)
        return "IV" + Common.ToRoman(number - 4);
      return number >= 1 ? "I" + Common.ToRoman(number - 1) : "";
    }

    public static int GetDJB2(string str)
    {
      int num = 5381;
      for (int index = 0; index < str.Length; ++index)
        num = (num << 5) + num + (int) str[index];
      return num;
    }

    public static byte[] SerializeObject(object sObject)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      try
      {
        binaryFormatter.Serialize((Stream) memoryStream, sObject);
      }
      catch (Exception ex)
      {
        Debug.Print(ex.ToString());
      }
      return memoryStream.ToArray();
    }

    public static object DeserializeObject(byte[] serializeData) => Common.DeserializeObject(serializeData, 0, serializeData.Length);

    public static object DeserializeObject(byte[] serializeData, int index, int length)
    {
      MemoryStream memoryStream = new MemoryStream(serializeData, index, length, false);
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      try
      {
        return binaryFormatter.Deserialize((Stream) memoryStream);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string ConfigName => new DirectoryInfo(Directory.GetCurrentDirectory()).Name;

    public static Type FindType(string typeName)
    {
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Type type = assembly.GetType(typeName);
        if (type != (Type) null)
          return type;
      }
      return (Type) null;
    }

    public static void MemoryCleanup()
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    public static object DynamicInvokeWithLog(this Delegate method, params object[] args)
    {
      object obj = (object) null;
      try
      {
        return method.DynamicInvoke(args);
      }
      catch (Exception ex)
      {
        MethodInfo method1 = method.Method;
        object target = method.Target;
        object[] objArray = args;
        Common.PrintDynamicInvokeDebugInfo(ex, method1, target, objArray);
        obj = (object) null;
        throw;
      }
    }

    public static object InvokeWithLog(
      this MethodInfo methodInfo,
      object obj,
      params object[] args)
    {
      object obj1 = (object) null;
      try
      {
        return methodInfo.Invoke(obj, args);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo1 = methodInfo;
        object obj2 = obj;
        object[] objArray = args;
        Common.PrintDynamicInvokeDebugInfo(ex, methodInfo1, obj2, objArray);
        obj1 = (object) null;
        throw;
      }
    }

    public static object InvokeWithLog(this ConstructorInfo constructorInfo, params object[] args)
    {
      object obj = (object) null;
      try
      {
        return constructorInfo.Invoke(args);
      }
      catch (Exception ex)
      {
        MethodInfo methodInfo = Common.GetMethodInfo<object[]>((Expression<Action<object[]>>) (a => constructorInfo.Invoke(a)));
        Common.PrintDynamicInvokeDebugInfo(ex, methodInfo, (object) null, args);
        obj = (object) null;
        throw;
      }
    }

    private static string GetStackTraceRaw(Exception e, int skipCount = 0)
    {
      StackTrace stackTrace = new StackTrace(e, 0, false);
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (GetStackTraceRaw));
      for (int index = 0; index < stackTrace.FrameCount; ++index)
      {
        if (index >= skipCount)
        {
          string str = "unknown_module.dll";
          try
          {
            StackFrame frame = stackTrace.GetFrame(index);
            MethodBase method = frame.GetMethod();
            str = method.Module.Assembly.Location;
            int ilOffset = frame.GetILOffset();
            int metadataToken = method.MetadataToken;
            mbStringBuilder.AppendLine<string>(str + "@" + (object) metadataToken + "@" + (object) ilOffset);
          }
          catch
          {
            mbStringBuilder.AppendLine<string>(str + "@-1@-1");
          }
        }
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    private static void WalkInnerExceptionRecursive(Exception InnerException, ref string StackStr)
    {
      if (InnerException == null)
        return;
      Common.WalkInnerExceptionRecursive(InnerException.InnerException, ref StackStr);
      string stackTraceRaw = Common.GetStackTraceRaw(InnerException);
      StackStr += stackTraceRaw;
      StackStr += "---End of stack trace from previous location where exception was thrown ---";
      StackStr += Environment.NewLine;
    }

    private static void PrintDynamicInvokeDebugInfo(
      Exception e,
      MethodInfo methodInfo,
      object obj,
      params object[] args)
    {
      string str1 = "Exception occurred inside invoke: " + methodInfo.Name;
      if (obj != null)
        str1 = str1 + "\nTarget type: " + obj.GetType().FullName;
      if (args != null)
      {
        str1 = str1 + "\nArgument count: " + (object) args.Length;
        foreach (object obj1 in args)
          str1 = obj1 != null ? str1 + "\nArgument type is " + obj1.GetType().FullName : str1 + "\nArgument is null";
      }
      string StackStr = "";
      if (e.InnerException != null)
        Common.WalkInnerExceptionRecursive(e, ref StackStr);
      Exception exception = e;
      while (exception.InnerException != null)
        exception = exception.InnerException;
      string str2 = str1 + "\nInner message: " + exception.Message;
      Debug.SetCrashReportCustomString(str2);
      Debug.SetCrashReportCustomStack(StackStr);
      Debug.Print(str2);
    }

    public static string GetISOLanguageCodeOfLocalizationCode(string localizationLanguageCode)
    {
      switch (localizationLanguageCode.ToLower())
      {
        case "chinese":
        case "中文":
        case "简体中文":
        case "繁體中文":
          return "zh";
        case "deutsch":
        case "german":
          return "de";
        case "english":
          return "en";
        case "français":
        case "french":
          return "fr";
        case "korean":
        case "한국어":
          return "kr";
        case "polish":
        case "polski":
          return "pl";
        case "turkce":
        case "turkish":
        case "turkçe":
        case "türkce":
        case "türkçe":
          return "tr";
        default:
          return "en";
      }
    }

    public static uint ParseIpAddress(string address)
    {
      byte[] addressBytes = IPAddress.Parse(address).GetAddressBytes();
      return (uint) (((int) addressBytes[0] << 24) + ((int) addressBytes[1] << 16) + ((int) addressBytes[2] << 8)) + (uint) addressBytes[3];
    }

    public static bool IsAllLetters(string text)
    {
      if (text == null)
        return false;
      foreach (char c in text)
      {
        if (!char.IsLetter(c))
          return false;
      }
      return true;
    }

    public static bool IsAllLettersOrWhiteSpaces(string text)
    {
      if (text == null)
        return false;
      foreach (char c in text)
      {
        if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
          return false;
      }
      return true;
    }

    public static bool IsCharAsian(char character)
    {
      if (character >= '一' && character <= '\u9FFF' || character >= '㐀' && character <= '\u4DBF' || (character >= '㐀' && character <= '\u4DBF' || character >= char.MinValue && character <= '캯') || (character >= '⺀' && character <= '\u31EF' || character >= '豈' && character <= '\uFAFF' || character >= '︰' && character <= '﹏'))
        return true;
      return character >= '⠀' && character <= '﨟';
    }

    public static MethodInfo GetMethodInfo(Expression<Action> expression) => Common.GetMethodInfo((LambdaExpression) expression);

    public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) => Common.GetMethodInfo((LambdaExpression) expression);

    public static MethodInfo GetMethodInfo<T, TResult>(
      Expression<Func<T, TResult>> expression)
    {
      return Common.GetMethodInfo((LambdaExpression) expression);
    }

    public static MethodInfo GetMethodInfo(LambdaExpression expression)
    {
      if (!(expression.Body is MethodCallExpression body))
        throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
      return body.Method;
    }

    public static ParallelOptions ParallelOptions
    {
      get
      {
        if (Common._parallelOptions == null)
        {
          Common._parallelOptions = new ParallelOptions();
          Common._parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount - 2;
        }
        return Common._parallelOptions;
      }
    }
  }
}
