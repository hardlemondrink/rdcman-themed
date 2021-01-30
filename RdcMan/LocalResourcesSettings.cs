// Decompiled with JetBrains decompiler
// Type: RdcMan.LocalResourcesSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class LocalResourcesSettings : SettingsGroup
  {
    internal const string TabName = "Local Resources";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static LocalResourcesSettings() => typeof (LocalResourcesSettings).GetSettingProperties(out LocalResourcesSettings._settingProperties);

    public LocalResourcesSettings()
      : base("Local Resources", "localResources")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new LocalResourcesTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => LocalResourcesSettings._settingProperties;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.LocalResourceSettings);

    [Setting("audioRedirection", DefaultValue = RdpClient.AudioCaptureRedirectionMode.DoNotRecord)]
    public EnumSetting<RdpClient.AudioRedirectionMode> AudioRedirectionMode { get; private set; }

    [Setting("audioRedirectionQuality", DefaultValue = RdpClient.AudioRedirectionQuality.Dynamic)]
    public EnumSetting<RdpClient.AudioRedirectionQuality> AudioRedirectionQuality { get; private set; }

    [Setting("audioCaptureRedirection", DefaultValue = RdpClient.AudioCaptureRedirectionMode.DoNotRecord)]
    public EnumSetting<RdpClient.AudioCaptureRedirectionMode> AudioCaptureRedirectionMode { get; private set; }

    [Setting("keyboardHook", DefaultValue = RdpClient.KeyboardHookMode.FullScreenClient)]
    public EnumSetting<RdpClient.KeyboardHookMode> KeyboardHookMode { get; private set; }

    [Setting("redirectClipboard", DefaultValue = true)]
    public BoolSetting RedirectClipboard { get; private set; }

    [Setting("redirectDrives")]
    public BoolSetting RedirectDrives { get; private set; }

    [Setting("redirectDrivesList")]
    public ListSetting<string> RedirectDrivesList { get; private set; }

    [Setting("redirectPrinters")]
    public BoolSetting RedirectPrinters { get; private set; }

    [Setting("redirectPorts")]
    public BoolSetting RedirectPorts { get; private set; }

    [Setting("redirectSmartCards", DefaultValue = true)]
    public BoolSetting RedirectSmartCards { get; private set; }

    [Setting("redirectPnpDevices")]
    public BoolSetting RedirectPnpDevices { get; private set; }
  }
}
