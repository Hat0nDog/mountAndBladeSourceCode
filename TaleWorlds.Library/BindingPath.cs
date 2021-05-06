// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.BindingPath
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
  public class BindingPath
  {
    private readonly string _path;

    public string Path => this._path;

    public string[] Nodes { get; private set; }

    public string FirstNode => this.Nodes[0];

    public string LastNode => this.Nodes.Length == 0 ? "" : this.Nodes[this.Nodes.Length - 1];

    private BindingPath(string path, string[] nodes)
    {
      this._path = path;
      this.Nodes = nodes;
    }

    public BindingPath(string path)
    {
      this._path = path;
      this.Nodes = path.Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public BindingPath(int path)
    {
      this._path = path.ToString();
      this.Nodes = new string[1]{ this._path };
    }

    public static BindingPath CreateFromProperty(string propertyName) => new BindingPath(propertyName, new string[1]
    {
      propertyName
    });

    public BindingPath(IEnumerable<string> nodes)
    {
      this.Nodes = nodes.ToArray<string>();
      this._path = "";
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: ".ctor");
      for (int index = 0; index < this.Nodes.Length; ++index)
      {
        string node = this.Nodes[index];
        mbStringBuilder.Append<string>(node);
        if (index + 1 != this.Nodes.Length)
          mbStringBuilder.Append('\\');
      }
      this._path = mbStringBuilder.ToStringAndRelease();
    }

    private BindingPath(string[] firstNodes, string[] secondNodes)
    {
      this.Nodes = new string[firstNodes.Length + secondNodes.Length];
      this._path = "";
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: ".ctor");
      for (int index = 0; index < firstNodes.Length; ++index)
        this.Nodes[index] = firstNodes[index];
      for (int index = 0; index < secondNodes.Length; ++index)
        this.Nodes[index + firstNodes.Length] = secondNodes[index];
      for (int index = 0; index < this.Nodes.Length; ++index)
      {
        string node = this.Nodes[index];
        mbStringBuilder.Append<string>(node);
        if (index + 1 != this.Nodes.Length)
          mbStringBuilder.Append('\\');
      }
      this._path = mbStringBuilder.ToStringAndRelease();
    }

    public BindingPath SubPath
    {
      get
      {
        if (this.Nodes.Length <= 1)
          return (BindingPath) null;
        MBStringBuilder mbStringBuilder = new MBStringBuilder();
        mbStringBuilder.Initialize(callerMemberName: nameof (SubPath));
        for (int index = 1; index < this.Nodes.Length; ++index)
        {
          mbStringBuilder.Append<string>(this.Nodes[index]);
          if (index + 1 < this.Nodes.Length)
            mbStringBuilder.Append('\\');
        }
        return new BindingPath(mbStringBuilder.ToStringAndRelease());
      }
    }

    public BindingPath ParentPath
    {
      get
      {
        if (this.Nodes.Length <= 1)
          return (BindingPath) null;
        string path = "";
        for (int index = 0; index < this.Nodes.Length - 1; ++index)
        {
          path += this.Nodes[index];
          if (index + 1 < this.Nodes.Length - 1)
            path += "\\";
        }
        return new BindingPath(path);
      }
    }

    public override int GetHashCode() => this._path.GetHashCode();

    public override bool Equals(object obj)
    {
      BindingPath bindingPath = obj as BindingPath;
      return !(bindingPath == (BindingPath) null) && this.Path == bindingPath.Path;
    }

    public static bool operator ==(BindingPath a, BindingPath b)
    {
      bool flag1 = (object) a == null;
      bool flag2 = (object) b == null;
      if (flag1 & flag2)
        return true;
      return !(flag1 | flag2) && a.Path == b.Path;
    }

    public static bool operator !=(BindingPath a, BindingPath b) => !(a == b);

    public static bool IsRelatedWithPathAsString(string path, string referencePath) => referencePath.StartsWith(path);

    public static bool IsRelatedWithPath(string path, BindingPath referencePath) => referencePath.Path.StartsWith(path);

    public bool IsRelatedWith(BindingPath referencePath) => BindingPath.IsRelatedWithPath(this.Path, referencePath);

    public void DecrementIfRelatedWith(BindingPath path, int startIndex)
    {
      BindingPath referencePath = path;
      int result;
      if (!this.IsRelatedWith(referencePath) || referencePath.Nodes.Length >= this.Nodes.Length || (!int.TryParse(this.Nodes[referencePath.Nodes.Length], out result) || result < startIndex))
        return;
      --result;
      this.Nodes[referencePath.Nodes.Length] = result.ToString();
    }

    public BindingPath Simplify()
    {
      List<string> stringList = new List<string>();
      for (int index = 0; index < this.Nodes.Length; ++index)
      {
        string node = this.Nodes[index];
        if (node == ".." && stringList.Count > 0 && stringList[stringList.Count - 1] != "..")
          stringList.RemoveAt(stringList.Count - 1);
        else
          stringList.Add(node);
      }
      return new BindingPath((IEnumerable<string>) stringList);
    }

    public BindingPath Append(BindingPath bindingPath) => new BindingPath(this.Nodes, bindingPath.Nodes);

    public override string ToString() => this.Path;
  }
}
