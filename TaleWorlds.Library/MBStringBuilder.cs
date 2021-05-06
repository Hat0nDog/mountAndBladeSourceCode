// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.MBStringBuilder
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace TaleWorlds.Library
{
  public struct MBStringBuilder
  {
    private StringBuilder _cachedStringBuilder;

    public void Initialize(int capacity = 16, [CallerMemberName] string callerMemberName = "") => this._cachedStringBuilder = MBStringBuilder.CachedStringBuilder.Acquire(capacity);

    public string ToStringAndRelease()
    {
      string str = this._cachedStringBuilder.ToString();
      this.Release();
      return str;
    }

    public void Release()
    {
      MBStringBuilder.CachedStringBuilder.Release(this._cachedStringBuilder);
      this._cachedStringBuilder = (StringBuilder) null;
    }

    public void Append(char value) => this._cachedStringBuilder.Append(value);

    public void Append(int value) => this._cachedStringBuilder.Append(value);

    public void Append(uint value) => this._cachedStringBuilder.Append(value);

    public void Append(float value) => this._cachedStringBuilder.Append(value);

    public void Append(double value) => this._cachedStringBuilder.Append(value);

    public void Append<T>(T value) where T : class => this._cachedStringBuilder.Append((object) value);

    public void AppendLine() => this._cachedStringBuilder.AppendLine();

    public void AppendLine<T>(T value) where T : class
    {
      this.Append<T>(value);
      this.AppendLine();
    }

    public int Length => this._cachedStringBuilder.Length;

    public override string ToString() => (string) null;

    private static class CachedStringBuilder
    {
      private const int MaxBuilderSize = 4096;
      [ThreadStatic]
      private static StringBuilder _cachedStringBuilder;

      public static StringBuilder Acquire(int capacity = 16)
      {
        if (capacity > 4096 || MBStringBuilder.CachedStringBuilder._cachedStringBuilder == null)
          return new StringBuilder(capacity);
        StringBuilder cachedStringBuilder = MBStringBuilder.CachedStringBuilder._cachedStringBuilder;
        MBStringBuilder.CachedStringBuilder._cachedStringBuilder = (StringBuilder) null;
        cachedStringBuilder.EnsureCapacity(capacity);
        return cachedStringBuilder;
      }

      public static void Release(StringBuilder sb)
      {
        if (sb.Capacity > 4096)
          return;
        MBStringBuilder.CachedStringBuilder._cachedStringBuilder = sb;
        MBStringBuilder.CachedStringBuilder._cachedStringBuilder.Clear();
      }

      public static string GetStringAndReleaseBuilder(StringBuilder sb)
      {
        string str = sb.ToString();
        MBStringBuilder.CachedStringBuilder.Release(sb);
        return str;
      }
    }
  }
}
