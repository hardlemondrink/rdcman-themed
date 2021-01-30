// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.WarningThresholdsElement
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class WarningThresholdsElement : ConfigurationElement
  {
    [ConfigurationProperty("connect", DefaultValue = 10)]
    public int Connect
    {
      get => (int) this["connect"];
      set => this["connect"] = (object) value;
    }
  }
}
