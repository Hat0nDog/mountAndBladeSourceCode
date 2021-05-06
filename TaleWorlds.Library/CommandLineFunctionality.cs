// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CommandLineFunctionality
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Library
{
  public static class CommandLineFunctionality
  {
    private static Dictionary<string, CommandLineFunctionality.CommandLineFunction> AllFunctions = new Dictionary<string, CommandLineFunctionality.CommandLineFunction>();

    private static bool CheckAssemblyReferencesThis(Assembly assembly)
    {
      Assembly assembly1 = typeof (CommandLineFunctionality).Assembly;
      if (assembly1.GetName().Name == assembly.GetName().Name)
        return true;
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.Name == assembly1.GetName().Name)
          return true;
      }
      return false;
    }

    public static List<string> CollectCommandLineFunctions()
    {
      List<string> stringList = new List<string>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (CommandLineFunctionality.CheckAssemblyReferencesThis(assembly))
        {
          foreach (Type type in assembly.GetTypes())
          {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
              object[] customAttributes = method.GetCustomAttributes(typeof (CommandLineFunctionality.CommandLineArgumentFunction), false);
              if (customAttributes != null && customAttributes.Length != 0 && (customAttributes[0] is CommandLineFunctionality.CommandLineArgumentFunction argumentFunction5 && !(method.ReturnType != typeof (string))))
              {
                string name = argumentFunction5.Name;
                string key = argumentFunction5.GroupName + "." + name;
                stringList.Add(key);
                CommandLineFunctionality.CommandLineFunction commandLineFunction = new CommandLineFunctionality.CommandLineFunction((Func<List<string>, string>) Delegate.CreateDelegate(typeof (Func<List<string>, string>), method));
                CommandLineFunctionality.AllFunctions.Add(key, commandLineFunction);
              }
            }
          }
        }
      }
      return stringList;
    }

    public static bool HasFunctionForCommand(string command) => CommandLineFunctionality.AllFunctions.TryGetValue(command, out CommandLineFunctionality.CommandLineFunction _);

    public static string CallFunction(string concatName, string concatArguments, out bool found)
    {
      CommandLineFunctionality.CommandLineFunction commandLineFunction;
      if (CommandLineFunctionality.AllFunctions.TryGetValue(concatName, out commandLineFunction))
      {
        List<string> objects;
        if (concatArguments != string.Empty)
          objects = new List<string>((IEnumerable<string>) concatArguments.Split(' '));
        else
          objects = new List<string>();
        found = true;
        return commandLineFunction.Call(objects);
      }
      found = false;
      return "Could not find the command " + concatName;
    }

    private class CommandLineFunction
    {
      public Func<List<string>, string> CommandLineFunc;
      public List<CommandLineFunctionality.CommandLineFunction> Children;

      public CommandLineFunction(Func<List<string>, string> commandlinefunc)
      {
        this.CommandLineFunc = commandlinefunc;
        this.Children = new List<CommandLineFunctionality.CommandLineFunction>();
      }

      public string Call(List<string> objects) => this.CommandLineFunc(objects);
    }

    public class CommandLineArgumentFunction : Attribute
    {
      public string Name;
      public string GroupName;

      public CommandLineArgumentFunction(string name, string groupname)
      {
        this.Name = name;
        this.GroupName = groupname;
      }
    }
  }
}
