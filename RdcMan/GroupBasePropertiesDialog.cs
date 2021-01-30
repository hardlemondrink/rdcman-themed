// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupBasePropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RdcMan
{
  internal abstract class GroupBasePropertiesDialog : NodePropertiesDialog
  {
    protected ListBox _credentialsListBox;
    private CredentialsStore _credentialsStore;
    private int _credentialsStoreChangeId;

    protected GroupBasePropertiesDialog(
      GroupBase group,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base((RdcTreeNode) group, dialogTitle, acceptButtonText, parentForm)
    {
    }

    protected int CreateProfileManagementTabPage()
    {
      int num1 = 0;
      TabPage page = FormTools.NewTabPage("Profile Management");
      int rowIndex1 = num1;
      int num2 = rowIndex1 + 1;
      Label label = FormTools.NewLabel("Profiles", 0, rowIndex1);
      ListBox listBox1 = new ListBox();
      ListBox listBox2 = listBox1;
      int rowIndex2 = num2;
      int num3 = rowIndex2 + 1;
      Point point = FormTools.NewLocation(0, rowIndex2);
      listBox2.Location = point;
      listBox1.Size = new Size(340, 200);
      this._credentialsListBox = listBox1;
      this._credentialsListBox.KeyDown += new KeyEventHandler(this.CredentialsListBox_KeyDown);
      this._credentialsListBox.DoubleClick += new EventHandler(this.EditButton_Click);
      this._credentialsListBox.VisibleChanged += new EventHandler(this.CredentialsListBox_VisibleChanged);
      Button button1 = new Button();
      button1.Text = "&Add";
      button1.Location = new Point(this._credentialsListBox.Right + 20, this._credentialsListBox.Top);
      Button button2 = button1;
      button2.Click += new EventHandler(this.AddButton_Click);
      Button button3 = new Button();
      button3.Text = "&Edit";
      button3.Location = new Point(this._credentialsListBox.Right + 20, button2.Bottom + 4);
      Button button4 = button3;
      button4.Click += new EventHandler(this.EditButton_Click);
      Button button5 = new Button();
      button5.Text = "&Delete";
      button5.Location = new Point(this._credentialsListBox.Right + 20, button4.Bottom + 4);
      Button button6 = button5;
      button6.Click += new EventHandler(this.DeleteButton_Click);
      page.Controls.Add((Control) label, (Control) this._credentialsListBox, (Control) button2, (Control) button4, (Control) button6);
      page.ResumeLayout();
      this.AddTabPage(page);
      return num3;
    }

    private void CredentialsListBox_VisibleChanged(object sender, EventArgs e) => this.PopulateCredentialsListBoxIfChanged();

    private void PopulateCredentialsListBoxIfChanged()
    {
      if (this._credentialsStoreChangeId == this._credentialsStore.ChangeId)
        return;
      this.PopulateCredentialsListBox();
    }

    protected void PopulateCredentialsManagementTab(CredentialsStore store)
    {
      this._credentialsStore = store;
      this.PopulateCredentialsListBox();
    }

    private void PopulateCredentialsListBox()
    {
      this._credentialsListBox.Items.Clear();
      foreach (object profile in this._credentialsStore.Profiles)
        this._credentialsListBox.Items.Add(profile);
      this._credentialsStoreChangeId = this._credentialsStore.ChangeId;
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
      using (AddCredentialsDialog credentialsDialog = new AddCredentialsDialog(this.AssociatedNode))
      {
        if (credentialsDialog.ShowDialog() != DialogResult.OK)
          return;
        CredentialsProfile credentialsProfile = new CredentialsProfile(credentialsDialog.ProfileName, credentialsDialog.ProfileScope, credentialsDialog.UserName, credentialsDialog.Password.Value, credentialsDialog.Domain);
        if (!this._credentialsStore.Contains(credentialsProfile.ProfileName))
          this._credentialsListBox.Items.Add((object) credentialsProfile);
        this._credentialsStore[credentialsProfile.ProfileName] = credentialsProfile;
      }
    }

    private void EditButton_Click(object sender, EventArgs e)
    {
      int selectedIndex = this._credentialsListBox.SelectedIndex;
      if (selectedIndex == -1)
        return;
      CredentialsProfile credentials = this._credentialsListBox.Items[selectedIndex] as CredentialsProfile;
      using (LogonSettingsDialog logonSettingsDialog = LogonSettingsDialog.NewEditCredentialsDialog(credentials))
      {
        if (logonSettingsDialog.ShowDialog() != DialogResult.OK)
          return;
        PasswordSetting password = logonSettingsDialog.PasswordChanged ? logonSettingsDialog.Password : credentials.Password;
        CredentialsProfile credentialsProfile = new CredentialsProfile(credentials.ProfileName, credentials.ProfileScope, logonSettingsDialog.UserName, password, logonSettingsDialog.Domain);
        this._credentialsStore[credentialsProfile.ProfileName] = credentialsProfile;
        this._credentialsListBox.Items[selectedIndex] = (object) credentialsProfile;
      }
    }

    private void DeleteButton_Click(object sender, EventArgs e) => this.DeleteCredentials();

    private void CredentialsListBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Delete)
        return;
      e.Handled = true;
      this.DeleteCredentials();
    }

    private void DeleteCredentials()
    {
      int selectedIndex = this._credentialsListBox.SelectedIndex;
      if (selectedIndex == -1)
        return;
      CredentialsProfile credentials = this._credentialsListBox.Items[selectedIndex] as CredentialsProfile;
      ICollection<string> credentialsInUseLocations = this.GetCredentialsInUseLocations(credentials);
      if (credentialsInUseLocations.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(credentials.ProfileName + " is in use in these nodes:").AppendLine();
        foreach (string str in (IEnumerable<string>) credentialsInUseLocations)
          stringBuilder.AppendLine(str);
        stringBuilder.AppendLine().AppendLine("Are you sure you want to delete it?");
        if (FormTools.YesNoDialog(stringBuilder.ToString()) != DialogResult.Yes)
          return;
      }
      this._credentialsStore.Remove(credentials.ProfileName);
      this._credentialsListBox.Items.RemoveAt(selectedIndex);
      this.RevertDeletedCredentials(credentials);
    }

    private ICollection<string> GetCredentialsInUseLocations(
      CredentialsProfile credentials)
    {
      HashSet<string> inUseLocations = new HashSet<string>();
      ICollection nodes;
      if (this.AssociatedNode.FileGroup != null)
        nodes = (ICollection) new FileGroup[1]
        {
          this.AssociatedNode.FileGroup
        };
      else
        nodes = (ICollection) ServerTree.Instance.Nodes;
      foreach (TabPage tabPage in this.TabPages)
      {
        if (tabPage is ICredentialsTabPage credentialsTabPage && credentialsTabPage.Credentials == credentials)
          inUseLocations.Add("{0}.{1}".InvariantFormat((object) this.Text, (object) tabPage.Text));
      }
      nodes.VisitNodes((Func<RdcTreeNode, NodeVisitorResult>) (node =>
      {
        if (node is VirtualGroup)
          return NodeVisitorResult.NoRecurse;
        if (node.LogonCredentials.DirectlyReferences((ILogonCredentials) credentials))
          inUseLocations.Add("{0}.{1}".InvariantFormat((object) node.FullPath, (object) "Logon Credentials"));
        if (node.GatewaySettings.DirectlyReferences((ILogonCredentials) credentials))
          inUseLocations.Add("{0}.{1}".InvariantFormat((object) node.FullPath, (object) "Gateway Settings"));
        return NodeVisitorResult.Continue;
      }));
      return (ICollection<string>) inUseLocations;
    }

    private void RevertDeletedCredentials(CredentialsProfile credentials)
    {
      ICollection nodes;
      if (this.AssociatedNode.FileGroup != null)
        nodes = (ICollection) new FileGroup[1]
        {
          this.AssociatedNode.FileGroup
        };
      else
        nodes = (ICollection) ServerTree.Instance.Nodes;
      nodes.VisitNodes((Func<RdcTreeNode, NodeVisitorResult>) (node =>
      {
        if (node is VirtualGroup)
          return NodeVisitorResult.NoRecurse;
        if (node.LogonCredentials.DirectlyReferences((ILogonCredentials) credentials))
          node.LogonCredentials.ProfileName.Reset();
        if (node.GatewaySettings.DirectlyReferences((ILogonCredentials) credentials))
          node.GatewaySettings.ProfileName.Reset();
        return NodeVisitorResult.Continue;
      }));
    }
  }
}
