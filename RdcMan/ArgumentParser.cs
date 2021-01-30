// Decompiled with JetBrains decompiler
// Type: RdcMan.ArgumentParser
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;

namespace RdcMan
{
  public class ArgumentParser
  {
    public Dictionary<string, bool> Switches = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SwitchValues = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public List<string> PlainArgs = new List<string>();

    public void AddSwitch(string name, bool requiresValue) => this.Switches[name] = requiresValue;

    public void Parse()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 1; index < commandLineArgs.Length; ++index)
      {
        if (this.IsSwitch(commandLineArgs[index]))
        {
          string key = commandLineArgs[index].Substring(1);
          bool flag;
          if (!this.Switches.TryGetValue(key, out flag))
            throw new ArgumentException("Unexpected switch: " + commandLineArgs[index]);
          string empty = string.Empty;
          if (flag)
          {
            if (index >= commandLineArgs.Length - 1)
              throw new ArgumentException("Switch " + commandLineArgs[index] + " requires an argument");
            empty = commandLineArgs[++index];
          }
          this.SwitchValues[key] = empty;
        }
        else
          this.PlainArgs.Add(commandLineArgs[index]);
      }
    }

    public bool HasSwitch(string name) => this.SwitchValues.TryGetValue(name, out string _);

    private bool IsSwitch(string arg)
    {
      char ch = arg[0];
      return ch == '/' || ch == '-';
    }
  }
}
