// Decompiled with JetBrains decompiler
// Type: RdcMan.SecuritySettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class SecuritySettings : SettingsGroup
  {
    internal const string TabName = "Security Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static SecuritySettings() => typeof (SecuritySettings).GetSettingProperties(out SecuritySettings._settingProperties);

    public SecuritySettings()
      : base("Security Settings", "securitySettings")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new SecuritySettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => SecuritySettings._settingProperties;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.SecuritySettings);

    [Setting("authentication", DefaultValue = RdpClient.AuthenticationLevel.Warn)]
    public EnumSetting<RdpClient.AuthenticationLevel> AuthenticationLevel { get; private set; }
  }
}
