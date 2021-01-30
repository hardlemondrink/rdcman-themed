// Decompiled with JetBrains decompiler
// Type: RdcMan.SmartGroupSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class SmartGroupSettings : GroupSettings
  {
    internal new const string TabName = "Smart Group Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static SmartGroupSettings() => typeof (SmartGroupSettings).GetSettingProperties(out SmartGroupSettings._settingProperties);

    public SmartGroupSettings()
      : base("Smart Group Settings")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new SmartGroupPropertiesTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => SmartGroupSettings._settingProperties;

    protected override void WriteSettings(XmlTextWriter tw, RdcTreeNode node)
    {
      HashSet<ISetting> exclusionSet = new HashSet<ISetting>();
      if (string.IsNullOrEmpty(this.Comment.Value))
        exclusionSet.Add((ISetting) this.Comment);
      this.WriteSettings(tw, node, exclusionSet);
    }

    protected override void Copy(RdcTreeNode node)
    {
    }
  }
}
