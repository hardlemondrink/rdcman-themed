// Decompiled with JetBrains decompiler
// Type: RdcMan.FileGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public class FileGroup : GroupBase
  {
    internal const string XmlNodeName = "file";
    protected new static Dictionary<string, Helpers.ReadXmlDelegate> NodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>((IDictionary<string, Helpers.ReadXmlDelegate>) GroupBase.NodeActions);

    public override EncryptionSettings EncryptionSettings { get; protected set; }

    static FileGroup()
    {
      FileGroup.NodeActions["credentialsProfiles"] = (Helpers.ReadXmlDelegate) ((childNode, parent, errors) => (parent as FileGroup).CredentialsProfiles.ReadXml(childNode, ProfileScope.File, parent, errors));
      ServerTree.Instance.GroupChanged += new Action<GroupChangedEventArgs>(FileGroup.OnGroupChanged);
      ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(FileGroup.OnServerChanged);
    }

    private static void OnServerChanged(ServerChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      FileGroup fileGroup = e.Server.FileGroup;
      if (fileGroup == null)
        return;
      fileGroup.HasChangedSinceWrite = true;
    }

    private static void OnGroupChanged(GroupChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      FileGroup fileGroup = e.Group.FileGroup;
      if (fileGroup == null)
        return;
      fileGroup.HasChangedSinceWrite = true;
    }

    internal FileGroup(string pathname)
    {
      this.Pathname = Path.GetFullPath(pathname);
      if (File.Exists(this.Pathname))
      {
        this.IsReadOnly = File.GetAttributes(this.Pathname).HasFlag((Enum) FileAttributes.ReadOnly);
      }
      else
      {
        this.Properties.GroupName.Value = Path.GetFileNameWithoutExtension(this.Pathname);
        this.Text = this.Properties.GroupName.Value;
      }
      this.ToolTipText = pathname;
      this.ChangeImageIndex(ImageConstants.File);
      this.CredentialsProfiles = new CredentialsStore();
      this.EncryptionSettings = new EncryptionSettings();
      this.AllSettingsGroups.Add((SettingsGroup) this.EncryptionSettings);
    }

    internal int SchemaVersion { get; set; }

    public string GetFilename() => Path.GetFileName(this.Pathname);

    public string GetDirectory() => Path.GetDirectoryName(this.Pathname);

    protected override void InitSettings()
    {
      this.Properties = (CommonNodeSettings) new FileGroupSettings();
      base.InitSettings();
    }

    internal override void ReadXml(XmlNode xmlNode, ICollection<string> errors)
    {
      this.ReadXml(FileGroup.NodeActions, xmlNode, errors);
      this.Text = this.Properties.GroupName.Value;
      if (!this.IsReadOnly)
        return;
      this.Text += " {RO}";
    }

    internal override void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("file");
      this.CredentialsProfiles.WriteXml(tw, (RdcTreeNode) this);
      base.WriteXml(tw);
      tw.WriteEndElement();
    }

    public override sealed bool ConfirmRemove(bool askUser)
    {
      FormTools.InformationDialog("Use the File menu to close the " + this.Text + " group");
      return false;
    }

    public override void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (FileGroupPropertiesDialog propertiesDialog = FileGroupPropertiesDialog.NewPropertiesDialog(this, parentForm))
      {
        propertiesDialog.SetActiveTab(activeTabName);
        if (propertiesDialog.ShowDialog() != DialogResult.OK)
          return;
        this.UpdateSettings((NodePropertiesDialog) propertiesDialog);
        ServerTree.Instance.OnNodeChanged((RdcTreeNode) this, ChangeType.PropertyChanged);
      }
    }

    internal void CheckCredentials()
    {
      Dictionary<string, List<string>> missingProfiles = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.VisitNodes((Action<RdcTreeNode>) (node =>
      {
        this.CheckCredentials(node, node.LogonCredentials, "Logon Credentials", missingProfiles);
        this.CheckCredentials(node, (LogonCredentials) node.GatewaySettings, "Gateway Settings", missingProfiles);
      }));
      if (missingProfiles.Count <= 0)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Some credential profiles were not found.").AppendLine("Please add the missing profiles before connecting to servers, edit properties, etc. otherwise the reference will be lost.").AppendLine("Click OK to copy the details to the clipboard.").AppendLine();
      foreach (KeyValuePair<string, List<string>> keyValuePair in missingProfiles)
      {
        stringBuilder.AppendLine("Profile name: " + keyValuePair.Key);
        stringBuilder.AppendFormat("Referenced by: ");
        foreach (string str in keyValuePair.Value)
          stringBuilder.Append(" " + str);
        stringBuilder.AppendLine().AppendLine();
      }
      if (FormTools.ExclamationDialog(stringBuilder.ToString(), MessageBoxButtons.OKCancel) != DialogResult.OK)
        return;
      Clipboard.SetText(stringBuilder.ToString());
    }

    private void CheckCredentials(
      RdcTreeNode node,
      LogonCredentials credentials,
      string name,
      Dictionary<string, List<string>> missingProfiles)
    {
      if (credentials == null || credentials.InheritSettingsType.Mode == InheritanceMode.FromParent || node.ResolveCredentials(credentials))
        return;
      string key = LogonCredentials.ConstructQualifiedName((ILogonCredentials) credentials);
      List<string> stringList;
      if (!missingProfiles.TryGetValue(key, out stringList))
        stringList = missingProfiles[key] = new List<string>();
      stringList.Add(string.Format("{0}.{1}", (object) node.FullPath, (object) name));
    }

    public CredentialsStore CredentialsProfiles { get; private set; }

    public bool HasChangedSinceWrite { get; set; }

    public string Pathname { get; set; }
  }
}
