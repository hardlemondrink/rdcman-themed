// Decompiled with JetBrains decompiler
// Type: RdcMan.RecentlyUsedGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  [Export(typeof (IBuiltInVirtualGroup))]
  internal class RecentlyUsedGroup : BuiltInVirtualGroup<RecentlyUsedServerRef>, IServerRefFactory
  {
    public static RecentlyUsedGroup Instance { get; private set; }

    static RecentlyUsedGroup()
    {
      Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(RecentlyUsedGroup.Server_ConnectionStateChanged);
      Server.FocusReceived += new Action<Server>(RecentlyUsedGroup.Server_FocusReceived);
    }

    private static void Server_FocusReceived(Server server)
    {
      if (server.ConnectionState != RdpClient.ConnectionState.Connected)
        return;
      RecentlyUsedGroup.Instance.MoveToTop(server);
    }

    private static void Server_ConnectionStateChanged(ConnectionStateChangedEventArgs args)
    {
      if (args.State != RdpClient.ConnectionState.Connected)
        return;
      RecentlyUsedGroup.Instance.MoveToTop(args.Server);
    }

    public override RecentlyUsedServerRef AddReference(ServerBase serverBase)
    {
      Server server = serverBase.ServerNode;
      RecentlyUsedServerRef serverRef = server.FindServerRef<RecentlyUsedServerRef>();
      if (serverRef == null)
      {
        ServerTree.Instance.Operation(OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
        {
          serverRef = this.ServerRefFactory.Create(server) as RecentlyUsedServerRef;
          this.Nodes.Insert(0, (TreeNode) serverRef);
          this.RemoveExtra();
        }));
        ServerTree.Instance.OnGroupChanged((GroupBase) RecentlyUsedGroup.Instance, ChangeType.TreeChanged);
      }
      return serverRef;
    }

    private void RemoveExtra() => ServerTree.Instance.Operation(OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
    {
      int index = (this.Properties as RecentlyUsedGroup.RecentlyUsedSettings).MaxNumberOfServers.Value;
      while (this.Nodes.Count > index)
        ServerTree.Instance.RemoveNode(this.Nodes[index] as RdcTreeNode);
    }));

    private void MoveToTop(Server server)
    {
      ServerRef serverRef = (ServerRef) this.AddReference((ServerBase) server);
      if (serverRef.Index <= 0)
        return;
      ServerTree.Instance.Operation(OperationBehavior.RestoreSelected, (Action) (() =>
      {
        this.Nodes.Remove((TreeNode) serverRef);
        this.Nodes.Insert(0, (TreeNode) serverRef);
      }));
      ServerTree.Instance.OnGroupChanged((GroupBase) RecentlyUsedGroup.Instance, ChangeType.InvalidateUI);
    }

    private RecentlyUsedGroup()
    {
      this.Text = "Recent";
      RecentlyUsedGroup.Instance = this;
    }

    protected override void InitSettings()
    {
      this.Properties = (CommonNodeSettings) new RecentlyUsedGroup.RecentlyUsedSettings();
      this.AllSettingsGroups.Add((SettingsGroup) this.Properties);
    }

    public override string ConfigName => "RecentlyUsed";

    public override bool AllowSort => false;

    public override bool CanRemoveChildren() => false;

    public override bool HasProperties => true;

    public override void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (TabbedSettingsDialog dialog = new TabbedSettingsDialog("Recently Used Group Settings", "OK", parentForm))
      {
        dialog.AddTabPage(this.Properties.CreateTabPage(dialog));
        dialog.InitButtons();
        if (dialog.ShowDialog((IWin32Window) parentForm) != DialogResult.OK)
          return;
        dialog.UpdateSettings();
        this.RemoveExtra();
        ServerTree.Instance.OnGroupChanged((GroupBase) this, ChangeType.PropertyChanged);
      }
    }

    public ServerRef Create(Server server) => (ServerRef) new RecentlyUsedServerRef(server);

    protected override string XmlNodeName => "recentlyUsed";

    protected override bool ShouldWriteNode(RdcTreeNode node, FileGroup file) => file == null;

    private class RecentlyUsedSettings : GroupSettings
    {
      private new const string TabName = "Properties";
      private static Dictionary<string, SettingProperty> _settingProperties;

      static RecentlyUsedSettings() => typeof (RecentlyUsedGroup.RecentlyUsedSettings).GetSettingProperties(out RecentlyUsedGroup.RecentlyUsedSettings._settingProperties);

      public RecentlyUsedSettings()
        : base("Properties")
        => this.InheritSettingsType.Mode = InheritanceMode.Disabled;

      public override TabPage CreateTabPage(TabbedSettingsDialog dialog) => (TabPage) new RecentlyUsedGroup.RecentlyUsedSettings.RecentlyUsedSettingsTabPage(dialog, this);

      protected override Dictionary<string, SettingProperty> SettingProperties => RecentlyUsedGroup.RecentlyUsedSettings._settingProperties;

      [Setting("maxNumberOfServers", DefaultValue = 10)]
      public IntSetting MaxNumberOfServers { get; private set; }

      private class RecentlyUsedSettingsTabPage : 
        SettingsTabPage<RecentlyUsedGroup.RecentlyUsedSettings>
      {
        public RecentlyUsedSettingsTabPage(
          TabbedSettingsDialog dialog,
          RecentlyUsedGroup.RecentlyUsedSettings settings)
          : base(dialog, settings)
        {
          int rowIndex1 = 0;
          int num1 = 0;
          Label label = FormTools.NewLabel("Number of entries", 0, rowIndex1);
          NumericTextBox numericTextBox1 = new NumericTextBox(1, 20, "Number of entries must be 1 to 20");
          NumericTextBox numericTextBox2 = numericTextBox1;
          int rowIndex2 = rowIndex1;
          int num2 = rowIndex2 + 1;
          Point point = FormTools.NewLocation(1, rowIndex2);
          numericTextBox2.Location = point;
          NumericTextBox numericTextBox3 = numericTextBox1;
          int num3 = num1;
          int num4 = num3 + 1;
          numericTextBox3.TabIndex = num3;
          numericTextBox1.TabStop = true;
          numericTextBox1.Setting = this.Settings.MaxNumberOfServers;
          numericTextBox1.Size = new Size(20, 20);
          NumericTextBox numericTextBox4 = numericTextBox1;
          this.Controls.Add((Control) label, (Control) numericTextBox4);
          this.FocusControl = (Control) numericTextBox4;
        }
      }
    }
  }
}
