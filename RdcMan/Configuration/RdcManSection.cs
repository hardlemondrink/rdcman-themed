// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.RdcManSection
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class RdcManSection : ConfigurationSection
  {
    [ConfigurationProperty("displaySizes", IsDefaultCollection = false)]
    public DisplaySizeElementCollection DisplaySizes => (DisplaySizeElementCollection) this["displaySizes"];

    [ConfigurationProperty("programUpdate")]
    public ProgramUpdateElement ProgramUpdate => (ProgramUpdateElement) this["programUpdate"];

    [ConfigurationProperty("warningThresholds")]
    public WarningThresholdsElement WarningThresholds => (WarningThresholdsElement) this["warningThresholds"];

    [ConfigurationProperty("logging")]
    public LoggingElement Logging => (LoggingElement) this["logging"];
  }
}
