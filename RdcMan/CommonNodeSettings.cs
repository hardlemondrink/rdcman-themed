// Decompiled with JetBrains decompiler
// Type: RdcMan.CommonNodeSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public abstract class CommonNodeSettings : SettingsGroup
  {
    protected CommonNodeSettings(string name)
      : base(name, "properties")
      => this.InheritSettingsType.Mode = InheritanceMode.Disabled;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.Properties);

    [Setting("name")]
    protected StringSetting NodeName { get; set; }

    [Setting("comment")]
    public StringSetting Comment { get; protected set; }
  }
}
