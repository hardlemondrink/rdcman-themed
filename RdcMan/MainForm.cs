// Decompiled with JetBrains decompiler
// Type: RdcMan.MainForm
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  internal class MainForm : RdcBaseForm, IMainForm
  {
    private const int MinimumRemoteDesktopPanelHeight = 200;
    private const int AutoHideIntervalInMilliseconds = 250;
    private readonly Dictionary<Keys, Action> Shortcuts;
    private readonly Dictionary<Keys, Action<RdcTreeNode>> SelectedNodeShortcuts;
    private ToolStripMenuItem _fileSaveMenuItem;
    private ToolStripMenuItem _fileSaveAsMenuItem;
    private ToolStripMenuItem _fileCloseMenuItem;
    private ToolStripMenuItem _editPropertiesMenuItem;
    private ToolStripMenuItem _sessionConnectMenuItem;
    private ToolStripMenuItem _sessionConnectAsMenuItem;
    private ToolStripMenuItem _sessionReconnectMenuItem;
    private ToolStripMenuItem _sessionSendKeysMenuItem;
    private ToolStripMenuItem _sessionRemoteActionsMenuItem;
    private ToolStripMenuItem _sessionDisconnectMenuItem;
    private ToolStripMenuItem _sessionLogOffMenuItem;
    private ToolStripMenuItem _sessionListSessionsMenuItem;
    private ToolStripMenuItem _sessionFullScreenMenuItem;
    private ToolStripMenuItem _sessionUndockMenuItem;
    private ToolStripMenuItem _sessionUndockAndConnectMenuItem;
    private ToolStripMenuItem _sessionDockMenuItem;
    private ToolStripMenuItem _sessionScreenCaptureMenuItem;
    private Splitter _treeSplitter;
    private bool _allowSizeChanged;
    private Panel _remoteDesktopPanel;
    private Timer _autoSaveTimer;
    private ClientPanel _clientPanel;
    private Server _fullScreenServer;
    private Control[] _savedControls;
    private bool _areShuttingDown;
    private Timer _serverTreeAutoHideTimer;

    private MainForm()
    {
      this.Shortcuts = new Dictionary<Keys, Action>()
      {
        {
          Keys.N | Keys.Control,
          (Action) (() => this.OnFileNew())
        },
        {
          Keys.O | Keys.Control,
          (Action) (() => this.OnFileOpen())
        },
        {
          Keys.S | Keys.Control,
          (Action) (() => this.OnFileSave())
        },
        {
          Keys.A | Keys.Control,
          (Action) (() => AddNodeDialogHelper.AddServersDialog())
        },
        {
          Keys.G | Keys.Control,
          (Action) (() => AddNodeDialogHelper.AddGroupDialog())
        },
        {
          Keys.F | Keys.Control,
          (Action) (() => MenuHelper.FindServers())
        },
        {
          Keys.Q | Keys.Control,
          (Action) (() => MenuHelper.ConnectTo())
        }
      };
      this.SelectedNodeShortcuts = new Dictionary<Keys, Action<RdcTreeNode>>()
      {
        {
          Keys.Delete,
          (Action<RdcTreeNode>) (node => ServerTree.Instance.ConfirmRemove(node, true))
        },
        {
          Keys.Delete | Keys.Shift,
          (Action<RdcTreeNode>) (node => ServerTree.Instance.ConfirmRemove(node, false))
        },
        {
          Keys.Return,
          (Action<RdcTreeNode>) (node => node.Connect())
        },
        {
          Keys.Return | Keys.Shift,
          (Action<RdcTreeNode>) (node =>
          {
            bool allConnected;
            if (node is ServerBase serverBase)
              allConnected = serverBase.IsConnected;
            else
              (node as GroupBase).AnyOrAllConnected(out bool _, out allConnected);
            if (allConnected)
              return;
            node.DoConnectAs();
          })
        },
        {
          Keys.Return | Keys.Alt,
          (Action<RdcTreeNode>) (node =>
          {
            if (!node.HasProperties)
              return;
            node.DoPropertiesDialog();
          })
        },
        {
          Keys.D | Keys.Control,
          (Action<RdcTreeNode>) (node => MenuHelper.AddFavorite(node))
        }
      };
    }

    public static MainForm Create()
    {
      MainForm mainForm = new MainForm();
      Program.TheForm = mainForm;
      return mainForm.Initialize() ? mainForm : (MainForm) null;
    }

    public bool IsFullScreen => this._fullScreenServer != null;

    public string DescriptionText { get; private set; }

    public string VersionText { get; private set; }

    public string BuildText { get; private set; }

    public bool IsInternalVersion { get; private set; }

    private bool Initialize()
    {
      this._allowSizeChanged = true;
      this.SuspendLayout();
      this.InitComp();
      try
      {
        RdpClient.Initialize(this);
      }
      catch
      {
        FormTools.ErrorDialog("RDCMan encountered an error during initialization. There are two likely causes for this: an incompatible version of mstscax.dll is registered or it is not registered at all. Please see the help file for details.");
        return false;
      }
      this.CreateMainMenu();
      this.SetMainMenuVisibility();
      this.SetTitle();
      this.VisibleChanged += new EventHandler(this.OnVisibleChanged);
      this.ScaleAndLayout();
      return true;
    }

    public void SetTitle()
    {
      TreeNode selectedNode = ServerTree.Instance.SelectedNode;
      string str = this.DescriptionText + " v" + this.VersionText;
      if (selectedNode != null)
        str = (!(selectedNode is ServerBase serverBase) ? selectedNode.Text : serverBase.ServerNode.GetQualifiedNameForUI()) + " - " + str;
      this.Text = str;
    }

    public void RecordLastFocusedServerLabel(ServerLabel label) => this._clientPanel.RecordLastFocusedServerLabel(label);

    public void AddToClientPanel(Control client) => this._clientPanel.Controls.Add(client);

    public void RemoveFromClientPanel(Control client) => this._clientPanel.Controls.Remove(client);

    public void GoToServerTree()
    {
      Program.TheForm.LeaveFullScreen();
      Program.TheForm.EnsureServerTreeVisible();
      ServerTree.Instance.Focus();
    }

    public override void GoFullScreenClient(Server server, bool isTopMostWindow)
    {
      if (this.IsFullScreen)
        return;
      this.LockWindowSize(false);
      this._fullScreenServer = server;
      this.RemoveFromClientPanel(server.Client.Control);
      this._savedControls = new Control[this.Controls.Count];
      this.Controls.CopyTo((Array) this._savedControls, 0);
      this.Controls.Clear();
      this.Controls.Add(server.Client.Control);
      base.GoFullScreenClient(server, isTopMostWindow);
    }

    public override bool SwitchFullScreenClient(Server server)
    {
      if (!this.IsFullScreen || !server.IsClientDocked)
        return false;
      if (server == this._fullScreenServer)
        return true;
      RdpClient client1 = this._fullScreenServer.Client;
      RdpClient client2 = server.Client;
      this._fullScreenServer.SuspendFullScreenBehavior();
      client1.MsRdpClient.FullScreen = false;
      this._fullScreenServer.ResumeFullScreenBehavior();
      server.SuspendFullScreenBehavior();
      server.SetNormalView();
      this.RemoveFromClientPanel(client2.Control);
      this.Controls.Add(client2.Control);
      client2.MsRdpClient.FullScreen = true;
      server.ResumeFullScreenBehavior();
      client2.Control.Bounds = new Rectangle(0, 0, client1.Control.Width, client1.Control.Height);
      server.GoFullScreen();
      client2.Control.Show();
      client1.Control.Hide();
      this._fullScreenServer.LeaveFullScreen();
      this.Controls.Remove(client1.Control);
      this.AddToClientPanel(client1.Control);
      this._fullScreenServer = server;
      return true;
    }

    public void LeaveFullScreen()
    {
      if (this._fullScreenServer == null)
        return;
      this._fullScreenServer.LeaveFullScreen();
    }

    public override void LeaveFullScreenClient(Server server)
    {
      if (!this.IsFullScreen)
        return;
      this.Controls.Clear();
      this.Controls.AddRange(this._savedControls);
      this._savedControls = (Control[]) null;
      this.AddToClientPanel(server.Client.Control);
      base.LeaveFullScreenClient(server);
      this._fullScreenServer = (Server) null;
      this.LockWindowSize();
    }

    public void EnsureServerTreeVisible() => this.UpdateServerTreeVisibility(ControlVisibility.Dock);

    private void InitComp()
    {
      bool lockWindowSize = Program.Preferences.LockWindowSize;
      Program.Preferences.LockWindowSize = false;
      Panel panel = new Panel();
      panel.Dock = DockStyle.None;
      this._remoteDesktopPanel = panel;
      ServerTree.Instance.HideSelection = false;
      ServerTree.Instance.Name = "ServerTree";
      ServerTree.Instance.TabIndex = 0;
      ServerTree.Instance.MouseLeave += new EventHandler(this.SetAutoHideServerTreeTimer);
      ServerTree.Instance.Leave += new EventHandler(this.SetAutoHideServerTreeTimer);
      ServerTree.Instance.LostFocus += new EventHandler(this.AutoHideServerTree);
      ServerTree.Instance.AfterSelect += new TreeViewEventHandler(this.ServerTree_AfterSelect);
      Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(this.Server_ConnectionStateChange);
      Splitter splitter = new Splitter();
      splitter.Dock = DockStyle.Left;
      splitter.Width = 4;
      splitter.MinSize = 10;
      splitter.MinExtra = 100;
      this._treeSplitter = splitter;
      this._treeSplitter.MouseHover += new EventHandler(this.SetAutoShowServerTreeTimer);
      this._treeSplitter.MouseLeave += new EventHandler(this.DisableAutoShowTimer);
      this._clientPanel = new ClientPanel();
      this._remoteDesktopPanel.Controls.Add((Control) ServerTree.Instance, (Control) this._treeSplitter, (Control) this._clientPanel);
      this._autoSaveTimer = new Timer();
      this._autoSaveTimer.Tick += new EventHandler(this.AutoSaveTimerTickHandler);
      this.SetMainMenuVisibility();
      if (Program.Preferences.ServerTreeWidth > Screen.PrimaryScreen.Bounds.Width)
        ServerTree.Instance.Width = 200;
      else
        ServerTree.Instance.Width = Program.Preferences.ServerTreeWidth;
      this.ServerTreeLocation = Program.Preferences.ServerTreeLocation;
      this.ServerTreeVisibility = Program.Preferences.ServerTreeVisibility;
      if (!Program.Preferences.WindowPosition.IsEmpty && Screen.FromPoint(Program.Preferences.WindowPosition).Bounds.Contains(Program.Preferences.WindowPosition))
      {
        this.StartPosition = FormStartPosition.Manual;
        this.Location = Program.Preferences.WindowPosition;
      }
      this.Controls.Add((Control) this._remoteDesktopPanel);
      this.Size = Program.Preferences.WindowSize;
      if (Program.Preferences.WindowIsMaximized)
        this.WindowState = FormWindowState.Maximized;
      Program.Preferences.LockWindowSize = lockWindowSize;
      this.LockWindowSize();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      this.Icon = new Icon(executingAssembly.GetManifestResourceStream("RdcMan.Resources.app.ico"));
      AssemblyName name = executingAssembly.GetName();
      this.VersionText = name.Version.Major.ToString() + "." + (object) name.Version.Minor;
      this.BuildText = name.Version.Build.ToString() + "." + (object) name.Version.Revision;
      if (!WinTrust.VerifyEmbeddedSignature(executingAssembly.Location))
      {
        this.BuildText += "    FOR INTERNAL MICROSOFT USE ONLY";
        this.IsInternalVersion = true;
      }
      object[] customAttributes = executingAssembly.GetCustomAttributes(typeof (AssemblyConfigurationAttribute), false);
      if (customAttributes.Length > 0)
        this.VersionText += (customAttributes[0] as AssemblyConfigurationAttribute).Configuration;
      this.DescriptionText = (executingAssembly.GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute).Description;
      ServerTree.Instance.Init(executingAssembly);
      this._serverTreeAutoHideTimer = new Timer();
      this._serverTreeAutoHideTimer.Tick += new EventHandler(this.ServerTreeAutoHideTimerTick);
    }

    private void Server_ConnectionStateChange(ConnectionStateChangedEventArgs args)
    {
    }

    private void ServerTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
    }

    protected override void LayoutContent()
    {
      int width = this.ClientSize.Width;
      int height1 = this.ClientSize.Height;
      int y = Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height;
      this._menuPanel.Width = width;
      int height2 = height1 - y;
      ServerTree.Instance.Height = height2;
      this._remoteDesktopPanel.Bounds = new Rectangle(0, y, width, height2);
    }

    private void AutoSaveTimerTickHandler(object sender, EventArgs e)
    {
      if (this._areShuttingDown)
        return;
      RdgFile.AutoSave();
      Program.Preferences.Save();
    }

    private void ServerTreeAutoHideTimerTick(object sender, EventArgs e)
    {
      if (Program.Preferences.ServerTreeVisibility == ControlVisibility.AutoHide)
      {
        if (!ServerTree.Instance.Visible)
        {
          this._serverTreeAutoHideTimer.Stop();
          this.UpdateServerTreeVisibility(ControlVisibility.Dock);
        }
        else
        {
          Rectangle screen = ServerTree.Instance.RectangleToScreen(ServerTree.Instance.ClientRectangle);
          screen.Inflate(48, 48);
          if (screen.Contains(Control.MousePosition))
            return;
          this._serverTreeAutoHideTimer.Stop();
          this.UpdateServerTreeVisibility(ControlVisibility.Hide);
        }
      }
      else
        this._serverTreeAutoHideTimer.Stop();
    }

    private void AutoHideServerTree(object sender, EventArgs e)
    {
      if (Program.Preferences.ServerTreeVisibility == ControlVisibility.Dock || !ServerTree.Instance.Visible)
        return;
      this._serverTreeAutoHideTimer.Stop();
      this.UpdateServerTreeVisibility(ControlVisibility.Hide);
    }

    private void SetAutoShowServerTreeTimer(object sender, EventArgs e)
    {
      if (Program.Preferences.ServerTreeVisibility != ControlVisibility.AutoHide || ServerTree.Instance.Visible)
        return;
      this._serverTreeAutoHideTimer.Interval = Program.Preferences.ServerTreeAutoHidePopUpDelay + 1;
      this._serverTreeAutoHideTimer.Start();
    }

    private void DisableAutoShowTimer(object sender, EventArgs e)
    {
      if (Program.Preferences.ServerTreeVisibility != ControlVisibility.AutoHide || ServerTree.Instance.Visible)
        return;
      this._serverTreeAutoHideTimer.Stop();
    }

    private void SetAutoHideServerTreeTimer(object sender, EventArgs e)
    {
      if (Program.Preferences.ServerTreeVisibility != ControlVisibility.AutoHide || !ServerTree.Instance.Visible)
        return;
      this._serverTreeAutoHideTimer.Interval = 250;
      this._serverTreeAutoHideTimer.Start();
    }

    protected void CreateMainMenu()
    {
      ToolStripMenuItem toolStripMenuItem1 = this._menuStrip.Add("&File", MenuNames.File);
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("&New...", MenuNames.FileNew, "Ctrl+N", (Action) (() => this.OnFileNew())));
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("&Open...", MenuNames.FileOpen, "Ctrl+O", (Action) (() => this.OnFileOpen())));
      toolStripMenuItem1.DropDownItems.Add("-");
      this._fileSaveMenuItem = (ToolStripMenuItem) new DelegateMenuItem("", MenuNames.FileSave, "Ctrl+S", (Action) (() => this.OnFileSave()));
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._fileSaveMenuItem);
      this._fileSaveAsMenuItem = (ToolStripMenuItem) new DelegateMenuItem("", MenuNames.FileSaveAs, (Action) (() => this.OnFileSaveAs()));
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._fileSaveAsMenuItem);
      int num;
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("Save A&ll", MenuNames.FileSaveAll, (Action) (() => num = (int) RdgFile.SaveAll())));
      toolStripMenuItem1.DropDownItems.Add("-");
      this._fileCloseMenuItem = (ToolStripMenuItem) new DelegateMenuItem("", MenuNames.FileClose, (Action) (() => this.OnFileClose()));
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) this._fileCloseMenuItem);
      toolStripMenuItem1.DropDownItems.Add("-");
      toolStripMenuItem1.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("E&xit", MenuNames.FileExit, "Alt+F4", (Action) (() => Program.TheForm.Close())));
      ToolStripMenuItem toolStripMenuItem2 = this._menuStrip.Add("&Edit", MenuNames.Edit);
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new FileRequiredMenuItem("&Add server...", MenuNames.EditAddServer, "Ctrl+A", (Action) (() => AddNodeDialogHelper.AddServersDialog())));
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new FileRequiredMenuItem("&Import servers...", MenuNames.EditImportServers, (Action) (() => AddNodeDialogHelper.ImportServersDialog())));
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new FileRequiredMenuItem("Add &group...", MenuNames.EditAddGroup, "Ctrl+G", (Action) (() => AddNodeDialogHelper.AddGroupDialog())));
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new FileRequiredMenuItem("Add s&mart group...", MenuNames.EditAddSmartGroup, (Action) (() => AddNodeDialogHelper.AddSmartGroupDialog())));
      toolStripMenuItem2.DropDownItems.Add("-");
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new SelectedNodeMenuItem("Remo&ve server/group...", MenuNames.EditRemove, "Delete", (Action<RdcTreeNode>) (node => ServerTree.Instance.ConfirmRemove(node, true))));
      toolStripMenuItem2.DropDownItems.Add("-");
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new FileRequiredMenuItem("&Find...", MenuNames.EditFind, "Ctrl+F", (Action) (() => MenuHelper.FindServers())));
      toolStripMenuItem2.DropDownItems.Add("-");
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) new SelectedNodeMenuItem<ServerBase>("A&dd to favorites", MenuNames.EditAddToFavorites, "Ctrl+D", (Action<ServerBase>) (param0 => MenuHelper.AddFavorite(ServerTree.Instance.SelectedNode as RdcTreeNode))));
      toolStripMenuItem2.DropDownItems.Add("-");
      this._editPropertiesMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("P&roperties", MenuNames.EditProperties, "Alt+Enter", (Action<RdcTreeNode>) (node => node.DoPropertiesDialog()));
      toolStripMenuItem2.DropDownItems.Add((ToolStripItem) this._editPropertiesMenuItem);
      ToolStripMenuItem toolStripMenuItem3 = this._menuStrip.Add("&Session", MenuNames.Session);
      this._sessionConnectMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("&Connect", MenuNames.SessionConnect, "Enter", (Action<RdcTreeNode>) (node => node.Connect()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionConnectMenuItem);
      this._sessionConnectAsMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("Connect &as...", MenuNames.SessionConnectAs, "Shift+Enter", (Action<RdcTreeNode>) (node => node.DoConnectAs()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionConnectAsMenuItem);
      this._sessionReconnectMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("R&econnect", MenuNames.SessionReconnect, (Action<RdcTreeNode>) (node => node.Reconnect()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionReconnectMenuItem);
      toolStripMenuItem3.DropDownItems.Add("-");
      this._sessionSendKeysMenuItem = toolStripMenuItem3.DropDownItems.Add("Send keys", MenuNames.SessionSendKeys);
      MenuHelper.AddSendKeysMenuItems(this._sessionSendKeysMenuItem, (Func<ServerBase>) (() => ServerTree.Instance.SelectedNode as ServerBase));
      if (RdpClient.SupportsRemoteSessionActions)
      {
        this._sessionRemoteActionsMenuItem = toolStripMenuItem3.DropDownItems.Add("Remote actions", MenuNames.SessionRemoteActions);
        MenuHelper.AddRemoteActionsMenuItems(this._sessionRemoteActionsMenuItem, (Func<ServerBase>) (() => ServerTree.Instance.SelectedNode as ServerBase));
        toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionRemoteActionsMenuItem);
      }
      toolStripMenuItem3.DropDownItems.Add("-");
      this._sessionDisconnectMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("&Disconnect", MenuNames.SessionDisconnect, (Action<RdcTreeNode>) (node => node.Disconnect()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionDisconnectMenuItem);
      toolStripMenuItem3.DropDownItems.Add("-");
      this._sessionLogOffMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem("Log off", MenuNames.SessionLogOff, (Action<RdcTreeNode>) (node => node.LogOff()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionLogOffMenuItem);
      this._sessionListSessionsMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("&List sessions", MenuNames.SessionListSessions, (Action<ServerBase>) (server => Program.ShowForm((Form) new ListSessionsForm(server))));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionListSessionsMenuItem);
      toolStripMenuItem3.DropDownItems.Add("-");
      this._sessionFullScreenMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("&Full screen", MenuNames.SessionFullScreen, (Action<ServerBase>) (server => server.GoFullScreen()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionFullScreenMenuItem);
      this._sessionUndockMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("&Undock", MenuNames.SessionUndock, (Action<ServerBase>) (server => server.Undock()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionUndockMenuItem);
      this._sessionUndockAndConnectMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("Undoc&k and connect", MenuNames.SessionUndockAndConnect, (Action<ServerBase>) (server =>
      {
        server.Undock();
        server.Connect();
      }));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionUndockAndConnectMenuItem);
      this._sessionDockMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("&Dock", MenuNames.SessionDock, (Action<ServerBase>) (server => server.Dock()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionDockMenuItem);
      toolStripMenuItem3.DropDownItems.Add("-");
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("Connect to...", MenuNames.SessionConnectTo, "Ctrl+Q", (Action) (() => MenuHelper.ConnectTo())));
      toolStripMenuItem3.DropDownItems.Add("-");
      this._sessionScreenCaptureMenuItem = (ToolStripMenuItem) new SelectedNodeMenuItem<ServerBase>("Screen capture", MenuNames.SessionScreenCapture, (Action<ServerBase>) (server => server.ScreenCapture()));
      toolStripMenuItem3.DropDownItems.Add((ToolStripItem) this._sessionScreenCaptureMenuItem);
      ToolStripMenuItem toolStripMenuItem4 = this._menuStrip.Add("&View", MenuNames.View);
      ToolStripMenuItem toolStripMenuItem5 = toolStripMenuItem4.DropDownItems.Add("&Sort order", MenuNames.ViewSortOrder);
      ToolStripMenuItem toolStripMenuItem6 = toolStripMenuItem5.DropDownItems.Add("&Groups", MenuNames.None);
      toolStripMenuItem6.DropDownItems.Add((ToolStripItem) new SortGroupsCheckedMenuItem("&Name", SortOrder.ByName));
      toolStripMenuItem6.DropDownItems.Add((ToolStripItem) new SortGroupsCheckedMenuItem("N&o sorting", SortOrder.None));
      ToolStripMenuItem toolStripMenuItem7 = toolStripMenuItem5.DropDownItems.Add("&Servers", MenuNames.None);
      toolStripMenuItem7.DropDownItems.Add((ToolStripItem) new SortServersCheckedMenuItem("&Status.Name", SortOrder.ByStatus));
      toolStripMenuItem7.DropDownItems.Add((ToolStripItem) new SortServersCheckedMenuItem("&Name", SortOrder.ByName));
      toolStripMenuItem7.DropDownItems.Add((ToolStripItem) new SortServersCheckedMenuItem("N&o sorting", SortOrder.None));
      ToolStripMenuItem toolStripMenuItem8 = toolStripMenuItem4.DropDownItems.Add("Server tree location", MenuNames.ViewServerTreeLocation);
      toolStripMenuItem8.DropDownItems.Add((ToolStripItem) new ServerTreeLocationMenuItem("&Left", DockStyle.Left));
      toolStripMenuItem8.DropDownItems.Add((ToolStripItem) new ServerTreeLocationMenuItem("&Right", DockStyle.Right));
      ToolStripMenuItem toolStripMenuItem9 = toolStripMenuItem4.DropDownItems.Add("Server tree visibility", MenuNames.ViewServerTreeVisibility);
      toolStripMenuItem9.DropDownItems.Add((ToolStripItem) new ServerTreeVisibilityMenuItem("&Dock", ControlVisibility.Dock));
      toolStripMenuItem9.DropDownItems.Add((ToolStripItem) new ServerTreeVisibilityMenuItem("&Auto hide", ControlVisibility.AutoHide));
      toolStripMenuItem9.DropDownItems.Add((ToolStripItem) new ServerTreeVisibilityMenuItem("&Hide", ControlVisibility.Hide));
      toolStripMenuItem4.DropDownItems.Add("-");
      ToolStripMenuItem toolStripMenuItem10 = toolStripMenuItem4.DropDownItems.Add("Built-in groups", MenuNames.ViewBuiltInGroups);
      foreach (IBuiltInVirtualGroup builtInVirtualGroup in Program.BuiltInVirtualGroups)
      {
        if (builtInVirtualGroup.IsVisibilityConfigurable)
          toolStripMenuItem10.DropDownItems.Add((ToolStripItem) new BuiltInVirtualGroupCheckedMenuItem(builtInVirtualGroup));
      }
      toolStripMenuItem4.DropDownItems.Add("-");
      toolStripMenuItem4.DropDownItems.Add((ToolStripItem) new DelegateCheckedMenuItem("&Lock window size", MenuNames.ViewLockWindowSize, (Func<bool>) (() => Program.Preferences.LockWindowSize), (Action<bool>) (isChecked =>
      {
        Program.Preferences.LockWindowSize = isChecked;
        this.LockWindowSize();
      })));
      ToolStripMenuItem toolStripMenuItem11 = new ToolStripMenuItem("&Client size");
      foreach (Size stockSiz in SizeHelper.StockSizes)
      {
        ClientSizeCheckedMenuItem sizeCheckedMenuItem = new ClientSizeCheckedMenuItem((RdcBaseForm) this, stockSiz);
        toolStripMenuItem11.DropDownItems.Add((ToolStripItem) sizeCheckedMenuItem);
      }
      toolStripMenuItem11.DropDownItems.Add((ToolStripItem) new CustomClientSizeCheckedMenuItem((RdcBaseForm) this, "&Custom"));
      toolStripMenuItem11.DropDownItems.Add((ToolStripItem) new SelectedNodeMenuItem<ServerBase>("From remote desktop size", MenuNames.None, (Action<ServerBase>) (node => this.SetClientSize(node.IsConnected ? node.ServerNode.Client.DesktopSize : node.RemoteDesktopSettings.DesktopSize.Value))));
      toolStripMenuItem4.DropDownItems.Add((ToolStripItem) toolStripMenuItem11);
      this._menuStrip.Items.Add((ToolStripItem) new RemoteDesktopsMenuItem());
      this._menuStrip.Add("&Tools", MenuNames.Tools).DropDownItems.Add((ToolStripItem) new DelegateMenuItem("&Options", MenuNames.ToolsOptions, (Action) (() => MenuHelper.ShowGlobalOptionsDialog())));
      ToolStripMenuItem toolStripMenuItem12 = this._menuStrip.Add("&Help", MenuNames.Help);
      toolStripMenuItem12.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("&Usage", MenuNames.HelpUsage, (Action) (() => Program.Usage())));
      toolStripMenuItem12.DropDownItems.Add("-");
      toolStripMenuItem12.DropDownItems.Add((ToolStripItem) new DelegateMenuItem("&About...", MenuNames.HelpAbout, (Action) (() => this.OnHelpAbout())));
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (!this._menuStrip.IsActive)
      {
        Action action1;
        if (this.Shortcuts.TryGetValue(keyData, out action1))
        {
          action1();
          return true;
        }
        RdcTreeNode selectedNode = this.GetSelectedNode();
        Action<RdcTreeNode> action2;
        if (selectedNode != null && this.SelectedNodeShortcuts.TryGetValue(keyData, out action2))
        {
          action2(selectedNode);
          return true;
        }
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    public RdcTreeNode GetSelectedNode() => this._clientPanel.GetSelectedNode(this.ActiveControl) ?? ServerTree.Instance.SelectedNode as RdcTreeNode;

    public override Size GetClientSize()
    {
      Size size = new Size(this._clientPanel.Width, this.ClientSize.Height);
      size.Height -= Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height;
      return size;
    }

    public void ShowGroup(GroupBase group) => this._clientPanel.ShowGroup(group);

    public void HideGroup(GroupBase group) => this._clientPanel.HideGroup(group);

    private bool AnyActive() => ConnectedGroup.Instance.Nodes.Count > 0 && FormTools.YesNoDialog("There are active connections. Are you sure?") != DialogResult.Yes;

    private SaveResult DoExit()
    {
      if (this._areShuttingDown)
        return SaveResult.Save;
      if (this.AnyActive())
        return SaveResult.Cancel;
      foreach (TreeNode node in ServerTree.Instance.Nodes)
      {
        if (node is FileGroup file)
        {
          SaveResult saveResult = RdgFile.ShouldSaveFile(file);
          switch (saveResult)
          {
            case SaveResult.Cancel:
              return saveResult;
            case SaveResult.NoSave:
              continue;
            default:
              if (RdgFile.DoSaveWithRetry(file) == SaveResult.Cancel)
                return SaveResult.Cancel;
              continue;
          }
        }
      }
      this._areShuttingDown = true;
      this._serverTreeAutoHideTimer.Stop();
      this._autoSaveTimer.Stop();
      this.Hide();
      Program.Preferences.WindowIsMaximized = this.WindowState == FormWindowState.Maximized;
      Rectangle rectangle = this.WindowState == FormWindowState.Normal ? this.Bounds : this.RestoreBounds;
      Program.Preferences.WindowPosition = new Point(rectangle.Left, rectangle.Top);
      Program.Preferences.WindowSize = new Size(rectangle.Width, rectangle.Height);
      Program.Preferences.ServerTreeWidth = ServerTree.Instance.Width;
      Program.Preferences.NeedToSave = true;
      Program.Preferences.Save();
      Program.PluginAction((Action<IPlugin>) (p => p.Shutdown()));
      using (Helpers.Timer("destroying sessions"))
      {
        ServerTree.Instance.SelectedNode = (TreeNode) null;
        ServerTree.Instance.Operation(OperationBehavior.SuspendSelect | OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
        {
          ServerTree.Instance.Nodes.VisitNodes((Action<RdcTreeNode>) (node =>
          {
            try
            {
              if (!(node is Server server))
                return;
              server.OnRemoving();
            }
            catch
            {
            }
          }));
          ServerTree.Instance.Nodes.Clear();
        }));
      }
      return SaveResult.Save;
    }

    public override void SetClientSize(Size size)
    {
      this.LockWindowSize(false);
      int width = size.Width;
      int height1 = size.Height;
      if (this.ServerTreeVisibility == ControlVisibility.Dock)
        width += ServerTree.Instance.Width + this._treeSplitter.Width;
      else if (this.ServerTreeVisibility == ControlVisibility.AutoHide)
        width += this._treeSplitter.Width;
      int height2 = height1 + (Program.Preferences.HideMainMenu ? 0 : this._menuPanel.Height);
      this.ClientSize = new Size(width, height2);
      this.LockWindowSize();
    }

    public void LockWindowSize() => this.LockWindowSize(Program.Preferences.LockWindowSize);

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      if (!this._allowSizeChanged)
        return;
      this.LayoutContent();
      Program.Preferences.NeedToSave = true;
    }

    protected override void OnClosing(CancelEventArgs e) => e.Cancel = this.DoExit() == SaveResult.Cancel;

    private void OnFileNew()
    {
      FileGroup fileGroup = RdgFile.NewFile();
      if (fileGroup == null)
        return;
      ServerTree.Instance.SelectedNode = (TreeNode) fileGroup;
      Program.Preferences.NeedToSave = true;
    }

    private void OnFileClose()
    {
      FileGroup selectedFile = ServerTree.Instance.GetSelectedFile();
      if (selectedFile == null)
        return;
      this.DoFileClose(selectedFile);
    }

    public void DoFileClose(FileGroup file)
    {
      RdgFile.CloseFileGroup(file);
      Program.Preferences.NeedToSave = true;
      Program.Preferences.Save();
    }

    private void OnFileOpen()
    {
      FileGroup fileGroup = RdgFile.OpenFile();
      if (fileGroup == null)
        return;
      ServerTree.Instance.SelectedNode = (TreeNode) fileGroup;
      Program.Preferences.NeedToSave = true;
      Program.Preferences.Save();
    }

    private void OnFileSave()
    {
      FileGroup selectedFile = ServerTree.Instance.GetSelectedFile();
      if (selectedFile == null)
        return;
      this.DoFileSave(selectedFile);
    }

    public void DoFileSave(FileGroup file)
    {
      int num = (int) RdgFile.SaveFileGroup(file);
    }

    private void OnFileSaveAs()
    {
      FileGroup selectedFile = ServerTree.Instance.GetSelectedFile();
      if (selectedFile == null)
        return;
      int num = (int) RdgFile.SaveAs(selectedFile);
    }

    protected override void UpdateMainMenu()
    {
      this.UpdateMenuItems(this._menuStrip.Items);
      RdcTreeNode selectedNode = this.GetSelectedNode();
      FileGroup fileGroup1;
      switch (selectedNode)
      {
        case null:
        case ServerRef _:
          fileGroup1 = (FileGroup) null;
          break;
        default:
          fileGroup1 = selectedNode.FileGroup;
          break;
      }
      FileGroup fileGroup2 = fileGroup1;
      if (fileGroup2 == null)
      {
        this._fileSaveMenuItem.Text = "&Save";
        this._fileSaveMenuItem.Enabled = false;
        this._fileSaveAsMenuItem.Text = "Save &as...";
        this._fileSaveAsMenuItem.Enabled = false;
        this._fileCloseMenuItem.Text = "&Close";
        this._fileCloseMenuItem.Enabled = false;
      }
      else
      {
        this._fileSaveMenuItem.Text = "&Save " + fileGroup2.GetFilename();
        this._fileSaveMenuItem.Enabled = true;
        this._fileSaveAsMenuItem.Text = "Save " + fileGroup2.GetFilename() + " &as...";
        this._fileSaveAsMenuItem.Enabled = true;
        this._fileCloseMenuItem.Text = "&Close " + fileGroup2.GetFilename();
        this._fileCloseMenuItem.Enabled = true;
      }
      this._editPropertiesMenuItem.Enabled = selectedNode != null && selectedNode.HasProperties;
      switch (selectedNode)
      {
        case ServerBase serverBase:
          bool isConnected = serverBase.IsConnected;
          bool clientFullScreen = serverBase.IsClientFullScreen;
          this._sessionConnectMenuItem.Enabled = !isConnected;
          this._sessionConnectAsMenuItem.Enabled = !isConnected;
          this._sessionReconnectMenuItem.Enabled = isConnected;
          this._sessionDisconnectMenuItem.Enabled = isConnected;
          this._sessionLogOffMenuItem.Enabled = !Policies.DisableLogOff && isConnected;
          this._sessionSendKeysMenuItem.Enabled = isConnected;
          if (RdpClient.SupportsRemoteSessionActions)
            this._sessionRemoteActionsMenuItem.Enabled = isConnected;
          this._sessionListSessionsMenuItem.Enabled = true;
          this._sessionFullScreenMenuItem.Enabled = isConnected && !clientFullScreen;
          this._sessionUndockMenuItem.Enabled = serverBase.IsClientDocked && !clientFullScreen;
          this._sessionUndockAndConnectMenuItem.Enabled = serverBase.IsClientDocked && !clientFullScreen && !isConnected;
          this._sessionDockMenuItem.Enabled = serverBase.IsClientUndocked;
          Server serverNode = serverBase.ServerNode;
          this._sessionScreenCaptureMenuItem.Enabled = serverNode.ConnectionState == RdpClient.ConnectionState.Connected && serverNode.IsClientDocked && serverNode.Client.Control.Visible;
          return;
        case GroupBase groupBase:
          bool anyConnected;
          bool allConnected;
          groupBase.AnyOrAllConnected(out anyConnected, out allConnected);
          this._sessionConnectMenuItem.Enabled = !allConnected;
          this._sessionConnectAsMenuItem.Enabled = !allConnected;
          this._sessionReconnectMenuItem.Enabled = anyConnected;
          this._sessionDisconnectMenuItem.Enabled = anyConnected;
          this._sessionLogOffMenuItem.Enabled = !Policies.DisableLogOff && anyConnected;
          break;
        default:
          this._sessionConnectMenuItem.Enabled = false;
          this._sessionConnectAsMenuItem.Enabled = false;
          this._sessionReconnectMenuItem.Enabled = false;
          this._sessionDisconnectMenuItem.Enabled = false;
          this._sessionLogOffMenuItem.Enabled = false;
          break;
      }
      this._sessionSendKeysMenuItem.Enabled = false;
      if (RdpClient.SupportsRemoteSessionActions)
        this._sessionRemoteActionsMenuItem.Enabled = false;
      this._sessionListSessionsMenuItem.Enabled = false;
      this._sessionFullScreenMenuItem.Enabled = false;
      this._sessionUndockMenuItem.Enabled = false;
      this._sessionUndockAndConnectMenuItem.Enabled = false;
      this._sessionDockMenuItem.Enabled = false;
      this._sessionScreenCaptureMenuItem.Enabled = false;
    }

    private void LockWindowSize(bool isLocked)
    {
      if (isLocked)
      {
        this.MinimumSize = this.Size;
        this.MaximumSize = this.Size;
      }
      else
      {
        this.MinimumSize = new Size(400, 300);
        this.MaximumSize = new Size(0, 0);
      }
    }

    private void OnHelpAbout()
    {
      using (About about = new About(this.IsInternalVersion))
      {
        int num = (int) about.ShowDialog();
      }
    }

    public void UpdateAutoSaveTimer()
    {
      if (Program.Preferences.AutoSaveFiles && Program.Preferences.AutoSaveInterval > 0)
      {
        this._autoSaveTimer.Interval = Program.Preferences.AutoSaveInterval * 60 * 1000;
        this._autoSaveTimer.Start();
      }
      else
        this._autoSaveTimer.Stop();
    }

    public DockStyle ServerTreeLocation
    {
      get => Program.Preferences.ServerTreeLocation;
      set
      {
        Program.Preferences.ServerTreeLocation = value;
        this._treeSplitter.Dock = value;
        ServerTree.Instance.Dock = value;
      }
    }

    public ControlVisibility ServerTreeVisibility
    {
      get => Program.Preferences.ServerTreeVisibility;
      set
      {
        Program.Preferences.ServerTreeVisibility = value;
        Size clientSize = this.GetClientSize();
        this.UpdateServerTreeVisibility(value);
        this.SetClientSize(clientSize);
      }
    }

    private void UpdateServerTreeVisibility(ControlVisibility value)
    {
      this.SuspendLayout();
      if (value == ControlVisibility.Dock)
      {
        ServerTree.Instance.Enabled = true;
        ServerTree.Instance.Show();
        if (Program.Preferences.ServerTreeVisibility != ControlVisibility.Dock)
        {
          ServerTree.Instance.BringToFront();
          if (Program.Preferences.ServerTreeLocation == DockStyle.Right)
            this._treeSplitter.Hide();
        }
        else
        {
          this._treeSplitter.SendToBack();
          ServerTree.Instance.SendToBack();
          this._treeSplitter.Show();
        }
      }
      else
      {
        ServerTree.Instance.Hide();
        ServerTree.Instance.Enabled = false;
        if (Program.Preferences.ServerTreeVisibility != ControlVisibility.AutoHide)
          this._treeSplitter.Hide();
        else
          this._treeSplitter.Show();
      }
      this.ResumeLayout();
    }

    private void OnVisibleChanged(object sender, EventArgs e) => Program.InitializedEvent.Set();

    MenuStrip IMainForm.MainMenuStrip => (MenuStrip) this._menuStrip;

    bool IMainForm.RegisterShortcut(Keys shortcutKey, Action action)
    {
      try
      {
        this.Shortcuts.Add(shortcutKey, action);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
