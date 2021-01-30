// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectionSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class ConnectionSettings : SettingsGroup
  {
    internal const string TabName = "Connection Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static ConnectionSettings() => typeof (ConnectionSettings).GetSettingProperties(out ConnectionSettings._settingProperties);

    public ConnectionSettings()
      : base("Connection Settings", "connectionSettings")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new ConnectionSettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => ConnectionSettings._settingProperties;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.ConnectionSettings);

    [Setting("connectToConsole")]
    public BoolSetting ConnectToConsole { get; private set; }

    [Setting("startProgram")]
    public StringSetting StartProgram { get; private set; }

    [Setting("workingDir")]
    public StringSetting WorkingDir { get; private set; }

    [Setting("port", DefaultValue = 3389)]
    public IntSetting Port { get; private set; }

    [Setting("loadBalanceInfo")]
    public StringSetting LoadBalanceInfo { get; private set; }
  }
}
