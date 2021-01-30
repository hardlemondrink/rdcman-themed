// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupDisplaySettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class GroupDisplaySettings : CommonDisplaySettings
  {
    private static Dictionary<string, SettingProperty> _settingProperties;

    static GroupDisplaySettings() => typeof (GroupDisplaySettings).GetSettingProperties(out GroupDisplaySettings._settingProperties);

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new GroupDisplaySettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => GroupDisplaySettings._settingProperties;

    [Setting("liveThumbnailUpdates", DefaultValue = true)]
    public BoolSetting SessionThumbnailPreview { get; protected set; }

    [Setting("allowThumbnailSessionInteraction")]
    public BoolSetting AllowThumbnailSessionInteraction { get; protected set; }

    [Setting("showDisconnectedThumbnails", DefaultValue = true)]
    public BoolSetting ShowDisconnectedThumbnails { get; protected set; }
  }
}
