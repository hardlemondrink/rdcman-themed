// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class ServerSettings : CommonNodeSettings
  {
    internal const string TabName = "Server Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static ServerSettings() => typeof (ServerSettings).GetSettingProperties(out ServerSettings._settingProperties);

    public ServerSettings()
      : base("Server Settings")
    {
    }

    public StringSetting ServerName => this.NodeName;

    [Setting("displayName")]
    public StringSetting DisplayName { get; private set; }

    [Setting("connectionType")]
    public EnumSetting<RdcMan.ConnectionType> ConnectionType { get; private set; }

    [Setting("vmId")]
    public StringSetting VirtualMachineId { get; private set; }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new ServerPropertiesTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => ServerSettings._settingProperties;

    protected override void WriteSettings(XmlTextWriter tw, RdcTreeNode node)
    {
      HashSet<ISetting> exclusionSet = new HashSet<ISetting>();
      if (this.ConnectionType.Value == RdcMan.ConnectionType.Normal)
      {
        exclusionSet.Add((ISetting) this.ConnectionType);
        exclusionSet.Add((ISetting) this.VirtualMachineId);
      }
      if (this.ServerName.Value.Equals(this.DisplayName.Value))
        exclusionSet.Add((ISetting) this.DisplayName);
      if (string.IsNullOrEmpty(this.Comment.Value))
        exclusionSet.Add((ISetting) this.Comment);
      this.WriteSettings(tw, node, exclusionSet);
    }

    protected override void Copy(RdcTreeNode node)
    {
      if (!(node is ServerBase serverBase))
        return;
      this.Copy((SettingsGroup) serverBase.Properties);
    }
  }
}
