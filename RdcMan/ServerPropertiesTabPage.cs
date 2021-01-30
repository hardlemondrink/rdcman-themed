// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerPropertiesTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerPropertiesTabPage : NodePropertiesPage<ServerSettings>
  {
    private RdcTextBox _serverNameTextBox;
    private RdcCheckBox _vmConsoleConnectCheckBox;
    private Label _vmIdLabel;
    private RdcTextBox _vmIdTextBox;
    private RdcTextBox _displayNameTextBox;
    private bool _displayNameUserCreated;

    public ServerPropertiesTabPage(TabbedSettingsDialog dialog, ServerSettings settings)
      : base(dialog, settings, "Server Settings")
    {
      int tabIndex1 = 0;
      int rowIndex1 = 0;
      this._serverNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&Server name:", this.Settings.ServerName, ref rowIndex1, ref tabIndex1);
      this._serverNameTextBox.Enabled = true;
      this._serverNameTextBox.TextChanged += new EventHandler(this.ServerNameChanged);
      this._serverNameTextBox.Validate = new Func<string>(this.ValidateServerName);
      int rowIndex2 = rowIndex1;
      int tabIndex2 = tabIndex1;
      int num1 = tabIndex2 + 1;
      this._vmConsoleConnectCheckBox = FormTools.NewCheckBox("&VM console connect", 0, rowIndex2, tabIndex2, 140);
      this._vmConsoleConnectCheckBox.CheckedChanged += new EventHandler(this.VMConsoleConnectCheckBox_CheckedChanged);
      Label label1 = new Label();
      Label label2 = label1;
      int rowIndex3 = rowIndex1;
      int rowIndex4 = rowIndex3 + 1;
      Point point = FormTools.NewLocation(1, rowIndex3);
      label2.Location = point;
      label1.Size = new Size(30, 20);
      label1.Text = "&id:";
      label1.TextAlign = ContentAlignment.MiddleLeft;
      label1.Visible = false;
      this._vmIdLabel = label1;
      RdcTextBox rdcTextBox1 = new RdcTextBox();
      rdcTextBox1.Location = new Point(this._vmIdLabel.Right, this._vmIdLabel.Top);
      rdcTextBox1.Setting = this.Settings.VirtualMachineId;
      rdcTextBox1.Size = new Size(340 - this._vmIdLabel.Width, 20);
      RdcTextBox rdcTextBox2 = rdcTextBox1;
      int num2 = num1;
      int tabIndex3 = num2 + 1;
      rdcTextBox2.TabIndex = num2;
      rdcTextBox1.Visible = false;
      this._vmIdTextBox = rdcTextBox1;
      this._displayNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&Display name:", this.Settings.DisplayName, ref rowIndex4, ref tabIndex3);
      this._displayNameTextBox.Enabled = true;
      this._displayNameTextBox.TextChanged += new EventHandler(this.DisplayNameChanged);
      this._displayNameTextBox.Validate = new Func<string>(this.ValidateDisplayName);
      this._displayNameUserCreated = !this.Settings.ServerName.Value.Equals(this.Settings.DisplayName.Value);
      this.AddParentCombo(ref rowIndex4, ref tabIndex3);
      this.AddComment(ref rowIndex4, ref tabIndex3).Setting = this.Settings.Comment;
      this.Controls.Add((Control) this._vmConsoleConnectCheckBox, (Control) this._vmIdLabel, (Control) this._vmIdTextBox);
      this.FocusControl = (Control) this._serverNameTextBox;
    }

    public List<string> ExpandedServerNames { get; private set; }

    protected override bool CanBeParent(GroupBase group) => group.CanAddServers();

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this._vmConsoleConnectCheckBox.Checked = this.Settings.ConnectionType.Value == ConnectionType.VirtualMachineConsoleConnect;
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      this.Settings.ConnectionType.Value = this._vmConsoleConnectCheckBox.Checked ? ConnectionType.VirtualMachineConsoleConnect : ConnectionType.Normal;
    }

    private string ValidateServerName()
    {
      this._serverNameTextBox.Text = this._serverNameTextBox.Text.Trim();
      string text = this._serverNameTextBox.Text;
      if (text.Length == 0)
        return "Please enter a server name";
      if (text.IndexOf(' ') != -1)
        return "Spaces are not permitted in a server name";
      if (text.IndexOf('/') == -1)
      {
        if (text.IndexOf('\\') == -1)
        {
          try
          {
            List<string> stringList = new List<string>(StringUtilities.ExpandPattern(text));
            if (stringList.Count > 1 && stringList.Count > 20 && FormTools.YesNoDialog("Expansion results in " + (object) stringList.Count + " servers. Are you sure?") == DialogResult.No)
              return "Expansion too large";
            this.ExpandedServerNames = stringList;
          }
          catch (ArgumentException ex)
          {
            return ex.Message;
          }
          return (string) null;
        }
      }
      return "Slashes are not permitted in a server name";
    }

    private void ServerNameChanged(object sender, EventArgs e)
    {
      if (this._displayNameUserCreated)
        return;
      string serverName;
      Server.SplitName(this._serverNameTextBox.Text, out serverName, out int _);
      this._displayNameTextBox.Text = serverName;
      this._displayNameUserCreated = false;
    }

    private string ValidateDisplayName()
    {
      this._displayNameTextBox.Text = this._displayNameTextBox.Text.Trim();
      return this._displayNameTextBox.Text.Length == 0 ? "Please enter a display name" : (string) null;
    }

    private void DisplayNameChanged(object sender, EventArgs e) => this._displayNameUserCreated = true;

    private void VMConsoleConnectCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      bool flag = this._vmConsoleConnectCheckBox.Checked;
      this._vmIdLabel.Visible = flag;
      this._vmIdTextBox.Visible = flag;
      this._vmIdTextBox.Enabled = flag;
      EnableTabsEventArgs args = new EnableTabsEventArgs()
      {
        Enabled = !flag,
        Reason = "for virtual machine console connect",
        TabNames = (IEnumerable<string>) new string[4]
        {
          "Local Resources",
          "Remote Desktop Settings",
          "Security Settings",
          "Connection Settings"
        }
      };
      (this.FindForm() as NodePropertiesDialog).EnableTabs(args);
    }
  }
}
