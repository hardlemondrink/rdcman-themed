// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcTreeNode
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public abstract class RdcTreeNode : TreeNode, ILogonCredentials
  {
    public const string PropertiesXmlNodeName = "properties";
    private bool _needToUpdateInheritedSettings = true;

    public string ProfileName => this.LogonCredentials.ProfileName.Value;

    public ProfileScope ProfileScope => this.LogonCredentials.ProfileName.Scope;

    public string UserName => this.LogonCredentials.UserName.Value;

    public PasswordSetting Password => this.LogonCredentials.Password;

    public string Domain => this.LogonCredentials.Domain.Value;

    public virtual RdcBaseForm ParentForm => (RdcBaseForm) Program.TheForm;

    public CommonNodeSettings Properties { get; set; }

    public LogonCredentials LogonCredentials { get; private set; }

    public ConnectionSettings ConnectionSettings { get; private set; }

    public GatewaySettings GatewaySettings { get; private set; }

    public RemoteDesktopSettings RemoteDesktopSettings { get; private set; }

    public LocalResourcesSettings LocalResourceSettings { get; private set; }

    public CommonDisplaySettings DisplaySettings { get; set; }

    public SecuritySettings SecuritySettings { get; private set; }

    public virtual EncryptionSettings EncryptionSettings
    {
      get => this.FileGroup.EncryptionSettings;
      protected set => throw new NotImplementedException();
    }

    internal GroupBase GetReadOnlyParent()
    {
      TreeNode treeNode = (TreeNode) this;
      while (!(treeNode is GroupBase groupBase) || !groupBase.IsReadOnly)
      {
        treeNode = treeNode.Parent;
        if (treeNode == null)
          return (GroupBase) null;
      }
      return groupBase;
    }

    protected List<SettingsGroup> AllSettingsGroups { get; private set; }

    public virtual FileGroup FileGroup
    {
      get
      {
        TreeNode treeNode = (TreeNode) this;
        while (treeNode.Parent != null)
          treeNode = treeNode.Parent;
        return treeNode as FileGroup;
      }
    }

    public List<TreeNode> GetPath()
    {
      List<TreeNode> list = new List<TreeNode>();
      this.VisitNodeAndParents((Action<RdcTreeNode>) (node => list.Insert(0, (TreeNode) node)));
      return list;
    }

    public int GetPathLength()
    {
      int len = 0;
      this.VisitNodeAndParents((Action<RdcTreeNode>) (n => ++len));
      return len;
    }

    public string ParentPath => this.Parent == null ? ServerTree.Instance.RootNode.Text : this.Parent.FullPath;

    protected RdcTreeNode()
    {
      this.AllSettingsGroups = new List<SettingsGroup>();
      this.InitSettings();
    }

    protected virtual void InitSettings()
    {
      this.LogonCredentials = new LogonCredentials();
      this.ConnectionSettings = new ConnectionSettings();
      this.GatewaySettings = new GatewaySettings();
      this.RemoteDesktopSettings = new RemoteDesktopSettings();
      this.LocalResourceSettings = new LocalResourcesSettings();
      this.SecuritySettings = new SecuritySettings();
      if (this.Properties != null)
        this.AllSettingsGroups.Add((SettingsGroup) this.Properties);
      this.AllSettingsGroups.AddRange((IEnumerable<SettingsGroup>) new SettingsGroup[7]
      {
        (SettingsGroup) this.LogonCredentials,
        (SettingsGroup) this.ConnectionSettings,
        (SettingsGroup) this.GatewaySettings,
        (SettingsGroup) this.RemoteDesktopSettings,
        (SettingsGroup) this.LocalResourceSettings,
        (SettingsGroup) this.DisplaySettings,
        (SettingsGroup) this.SecuritySettings
      });
    }

    internal void CopySettings(RdcTreeNode node, System.Type excludeType)
    {
      for (int index = 0; index < this.AllSettingsGroups.Count; ++index)
      {
        if (!(this.AllSettingsGroups[index].GetType() == excludeType))
        {
          this.AllSettingsGroups[index].InheritSettingsType.Mode = node.AllSettingsGroups[index].InheritSettingsType.Mode;
          this.AllSettingsGroups[index].Copy(node.AllSettingsGroups[index]);
        }
      }
    }

    internal SettingsGroup GetSettingsGroupByName(string name) => this.AllSettingsGroups.Where<SettingsGroup>((Func<SettingsGroup, bool>) (sg => sg.Name.Equals(name))).FirstOrDefault<SettingsGroup>();

    internal abstract void WriteXml(XmlTextWriter tw);

    protected void WriteXmlSettingsGroups(XmlTextWriter tw)
    {
      foreach (SettingsGroup allSettingsGroup in this.AllSettingsGroups)
        allSettingsGroup.WriteXml(tw, this);
    }

    protected void ReadXml(
      Dictionary<string, Helpers.ReadXmlDelegate> nodeActions,
      XmlNode xmlNode,
      ICollection<string> errors)
    {
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        Helpers.ReadXmlDelegate readXmlDelegate;
        nodeActions.TryGetValue(childNode.Name, out readXmlDelegate);
        try
        {
          if (readXmlDelegate != null)
            readXmlDelegate(childNode, this, errors);
          else
            this.ReadXmlSettingsGroup(childNode, errors);
        }
        catch (Exception ex)
        {
          errors.Add("Exception reading Xml node {0} in '{1}': {2}".InvariantFormat((object) childNode.GetFullPath(), (object) this.Text, (object) ex.Message));
        }
      }
    }

    protected void ReadXmlSettingsGroup(XmlNode xmlNode, ICollection<string> errors)
    {
      SettingsGroup settingsGroup = this.AllSettingsGroups.Where<SettingsGroup>((Func<SettingsGroup, bool>) (i => xmlNode.Name == i.XmlNodeName)).FirstOrDefault<SettingsGroup>();
      if (settingsGroup != null)
        settingsGroup.ReadXml(xmlNode, this, errors);
      else
        errors.Add("Unexpected Xml node {0} in '{1}'".InvariantFormat((object) xmlNode.GetFullPath(), (object) this.Text));
    }

    internal virtual void UpdateSettings(NodePropertiesDialog dlg)
    {
      if (dlg == null)
        return;
      dlg.UpdateSettings();
      if (this.TreeView == null || dlg.PropertiesPage.ParentGroup == null || this.Parent == dlg.PropertiesPage.ParentGroup)
        return;
      ServerTree.Instance.MoveNode(this, dlg.PropertiesPage.ParentGroup);
    }

    internal abstract void Show();

    internal abstract void Hide();

    public abstract void Connect();

    public abstract void ConnectAs(
      LogonCredentials logonSettings,
      ConnectionSettings connectionSettings);

    public abstract void Reconnect();

    public abstract void Disconnect();

    public abstract void LogOff();

    public abstract void OnRemoving();

    public void DoPropertiesDialog() => this.DoPropertiesDialog((Form) null, (string) null);

    public abstract void DoPropertiesDialog(Form parentForm, string activeTabName);

    public virtual bool CanRemove(bool popUI) => this.AllowEdit(popUI);

    public abstract bool ConfirmRemove(bool askUser);

    public abstract bool CanDropOnTarget(RdcTreeNode targetNode);

    public virtual bool HasProperties => true;

    public virtual bool HandleMove(RdcTreeNode childNode) => false;

    public virtual void ChangeImageIndex(ImageConstants index)
    {
      this.ImageIndex = (int) index;
      this.SelectedImageIndex = (int) ServerTree.TranslateImage(index, true);
    }

    public virtual void CollectNodesToInvalidate(bool recurseChildren, HashSet<RdcTreeNode> set)
    {
      if (recurseChildren)
      {
        foreach (RdcTreeNode node in this.Nodes)
          node.CollectNodesToInvalidate(recurseChildren, set);
      }
      set.Add(this);
    }

    public void ResetInheritance() => this._needToUpdateInheritedSettings = true;

    public virtual void InvalidateNode() => this._needToUpdateInheritedSettings = true;

    public bool InheritSettings()
    {
      bool anyInherited = false;
      if (!this._needToUpdateInheritedSettings)
        return anyInherited;
      foreach (SettingsGroup allSettingsGroup in this.AllSettingsGroups)
        allSettingsGroup.InheritSettings(this, ref anyInherited);
      this._needToUpdateInheritedSettings = false;
      return anyInherited;
    }

    public void DoConnectAs()
    {
      RdcTreeNode node = this;
      if (node is ServerRef serverRef)
        node = (RdcTreeNode) serverRef.ServerNode;
      using (ConnectAsDialog connectAsDialog = ConnectAsDialog.NewConnectAsDialog(node, (Form) Program.TheForm))
      {
        if (connectAsDialog.ShowDialog() != DialogResult.OK)
          return;
        connectAsDialog.UpdateSettings();
        this.ConnectAs(connectAsDialog.LogonCredentials, connectAsDialog.ConnectionSettings);
      }
    }

    public virtual bool AllowEdit(bool popUI)
    {
      GroupBase readOnlyParent = this.GetReadOnlyParent();
      if (readOnlyParent == null)
        return true;
      if (popUI)
        FormTools.InformationDialog("{0} '{1}' is read-only and cannot be edited".CultureFormat(readOnlyParent == this ? (object) "Group" : (object) "Parent group", (object) readOnlyParent.Text));
      return false;
    }

    public CredentialsProfile LookupCredentialsProfile(
      ILogonCredentials logonCredentials)
    {
      CredentialsStore credentialsProfiles = Program.CredentialsProfiles;
      if (logonCredentials.ProfileScope == ProfileScope.File)
        credentialsProfiles = this.FileGroup.CredentialsProfiles;
      CredentialsProfile profile;
      credentialsProfiles.TryGetValue(logonCredentials.ProfileName, out profile);
      return profile;
    }

    internal void ResolveCredentials()
    {
      this.ResolveAndFixCredentials(this.LogonCredentials);
      this.ResolveAndFixCredentials((LogonCredentials) this.GatewaySettings);
    }

    internal bool ResolveCredentials(LogonCredentials logonCredentials)
    {
      if (logonCredentials.ProfileName.Scope == ProfileScope.Local)
      {
        if (!LogonCredentials.IsCustomProfile(logonCredentials.ProfileName.Value))
          logonCredentials.ProfileName.Value = "Custom";
        return true;
      }
      CredentialsProfile credentialsProfile = this.LookupCredentialsProfile((ILogonCredentials) logonCredentials);
      if (credentialsProfile == null)
        return false;
      logonCredentials.UserName.Value = credentialsProfile.UserName;
      logonCredentials.Domain.Value = credentialsProfile.Domain;
      if (credentialsProfile.IsDecrypted)
        logonCredentials.Password.SetPlainText(credentialsProfile.Password.Value);
      return true;
    }

    private void ResolveAndFixCredentials(LogonCredentials logonCredentials)
    {
      if (logonCredentials == null || this.ResolveCredentials(logonCredentials))
        return;
      logonCredentials.ProfileName.Reset();
    }
  }
}
