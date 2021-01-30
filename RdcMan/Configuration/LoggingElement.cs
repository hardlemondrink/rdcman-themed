// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.LoggingElement
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class LoggingElement : ConfigurationElement
  {
    [ConfigurationProperty("enabled")]
    public bool Enabled
    {
      get => (bool) this["enabled"];
      set => this["enabled"] = (object) value;
    }

    [ConfigurationProperty("path")]
    public string Path
    {
      get => (string) this["path"];
      set => this["path"] = (object) value;
    }

    [ConfigurationProperty("maximumNumberOfFiles", DefaultValue = 5)]
    public int MaximumNumberOfFiles
    {
      get => (int) this["maximumNumberOfFiles"];
      set => this["maximumNumberOfFiles"] = (object) value;
    }
  }
}
