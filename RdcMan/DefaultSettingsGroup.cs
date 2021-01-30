// Decompiled with JetBrains decompiler
// Type: RdcMan.DefaultSettingsGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  internal class DefaultSettingsGroup : VirtualGroup
  {
    public const string Name = "Default settings group";
    private const string XmlTopNode = "defaultSettings";

    public static DefaultSettingsGroup Instance { get; private set; }

    static DefaultSettingsGroup() => DefaultSettingsGroup.Instance = new DefaultSettingsGroup();

    private DefaultSettingsGroup()
    {
      this.EncryptionSettings = new EncryptionSettings();
      this.AllSettingsGroups.Add((SettingsGroup) this.EncryptionSettings);
      foreach (SettingsGroup allSettingsGroup in this.AllSettingsGroups)
        allSettingsGroup.InheritSettingsType.Mode = InheritanceMode.Disabled;
    }

    public override EncryptionSettings EncryptionSettings { get; protected set; }

    internal override void ReadXml(XmlNode node, ICollection<string> errors)
    {
      if (!node.Name.Equals("defaultSettings"))
      {
        errors.Add("Default settings group malformed");
      }
      else
      {
        foreach (XmlNode childNode in node.ChildNodes)
          this.ReadXmlSettingsGroup(childNode, errors);
      }
    }

    internal override void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("defaultSettings");
      this.WriteXmlSettingsGroups(tw);
      tw.WriteEndElement();
    }

    public override void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (DefaultGroupPropertiesDialog propertiesDialog = DefaultGroupPropertiesDialog.NewPropertiesDialog((GroupBase) this, parentForm))
      {
        propertiesDialog.SetActiveTab(activeTabName);
        if (propertiesDialog.ShowDialog() != DialogResult.OK)
          return;
        this.UpdateSettings((NodePropertiesDialog) propertiesDialog);
        ServerTree.Instance.OnGroupChanged(ServerTree.Instance.RootNode, ChangeType.PropertyChanged);
        Program.Preferences.NeedToSave = true;
      }
    }
  }
}
