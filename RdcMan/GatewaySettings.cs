// Decompiled with JetBrains decompiler
// Type: RdcMan.GatewaySettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class GatewaySettings : LogonCredentials
  {
    public new const string TabName = "Gateway Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static GatewaySettings() => typeof (GatewaySettings).GetSettingProperties(out GatewaySettings._settingProperties);

    public GatewaySettings()
      : base("Gateway Settings", "gatewaySettings")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new GatewaySettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => GatewaySettings._settingProperties;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.GatewaySettings);

    [Setting("enabled")]
    public BoolSetting UseGatewayServer { get; private set; }

    [Setting("hostName")]
    public StringSetting HostName { get; private set; }

    [Setting("logonMethod", DefaultValue = RdpClient.GatewayLogonMethod.Any)]
    public EnumSetting<RdpClient.GatewayLogonMethod> LogonMethod { get; private set; }

    [Setting("localBypass", DefaultValue = true)]
    public BoolSetting BypassGatewayForLocalAddresses { get; private set; }

    [Setting("credSharing", DefaultValue = true)]
    public BoolSetting CredentialSharing { get; private set; }

    [Setting("enableCredSspSupport", DefaultValue = true, IsObsolete = true)]
    public BoolSetting EnableCredentialSspSupport { get; private set; }
  }
}
