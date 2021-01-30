// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerDisplaySettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class ServerDisplaySettings : CommonDisplaySettings
  {
    private static Dictionary<string, SettingProperty> _settingProperties;

    static ServerDisplaySettings() => typeof (ServerDisplaySettings).GetSettingProperties(out ServerDisplaySettings._settingProperties);

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new ServerDisplaySettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => ServerDisplaySettings._settingProperties;
  }
}
