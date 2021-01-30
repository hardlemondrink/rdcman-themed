// Decompiled with JetBrains decompiler
// Type: RdcMan.RemoteDesktopSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class RemoteDesktopSettings : SettingsGroup
  {
    internal const string TabName = "Remote Desktop Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static RemoteDesktopSettings()
    {
      typeof (RemoteDesktopSettings).GetSettingProperties(out RemoteDesktopSettings._settingProperties);
      RemoteDesktopSettings._settingProperties["size"].Attribute.DefaultValue = (object) new Size(1024, 768);
    }

    public RemoteDesktopSettings()
      : base("Remote Desktop Settings", "remoteDesktop")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new RemoteDesktopTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => RemoteDesktopSettings._settingProperties;

    protected override void WriteSettings(XmlTextWriter tw, RdcTreeNode node)
    {
      HashSet<ISetting> exclusionSet = new HashSet<ISetting>();
      if (this.DesktopSizeSameAsClientAreaSize.Value || this.DesktopSizeFullScreen.Value)
        exclusionSet.Add((ISetting) this.DesktopSize);
      this.WriteSettings(tw, node, exclusionSet);
    }

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.RemoteDesktopSettings);

    [Setting("size")]
    public SizeSetting DesktopSize { get; private set; }

    [Setting("sameSizeAsClientArea")]
    public BoolSetting DesktopSizeSameAsClientAreaSize { get; private set; }

    [Setting("fullScreen", DefaultValue = true)]
    public BoolSetting DesktopSizeFullScreen { get; private set; }

    [Setting("colorDepth", DefaultValue = 24)]
    public IntSetting ColorDepth { get; private set; }
  }
}
