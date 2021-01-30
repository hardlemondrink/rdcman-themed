// Decompiled with JetBrains decompiler
// Type: RdcMan.EncryptionSettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public class EncryptionSettings : SettingsGroup
  {
    public const string TabName = "Encryption Settings";
    private static Dictionary<string, SettingProperty> _settingProperties;

    static EncryptionSettings()
    {
      typeof (EncryptionSettings).GetSettingProperties(out EncryptionSettings._settingProperties);
      EncryptionSettings._settingProperties[nameof (CredentialName)].Attribute.DefaultValue = (object) CredentialsUI.GetLoggedInUser();
    }

    public EncryptionSettings()
      : base("Encryption Settings", "encryptionSettings")
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new EncryptionSettingsTabPage(dialog, this);

    protected override Dictionary<string, SettingProperty> SettingProperties => EncryptionSettings._settingProperties;

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.EncryptionSettings);

    [Setting("encryptionMethod", DefaultValue = RdcMan.EncryptionMethod.LogonCredentials)]
    public EnumSetting<RdcMan.EncryptionMethod> EncryptionMethod { get; private set; }

    [Setting("credentialName")]
    public StringSetting CredentialName { get; private set; }

    [Setting("credentialData")]
    public StringSetting CredentialData { get; private set; }
  }
}
