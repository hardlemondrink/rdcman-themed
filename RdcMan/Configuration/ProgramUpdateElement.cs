// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.ProgramUpdateElement
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class ProgramUpdateElement : ConfigurationElement
  {
    [ConfigurationProperty("versionPath")]
    public string VersionPath
    {
      get => (string) this["versionPath"];
      set => this["versionPath"] = (object) value;
    }

    [ConfigurationProperty("updateUrl")]
    public string UpdateUrl
    {
      get => (string) this["updateUrl"];
      set => this["updateUrl"] = (object) value;
    }
  }
}
