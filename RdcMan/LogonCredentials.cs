// Decompiled with JetBrains decompiler
// Type: RdcMan.LogonCredentials
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class LogonCredentials : SettingsGroup, ILogonCredentials
  {
    internal const string TabName = "Logon Credentials";
    public static readonly string GlobalStoreName = ProfileScope.Global.ToString();
    private static Dictionary<string, SettingProperty> _settingProperties;

    static LogonCredentials()
    {
      typeof (LogonCredentials).GetSettingProperties(out LogonCredentials._settingProperties);
      LogonCredentials._settingProperties["userName"].Attribute.DefaultValue = (object) Environment.UserName;
      LogonCredentials._settingProperties["domain"].Attribute.DefaultValue = (object) Environment.UserDomainName;
    }

    public LogonCredentials()
      : this("Logon Credentials", "logonCredentials")
    {
    }

    public LogonCredentials(string description, string xmlNodeName)
      : base(description, xmlNodeName)
    {
    }

    public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new LogonCredentialsTabPage(dialog, this);

    public bool DirectlyReferences(ILogonCredentials credentials) => this.InheritSettingsType.Mode == InheritanceMode.None && this.ProfileName.Scope == credentials.ProfileScope && this.ProfileName.Value == credentials.ProfileName;

    public static bool IsCustomProfile(string profileName) => string.Compare(profileName, "Custom", true) == 0;

    public static string ConstructQualifiedName(ILogonCredentials credentials) => LogonCredentials.IsCustomProfile(credentials.ProfileName) ? credentials.ProfileName : string.Format("{0} ({1})", (object) credentials.ProfileName, (object) credentials.ProfileScope);

    protected override Dictionary<string, SettingProperty> SettingProperties => LogonCredentials._settingProperties;

    protected override void WriteSettings(XmlTextWriter tw, RdcTreeNode node)
    {
      HashSet<ISetting> exclusionSet = new HashSet<ISetting>();
      if (this.ProfileName.Scope != ProfileScope.Local)
      {
        exclusionSet.Add((ISetting) this.UserName);
        exclusionSet.Add((ISetting) this.Password);
        exclusionSet.Add((ISetting) this.Domain);
      }
      this.WriteSettings(tw, node, exclusionSet);
    }

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.LogonCredentials);

    public void SetPassword(string clearTextPassword) => this.Password.SetPlainText(clearTextPassword);

    [Setting("profileName")]
    public ProfileSetting ProfileName { get; protected set; }

    [Setting("userName")]
    public StringSetting UserName { get; protected set; }

    [Setting("password")]
    internal PasswordSetting Password { get; set; }

    [Setting("domain")]
    public StringSetting Domain { get; protected set; }

    string ILogonCredentials.ProfileName => this.ProfileName.Value;

    ProfileScope ILogonCredentials.ProfileScope => this.ProfileName.Scope;

    string ILogonCredentials.UserName => this.UserName.Value;

    PasswordSetting ILogonCredentials.Password => this.Password;

    string ILogonCredentials.Domain => this.Domain.Value;
  }
}
