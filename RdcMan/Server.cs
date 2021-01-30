// Decompiled with JetBrains decompiler
// Type: RdcMan.Server
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using AxMSTSCLib;
using MSTSCLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Win32;

namespace RdcMan
{
  public class Server : ServerBase
  {
    public const string XmlNodeName = "server";
    internal const string XmlDisplayNameTag = "displayName";
    internal const string XmlServerNameTag = "name";
    internal const string XmlCommentTag = "comment";
    internal const string ConnectionTypeTag = "connectionType";
    internal const string VirtualMachineIdTag = "vmId";
    private const bool SimulateConnections = false;
    private RdpClient.ConnectionState _connectionState;
    private static readonly Dictionary<string, Helpers.ReadXmlDelegate> PropertyActions = new Dictionary<string, Helpers.ReadXmlDelegate>()
    {
      {
        "name",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as Server).Properties.ServerName.Value = childNode.InnerText)
      },
      {
        "connectionType",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) =>
        {
          ConnectionType result;
          Enum.TryParse<ConnectionType>(childNode.InnerText, out result);
          (node as Server).Properties.ConnectionType.Value = result;
        })
      },
      {
        "vmId",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as Server).Properties.VirtualMachineId.Value = childNode.InnerText)
      },
      {
        "displayName",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as Server).Properties.DisplayName.Value = childNode.InnerText)
      },
      {
        "comment",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) =>
        {
          if (childNode.FirstChild == null)
            return;
          (node as Server).Properties.Comment.Value = childNode.InnerText;
        })
      }
    };
    private RdpClient _client;
    private ServerBox _serverBox;
    private readonly List<ServerRef> _serverRefList;
    private ServerBase.DisplayStates _displayState;
    private string _disconnectionReason = string.Empty;
    private readonly object _connectionStateLock = new object();
    private int _noFullScreenBehavior;
    private static readonly Server.DisconnectionReason[] DisconnectionReasons = new Server.DisconnectionReason[31]
    {
      new Server.DisconnectionReason(1, ""),
      new Server.DisconnectionReason(2, ""),
      new Server.DisconnectionReason(3, ""),
      new Server.DisconnectionReason(260, "DNS name lookup failure"),
      new Server.DisconnectionReason(263, "Authentication failure"),
      new Server.DisconnectionReason(264, "Connection timed out"),
      new Server.DisconnectionReason(516, "Unable to establish a connection"),
      new Server.DisconnectionReason(522, "Smart card reader not detected"),
      new Server.DisconnectionReason(1289, "Server does not support authentication"),
      new Server.DisconnectionReason(1800, "You already have a console session in progress"),
      new Server.DisconnectionReason(2052, "Bad IP address specified"),
      new Server.DisconnectionReason(2055, "Login failed"),
      new Server.DisconnectionReason(2056, "Server has no sessions available"),
      new Server.DisconnectionReason(2308, "Socket closed"),
      new Server.DisconnectionReason(2567, "The specified user has no account"),
      new Server.DisconnectionReason(2824, "Session connected by other client"),
      new Server.DisconnectionReason(2825, "Server authentication failure"),
      new Server.DisconnectionReason(3847, "The password is expired"),
      new Server.DisconnectionReason(4615, "The user password must be changed before logging on for the first time"),
      new Server.DisconnectionReason(7175, "An incorrect PIN was presented to the smart card"),
      new Server.DisconnectionReason(7943, "No credentials entered"),
      new Server.DisconnectionReason(8711, "The smart card is blocked"),
      new Server.DisconnectionReason(50331655, "Gateway authentication failure"),
      new Server.DisconnectionReason(50331656, "Server not found"),
      new Server.DisconnectionReason(50331660, "Unable to connect to gateway"),
      new Server.DisconnectionReason(50331669, "Smartcard authentication failure"),
      new Server.DisconnectionReason(50331670, "Server not found"),
      new Server.DisconnectionReason(50331676, "Your user or computer account is not authorized to access the gateway server"),
      new Server.DisconnectionReason(50331677, "No gateway credentials entered"),
      new Server.DisconnectionReason(50331678, ""),
      new Server.DisconnectionReason(50331686, "No smartcard PIN entered")
    };
    private static readonly Server.DisconnectionReason[] ExtendedDisconnectionReasons = new Server.DisconnectionReason[26]
    {
      new Server.DisconnectionReason(0, "No additional information is available"),
      new Server.DisconnectionReason(1, ""),
      new Server.DisconnectionReason(2, ""),
      new Server.DisconnectionReason(3, "The server has disconnected the client because the client has been idle for a period of time longer than the designated time-out period"),
      new Server.DisconnectionReason(4, "The server has disconnected the client because the client has exceeded the period designated for connection"),
      new Server.DisconnectionReason(5, "The client's connection was replaced by another connection"),
      new Server.DisconnectionReason(6, "No memory is available"),
      new Server.DisconnectionReason(7, "The server denied the connection"),
      new Server.DisconnectionReason(8, "The server denied the connection for security reasons"),
      new Server.DisconnectionReason(9, "The user account is not authorized for remote login"),
      new Server.DisconnectionReason(10, "The user account credentials must be reentered"),
      new Server.DisconnectionReason(11, "The client was remotely disconnected"),
      new Server.DisconnectionReason(12, "The connection was lost"),
      new Server.DisconnectionReason(256, "Internal licensing error"),
      new Server.DisconnectionReason(257, "No license server was available"),
      new Server.DisconnectionReason(258, "No valid software license was available"),
      new Server.DisconnectionReason(259, "The remote computer received a licensing message that was not valid"),
      new Server.DisconnectionReason(260, "The hardware ID does not match the one designated on the software license"),
      new Server.DisconnectionReason(261, "Client license error"),
      new Server.DisconnectionReason(262, "Network problems occurred during the licensing protocol"),
      new Server.DisconnectionReason(263, "The client ended the licensing protocol prematurely"),
      new Server.DisconnectionReason(264, "A licensing message was encrypted incorrectly"),
      new Server.DisconnectionReason(265, "The local computer's client access license could not be upgraded or renewed"),
      new Server.DisconnectionReason(266, "The remote computer is not licensed to accept remote connections"),
      new Server.DisconnectionReason(267, ""),
      new Server.DisconnectionReason(768, "")
    };

    static Server() => ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(Server.OnServerChanged);

    protected Server()
    {
      this._serverRefList = new List<ServerRef>();
      this.ChangeImageIndex(ImageConstants.DisconnectedServer);
    }

    private static void OnServerChanged(ServerChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged) || !(e.Server is Server server))
        return;
      Action<ServerRef> action = (Action<ServerRef>) (r =>
      {
        GroupBase parent = r.Parent as GroupBase;
        if (!ServerTree.Instance.SortGroup(parent))
          return;
        ServerTree.Instance.OnGroupChanged(parent, ChangeType.InvalidateUI);
      });
      server.VisitServerRefs(action);
    }

    public static event Action<ConnectionStateChangedEventArgs> ConnectionStateChanged;

    public static event Action<Server> FocusReceived;

    public RdpClient.ConnectionState ConnectionState
    {
      get => this._connectionState;
      private set
      {
        if (this._connectionState == value)
          return;
        this._connectionState = value;
        Action<ConnectionStateChangedEventArgs> connectionStateChanged = Server.ConnectionStateChanged;
        if (connectionStateChanged == null)
          return;
        ConnectionStateChangedEventArgs changedEventArgs = new ConnectionStateChangedEventArgs(this, this._connectionState);
        connectionStateChanged(changedEventArgs);
      }
    }

    public override Server ServerNode => this;

    public override string RemoveTypeDescription => "server";

    public override ServerSettings Properties => ((RdcTreeNode) this).Properties as ServerSettings;

    public override CommonDisplaySettings DisplaySettings => ((RdcTreeNode) this).DisplaySettings;

    public override ServerBase.DisplayStates DisplayState
    {
      get => this._displayState;
      set
      {
        if (value == this._displayState)
          return;
        this._displayState = value;
        if (value == ServerBase.DisplayStates.Invalid)
          return;
        this.SetText();
        this.SetClientSizeProperties();
      }
    }

    public override bool IsClientDocked => !this.IsClientInitialized || this.ServerForm == null;

    public override bool IsClientUndocked => this.IsClientInitialized && this.ServerForm != null;

    public override RdcBaseForm ParentForm => this.IsClientUndocked ? (RdcBaseForm) this.ServerForm : base.ParentForm;

    private ServerForm ServerForm => this.Client.Control.Parent as ServerForm;

    public void SuspendFullScreenBehavior() => Interlocked.Increment(ref this._noFullScreenBehavior);

    public void ResumeFullScreenBehavior() => Interlocked.Decrement(ref this._noFullScreenBehavior);

    public override Size Size
    {
      get => !this.UseServerBox ? this._client.Control.Size : this._serverBox.Size;
      set
      {
        if (!this.UseServerBox)
          this._client.Control.Size = value;
        this._serverBox.Size = value;
      }
    }

    private bool IsClientInPanel => this.IsClientInitialized && this.IsClientDocked;

    public override Point Location
    {
      get => !this.UseServerBox ? this._client.Control.Location : this._serverBox.Location;
      set
      {
        if (!this.UseServerBox)
          this._client.Control.Location = value;
        this._serverBox.Location = value;
      }
    }

    public string GetQualifiedNameForUI()
    {
      string serverName;
      Server.SplitName(this.ServerName, out serverName, out int _);
      return this.DisplayName.Equals(serverName, StringComparison.OrdinalIgnoreCase) ? this.DisplayName : string.Format("{0} ({1})", (object) this.DisplayName, (object) serverName);
    }

    public string ConnectedText => this.IsThumbnail ? "Connected" : "Connected to " + this.GetQualifiedNameForUI();

    public string ConnectingText => this.IsThumbnail ? "Connecting" : "Connecting to " + this.GetQualifiedNameForUI();

    public string DisconnectedText
    {
      get
      {
        string str;
        if (this.IsThumbnail)
        {
          str = "Disconnected";
          if (!string.IsNullOrEmpty(this._disconnectionReason))
            str += " [error]";
        }
        else
        {
          str = "Disconnected from " + this.GetQualifiedNameForUI();
          if (!string.IsNullOrEmpty(this._disconnectionReason))
            str = str + Environment.NewLine + "[" + this._disconnectionReason + "]";
        }
        return str;
      }
    }

    private void SetText()
    {
      this._serverBox.SetText();
      if (!this.IsClientInitialized)
        return;
      this._client.SetText();
    }

    public string GetConnectionStateText()
    {
      switch (this.ConnectionState)
      {
        case RdpClient.ConnectionState.Disconnected:
          return this.DisconnectedText;
        case RdpClient.ConnectionState.Connecting:
          return this.ConnectingText;
        case RdpClient.ConnectionState.Connected:
          return this.ConnectedText;
        default:
          return "<GetText error>";
      }
    }

    protected override void InitSettings()
    {
      this.Properties = (CommonNodeSettings) new ServerSettings();
      this.DisplaySettings = (CommonDisplaySettings) new ServerDisplaySettings();
      base.InitSettings();
    }

    internal override void Focus()
    {
      if (!this.IsClientUndocked && this.UseServerBox)
        this._serverBox.Focus();
      else
        this._client.Control.Focus();
    }

    internal override void FocusConnectedClient()
    {
      if (!this.IsConnected || !this.IsClientInitialized)
        return;
      this._client.Control.Focus();
    }

    internal void SetNormalView()
    {
      this.DisplayState = ServerBase.DisplayStates.Normal;
      this.Size = Program.TheForm.GetClientSize();
      this.Location = new Point(0, 0);
      this.EnableDisableClient();
    }

    internal void SetThumbnailView(int left, int top, int width, int height)
    {
      this.DisplayState = ServerBase.DisplayStates.Thumbnail;
      this.Size = new Size(width, height);
      this.Location = new Point(left, top);
      this.EnableDisableClient();
    }

    internal override void ScreenCapture()
    {
      Control control = this.Client.Control;
      Graphics graphics = (Graphics) null;
      try
      {
        Point screen = control.PointToScreen(control.Location);
        Size size = control.Size;
        Bitmap bitmap = new Bitmap(size.Width, size.Height);
        graphics = Graphics.FromImage((Image) bitmap);
        graphics.CopyFromScreen(screen.X, screen.Y, 0, 0, bitmap.Size);
        Clipboard.SetDataObject((object) bitmap);
      }
      catch (Exception ex)
      {
        FormTools.ErrorDialog("Error capturing session screen: " + ex.Message);
      }
      finally
      {
        graphics?.Dispose();
      }
    }

    internal RdpClient Client => this._client;

    private bool IsClientInitialized => this.Client != null;

    protected void InitRequiredForDisplay() => this._serverBox = new ServerBox(this);

    private void AddToClientPanel()
    {
      if (this._serverBox.Parent != null)
        return;
      Program.TheForm.AddToClientPanel((Control) this._serverBox);
    }

    private void RemoveFromClientPanel()
    {
      if (this._serverBox.Parent == null)
        return;
      Program.TheForm.RemoveFromClientPanel((Control) this._serverBox);
    }

    private void InitClient()
    {
      if (this.IsClientInitialized)
        return;
      this._client = RdpClient.AllocClient(this, Program.TheForm);
      this._client.ConnectConnectionHandlers(new EventHandler(this.OnConnected), new EventHandler(this.OnConnecting), new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(this.OnDisconnected), new AxMSTSCLib.IMsTscAxEvents_OnAutoReconnectingEventHandler(this.OnAutoReconnecting), new AxMSTSCLib.IMsTscAxEvents_OnAutoReconnecting2EventHandler(this.OnAutoReconnecting2), new EventHandler(this.OnAutoReconnected), new AxMSTSCLib.IMsTscAxEvents_OnFocusReleasedEventHandler(this.OnFocusReleased));
      this._client.ConnectContainerHandlers(new EventHandler(this.OnRequestGoFullScreen), new EventHandler(this.OnRequestLeaveFullScreen), new EventHandler(this.OnRequestContainerMinimize), new AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler(this.OnConfirmClose), new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(this.OnFatalError));
      this._client.Control.GotFocus += new EventHandler(this.ClientGotFocus);
      this._client.AdvancedSettings2.ContainerHandledFullScreen = 1;
      this._client.AdvancedSettings2.allowBackgroundInput = 1;
      this._client.Control.Size = this._serverBox.Size;
      this._client.Control.Location = this._serverBox.Location;
      this.SetClientSizeProperties();
      this.SetText();
      if (this.UseServerBox || !this._serverBox.Visible)
        return;
      this._client.Control.Show();
      this._serverBox.Hide();
    }

    private void DestroyClient()
    {
      if (!this.IsClientInitialized)
        return;
      this._client.DisconnectConnectionHandlers(new EventHandler(this.OnConnected), new EventHandler(this.OnConnecting), new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(this.OnDisconnected), new AxMSTSCLib.IMsTscAxEvents_OnAutoReconnectingEventHandler(this.OnAutoReconnecting), new AxMSTSCLib.IMsTscAxEvents_OnAutoReconnecting2EventHandler(this.OnAutoReconnecting2), new EventHandler(this.OnAutoReconnected), new AxMSTSCLib.IMsTscAxEvents_OnFocusReleasedEventHandler(this.OnFocusReleased));
      this._client.DisconnectContainerHandlers(new EventHandler(this.OnRequestGoFullScreen), new EventHandler(this.OnRequestLeaveFullScreen), new EventHandler(this.OnRequestContainerMinimize), new AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler(this.OnConfirmClose), new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(this.OnFatalError));
      RdpClient client = this._client;
      this._client = (RdpClient) null;
      RdpClient.ReleaseClient(client);
    }

    internal static Server CreateForAddDialog() => new Server();

    public static Server Create(string serverName, string displayName, GroupBase group)
    {
      Server server = new Server();
      server.Properties.ServerName.Value = serverName;
      server.Properties.DisplayName.Value = displayName;
      server.FinishConstruction(group);
      return server;
    }

    internal static Server Create(ServerPropertiesDialog dlg)
    {
      Server associatedNode = dlg.AssociatedNode as Server;
      associatedNode.FinishConstruction(dlg.PropertiesPage.ParentGroup);
      return associatedNode;
    }

    internal static Server Create(string name, ServerPropertiesDialog dlg)
    {
      Server associatedNode = dlg.AssociatedNode as Server;
      Server server = new Server();
      server.CopySettings((RdcTreeNode) associatedNode, (System.Type) null);
      server.Properties.ServerName.Value = name;
      server.Properties.DisplayName.Value = name;
      server.FinishConstruction(dlg.PropertiesPage.ParentGroup);
      return server;
    }

    internal static Server Create(
      XmlNode xmlNode,
      GroupBase group,
      ICollection<string> errors)
    {
      Server server = new Server();
      server.ReadXml(xmlNode, errors);
      server.FinishConstruction(group);
      return server;
    }

    protected void FinishConstruction(GroupBase group)
    {
      if (string.IsNullOrEmpty(this.DisplayName))
        this.Properties.DisplayName.Value = this.ServerName;
      this.Text = this.DisplayName;
      this.InitRequiredForDisplay();
      ServerTree.Instance.AddNode((RdcTreeNode) this, group);
    }

    private void ReadXml(XmlNode xmlNode, ICollection<string> errors) => this.ReadXml(Server.PropertyActions, xmlNode, errors);

    public override void OnRemoving()
    {
      this.VisitServerRefs((Action<ServerRef>) (r => r.OnRemoveServer()));
      this._serverRefList.Clear();
      if (this.IsClientUndocked)
        this.ServerForm.Close();
      this.Hide();
      this._serverBox.Dispose();
      this._serverBox = (ServerBox) null;
      this.DestroyClient();
    }

    private bool UseServerBox
    {
      get
      {
        if (!this.IsClientInPanel)
          return true;
        return this.IsThumbnail && !(this.ServerNode.Parent as GroupBase).DisplaySettings.SessionThumbnailPreview.Value;
      }
    }

    internal override void Show()
    {
      this.AddToClientPanel();
      if (this.UseServerBox)
        this._serverBox.Show();
      else
        this._client.Control.Show();
    }

    internal override void Hide()
    {
      if (this.DisplayState == ServerBase.DisplayStates.Invalid)
        return;
      this.DisplayState = ServerBase.DisplayStates.Invalid;
      this._serverBox.Hide();
      this.RemoveFromClientPanel();
      if (!this.IsClientInPanel)
        return;
      this._client.Control.Hide();
    }

    public override void Connect() => this.ConnectAs((LogonCredentials) null, (ConnectionSettings) null);

    public override void ConnectAs(
      LogonCredentials logonCredentials,
      ConnectionSettings connectionSettings)
    {
      this.InitClient();
      lock (this._connectionStateLock)
      {
        if (this.IsConnected)
          return;
        this.InheritSettings();
        this.ResolveCredentials();
        if (logonCredentials == null)
          logonCredentials = this.LogonCredentials;
        else
          this.ResolveCredentials(logonCredentials);
        if (connectionSettings == null)
          connectionSettings = this.ConnectionSettings;
        string str1 = "{none}";
        try
        {
          IMsRdpClientAdvancedSettings advancedSettings2 = this._client.AdvancedSettings2;
          IMsRdpClientAdvancedSettings6 advancedSettings7 = this._client.AdvancedSettings7;
          IMsRdpClientAdvancedSettings7 advancedSettings8 = this._client.AdvancedSettings8;
          IMsRdpClientNonScriptable4 ocx = (IMsRdpClientNonScriptable4) this._client.GetOcx();
          string serverName;
          int port;
          Server.SplitName(this.ServerName, out serverName, out port);
          if (port == -1)
            port = this.ConnectionSettings.Port.Value;
          str1 = "server name";
          this._client.MsRdpClient.Server = serverName;
          string userName = CredentialsUI.GetUserName(logonCredentials.UserName.Value);
          string str2 = logonCredentials.Domain.Value;
          if (!string.IsNullOrEmpty(userName))
          {
            str1 = "user name";
            this._client.MsRdpClient.UserName = userName;
          }
          else
            this._client.MsRdpClient.UserName = (string) null;
          if (!string.IsNullOrEmpty(str2))
          {
            str1 = "domain";
            this._client.MsRdpClient.Domain = !str2.Equals("[server]", StringComparison.OrdinalIgnoreCase) ? (!str2.Equals("[display]", StringComparison.OrdinalIgnoreCase) ? str2 : this.DisplayName) : this.ServerName;
          }
          else
            this._client.MsRdpClient.Domain = (string) null;
          str1 = "password";
          if (logonCredentials.Password.IsDecrypted && !string.IsNullOrEmpty(logonCredentials.Password.Value))
            advancedSettings2.ClearTextPassword = logonCredentials.Password.Value;
          advancedSettings2.keepAliveInterval = 60000;
          advancedSettings7.HotKeyAltEsc = (int) Program.Preferences.HotKeyAltEsc;
          advancedSettings7.HotKeyAltSpace = (int) Program.Preferences.HotKeyAltSpace;
          advancedSettings7.HotKeyAltShiftTab = (int) Program.Preferences.HotKeyAltShiftTab;
          advancedSettings7.HotKeyAltTab = (int) Program.Preferences.HotKeyAltTab;
          advancedSettings7.HotKeyCtrlEsc = (int) Program.Preferences.HotKeyCtrlEsc;
          advancedSettings7.HotKeyCtrlAltDel = (int) Program.Preferences.HotKeyCtrlAltDel;
          advancedSettings7.HotKeyFocusReleaseLeft = (int) Program.Preferences.HotKeyFocusReleaseLeft;
          advancedSettings7.HotKeyFocusReleaseRight = (int) Program.Preferences.HotKeyFocusReleaseRight;
          advancedSettings7.HotKeyFullScreen = (int) Program.Preferences.HotKeyFullScreen;
          this._client.SecuredSettings2.KeyboardHookMode = (int) this.LocalResourceSettings.KeyboardHookMode.Value;
          RdpClient.ConnectionBarState connectionBarState = Program.Preferences.ConnectionBarState;
          if (connectionBarState == RdpClient.ConnectionBarState.Off)
          {
            advancedSettings2.DisplayConnectionBar = false;
          }
          else
          {
            advancedSettings2.DisplayConnectionBar = true;
            advancedSettings2.PinConnectionBar = connectionBarState == RdpClient.ConnectionBarState.Pinned;
          }
          this._client.MsRdpClient.FullScreen = false;
          advancedSettings2.PerformanceFlags = Program.Preferences.PerformanceFlags;
          advancedSettings2.GrabFocusOnConnect = false;
          str1 = "gateway settings";
          this.ConfigureGateway();
          if (this.Properties.ConnectionType.Value == ConnectionType.VirtualMachineConsoleConnect)
          {
            advancedSettings8.PCB = this.Properties.VirtualMachineId.Value;
            advancedSettings2.RDPPort = 2179;
            advancedSettings2.ConnectToServerConsole = true;
            advancedSettings7.AuthenticationLevel = 0U;
            advancedSettings7.AuthenticationServiceClass = "Microsoft Virtual Console Service";
            advancedSettings7.EnableCredSspSupport = true;
            advancedSettings8.NegotiateSecurityLayer = false;
          }
          else
          {
            str1 = "port";
            advancedSettings2.RDPPort = port;
            str1 = "loadBalanceInfo";
            string str3 = this.ConnectionSettings.LoadBalanceInfo.Value;
            if (!string.IsNullOrEmpty(str3))
            {
              if (str3.Length % 2 == 1)
                str3 += " ";
              byte[] bytes = Encoding.UTF8.GetBytes(str3 + Environment.NewLine);
              advancedSettings2.LoadBalanceInfo = Encoding.Unicode.GetString(bytes);
            }
            str1 = "connect to console";
            if (advancedSettings7 != null)
              advancedSettings7.ConnectToAdministerServer = connectionSettings.ConnectToConsole.Value;
            advancedSettings2.ConnectToServerConsole = connectionSettings.ConnectToConsole.Value;
            str1 = "start program";
            this._client.SecuredSettings.StartProgram = this.ConnectionSettings.StartProgram.Value;
            this._client.SecuredSettings.WorkDir = this.ConnectionSettings.WorkingDir.Value;
            this._client.AdvancedSettings5.EnableAutoReconnect = true;
            this._client.AdvancedSettings5.MaxReconnectAttempts = 20;
            advancedSettings2.EnableWindowsKey = 1;
            str1 = "local resources";
            this._client.SecuredSettings2.AudioRedirectionMode = (int) this.LocalResourceSettings.AudioRedirectionMode.Value;
            if (advancedSettings8 != null)
            {
              advancedSettings8.AudioQualityMode = (uint) this.LocalResourceSettings.AudioRedirectionQuality.Value;
              advancedSettings8.AudioCaptureRedirectionMode = this.LocalResourceSettings.AudioCaptureRedirectionMode.Value == RdpClient.AudioCaptureRedirectionMode.Record;
              if (RdpClient.SupportsPanning)
              {
                advancedSettings8.EnableSuperPan = Program.Preferences.EnablePanning;
                advancedSettings8.SuperPanAccelerationFactor = (uint) Program.Preferences.PanningAcceleration;
              }
            }
            if (RdpClient.SupportsFineGrainedRedirection)
            {
              IMsRdpDriveCollection driveCollection = this._client.ClientNonScriptable3.DriveCollection;
              for (uint index = 0; index < driveCollection.DriveCount; ++index)
              {
                IMsRdpDrive msRdpDrive = driveCollection.get_DriveByIndex(index);
                string str4 = msRdpDrive.Name.Substring(0, msRdpDrive.Name.Length - 1);
                msRdpDrive.RedirectionState = this.LocalResourceSettings.RedirectDrivesList.Value.Contains(str4);
              }
            }
            else
              advancedSettings2.RedirectDrives = this.LocalResourceSettings.RedirectDrives.Value;
            advancedSettings2.RedirectPorts = this.LocalResourceSettings.RedirectPorts.Value;
            advancedSettings2.RedirectPrinters = this.LocalResourceSettings.RedirectPrinters.Value;
            advancedSettings2.RedirectSmartCards = this.LocalResourceSettings.RedirectSmartCards.Value;
            this._client.AdvancedSettings6.RedirectClipboard = this.LocalResourceSettings.RedirectClipboard.Value;
            this._client.AdvancedSettings6.RedirectDevices = this.LocalResourceSettings.RedirectPnpDevices.Value;
            str1 = "remote desktop attributes";
            this._client.DesktopSize = this.GetRemoteDesktopSize();
            this._client.MsRdpClient.ColorDepth = this.RemoteDesktopSettings.ColorDepth.Value;
            str1 = "security settings";
            this._client.AdvancedSettings5.AuthenticationLevel = (uint) this.SecuritySettings.AuthenticationLevel.Value;
            if (advancedSettings7 != null)
            {
              advancedSettings7.EnableCredSspSupport = true;
              ocx.PromptForCredentials = false;
              ocx.NegotiateSecurityLayer = true;
            }
          }
          str1 = "client connection";
          this._disconnectionReason = string.Empty;
          using (Helpers.Timer("invoking connect on {0} client", (object) this.DisplayName))
            this._client.MsRdpClient.Connect();
        }
        catch (Exception ex)
        {
          this.ConnectionState = RdpClient.ConnectionState.Disconnected;
          this._disconnectionReason = "Error setting up connection properties";
          FormTools.ErrorDialog("Error possibly involving '" + str1 + "':\n" + ex.Message);
          Log.Write("Error({1}) connecting to {0}", (object) this.DisplayName, (object) ex.Message);
        }
      }
    }

    internal void DumpSessionState()
    {
      using (Helpers.Timer("dumping session state of {0}", (object) this.DisplayName))
        this._client.Dump();
    }

    internal static void SplitName(string qualifiedName, out string serverName, out int port)
    {
      string[] strArray = qualifiedName.Split(new char[1]
      {
        ':'
      }, StringSplitOptions.RemoveEmptyEntries);
      serverName = strArray.Length > 0 ? strArray[0] : string.Empty;
      if (strArray.Length == 2 && int.TryParse(strArray[1], out port))
        return;
      port = -1;
    }

    private void ConfigureGateway()
    {
      IMsRdpClientTransportSettings transportSettings = this._client.TransportSettings;
      if (this.GatewaySettings.UseGatewayServer.Value)
      {
        uint num1 = this.GatewaySettings.BypassGatewayForLocalAddresses.Value ? 2U : 1U;
        transportSettings.GatewayProfileUsageMethod = 1U;
        transportSettings.GatewayUsageMethod = num1;
        uint num2 = (uint) this.GatewaySettings.LogonMethod.Value;
        transportSettings.GatewayCredsSource = num2;
        transportSettings.GatewayHostname = this.GatewaySettings.HostName.Value;
        IMsRdpClientTransportSettings2 transportSettings2 = this._client.TransportSettings2;
        if (transportSettings2 == null)
          return;
        transportSettings2.GatewayCredSharing = this.GatewaySettings.CredentialSharing.Value ? 1U : 0U;
        if (this.GatewaySettings.LogonMethod.Value != RdpClient.GatewayLogonMethod.NTLM)
          return;
        transportSettings2.GatewayUsername = this.GatewaySettings.UserName.Value;
        transportSettings2.GatewayDomain = this.GatewaySettings.Domain.Value;
        transportSettings2.GatewayPassword = this.GatewaySettings.Password.Value;
      }
      else
      {
        transportSettings.GatewayProfileUsageMethod = 0U;
        transportSettings.GatewayUsageMethod = 0U;
      }
    }

    public override void Reconnect()
    {
      Log.Write("Begin reconnect to {0}", (object) this.DisplayName);
      ReconnectGroup.Instance.AddReference((ServerBase) this).Start(true);
    }

    public override void Disconnect()
    {
      using (Helpers.Timer("invoking disconnect on the {0} client", (object) this.DisplayName))
      {
        if (!this.IsConnected)
          return;
        try
        {
          this._client.MsRdpClient.Disconnect();
        }
        catch (Exception ex)
        {
          Log.Write("Error disconnection: {0}", (object) ex.Message);
        }
      }
    }

    public override void LogOff()
    {
      Log.Write("Begin logoff from {0}", (object) this.DisplayName);
      if (!this.IsConnected)
        return;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Server.LogOffWorkerProc), (object) this);
    }

    private static void LogOffWorkerProc(object o)
    {
      Server server = o as Server;
      RemoteSessions remoteSessions = new RemoteSessions((ServerBase) server);
      bool success = true;
      string reason = string.Empty;
      try
      {
        if (!remoteSessions.OpenServer())
        {
          success = false;
          reason = "Unable to access remote sessions";
        }
        else
        {
          IList<RemoteSessionInfo> remoteSessionInfoList = remoteSessions.QuerySessions();
          if (remoteSessionInfoList == null)
          {
            success = false;
            reason = "Unable to enumerate remote sessions";
          }
          else
          {
            int id = -1;
            foreach (RemoteSessionInfo remoteSessionInfo in (IEnumerable<RemoteSessionInfo>) remoteSessionInfoList)
            {
              if (remoteSessionInfo.State == Wts.ConnectstateClass.Active && remoteSessionInfo.ClientName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase) && (remoteSessionInfo.UserName.Equals(server._client.MsRdpClient.UserName, StringComparison.OrdinalIgnoreCase) && remoteSessionInfo.DomainName.Equals(server._client.MsRdpClient.Domain, StringComparison.OrdinalIgnoreCase)))
              {
                if (id == -1)
                {
                  id = remoteSessionInfo.SessionId;
                }
                else
                {
                  success = false;
                  reason = "Multiple active sessions, couldn't determine which to log off";
                  return;
                }
              }
            }
            if (!success)
              return;
            success = remoteSessions.LogOffSession(id);
            reason = "Log off session API failed";
          }
        }
      }
      catch
      {
        success = false;
        reason = "Internal error";
      }
      finally
      {
        remoteSessions.CloseServer();
        Program.TheForm.Invoke((Delegate) (() => server.LogOffResultCallback(success, reason)));
      }
    }

    private void LogOffResultCallback(bool success, string text)
    {
      Log.Write("End logoff from {0}", (object) this.DisplayName);
      if (success)
        return;
      FormTools.ErrorDialog("Unable to log off from " + this.DisplayName + "\r\nReason: " + text);
    }

    public override bool IsConnected => this.ConnectionState != RdpClient.ConnectionState.Disconnected;

    public override bool IsClientFullScreen => this.IsClientInitialized && this.Client.MsRdpClient.FullScreen;

    private void OnConnecting(object sender, EventArgs e)
    {
      lock (this._connectionStateLock)
      {
        Log.Write("OnConnecting {0}", (object) this.DisplayName);
        this.UpdateOnConnectionStateChange(ImageConstants.ConnectingServer, RdpClient.ConnectionState.Connecting);
      }
    }

    private void OnConnected(object sender, EventArgs e)
    {
      lock (this._connectionStateLock)
      {
        Log.Write("OnConnected {0}", (object) this.DisplayName);
        this.Location = new Point(this.Location.X, this.Location.Y);
        this.UpdateOnConnectionStateChange(ImageConstants.ConnectedServer, RdpClient.ConnectionState.Connected);
        if (!(ServerTree.Instance.SelectedNode is ServerBase selectedNode) || selectedNode.ServerNode != this)
          return;
        this.Focus();
      }
    }

    private void OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
    {
      lock (this._connectionStateLock)
      {
        Log.Write("OnDisconnected {0}: discReason={1} extendedDisconnectReason={2}", (object) this.DisplayName, (object) e.discReason, (object) this._client.MsRdpClient.ExtendedDisconnectReason);
        this._disconnectionReason = string.Empty;
        Server.DisconnectionReason disconnectionReason = (Server.DisconnectionReason) null;
        if (this._client.MsRdpClient.ExtendedDisconnectReason != ExtendedDisconnectReasonCode.exDiscReasonNoInfo)
        {
          disconnectionReason = ((IEnumerable<Server.DisconnectionReason>) Server.ExtendedDisconnectionReasons).SingleOrDefault<Server.DisconnectionReason>((Func<Server.DisconnectionReason, bool>) (r => (ExtendedDisconnectReasonCode) r.Code == this._client.MsRdpClient.ExtendedDisconnectReason));
          if (disconnectionReason == null)
            this._disconnectionReason = string.Format("Unknown extended disconnection reason {0}", (object) this._client.MsRdpClient.ExtendedDisconnectReason);
        }
        else if (e != null)
        {
          disconnectionReason = ((IEnumerable<Server.DisconnectionReason>) Server.DisconnectionReasons).SingleOrDefault<Server.DisconnectionReason>((Func<Server.DisconnectionReason, bool>) (r => r.Code == e.discReason));
          if (disconnectionReason == null)
            this._disconnectionReason = string.Format("Unknown disconnection reason {0}", (object) e.discReason);
        }
        if (disconnectionReason != null)
          this._disconnectionReason = disconnectionReason.Text;
        if (this._client.MsRdpClient.FullScreen)
        {
          this.ParentForm.LeaveFullScreenClient(this);
          this._client.MsRdpClient.FullScreen = false;
        }
        if (this.IsClientDocked)
        {
          if (this._client.Control.Visible)
            this._serverBox.Show();
          this.DestroyClient();
        }
        this.UpdateOnConnectionStateChange(ImageConstants.DisconnectedServer, RdpClient.ConnectionState.Disconnected);
      }
    }

    private void OnAutoReconnecting(object sender, IMsTscAxEvents_OnAutoReconnectingEvent e) => Log.Write("OnAutoReconnecting {0}: disconnectReason={1} attemptCount={2}", (object) this.DisplayName, (object) e.disconnectReason, (object) e.attemptCount);

    private void OnAutoReconnecting2(object sender, IMsTscAxEvents_OnAutoReconnecting2Event e) => Log.Write("OnAutoReconnecting2 {0}: disconnectReason={1} networkAvailable={2} attemptCount={3} maxAttemptCount={4}", (object) this.DisplayName, (object) e.disconnectReason, (object) e.networkAvailable, (object) e.attemptCount, (object) e.maxAttemptCount);

    private void OnAutoReconnected(object sender, EventArgs e) => Log.Write("OnAutoReconnected {0}", (object) this.DisplayName);

    private void UpdateOnConnectionStateChange(
      ImageConstants image,
      RdpClient.ConnectionState state)
    {
      using (Helpers.Timer("changing connection state of {0} to {1}", (object) this.DisplayName, (object) state))
      {
        this.ChangeImageIndex(image);
        this.ConnectionState = state;
        if (this._serverBox == null)
          return;
        this._serverBox.SetText();
      }
    }

    private void OnFocusReleased(object sender, IMsTscAxEvents_OnFocusReleasedEvent e)
    {
      Log.Write("OnFocusReleased {0}: direction={1}", (object) this.DisplayName, (object) e.iDirection);
      NodeHelper.SelectNewActiveConnection(e.iDirection == -1);
    }

    private void OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e) => e.pfAllowClose = true;

    private void OnRequestContainerMinimize(object sender, EventArgs e)
    {
      Log.Write("OnRequestContainerMinimize {0}", (object) this.DisplayName);
      this.ParentForm.WindowState = FormWindowState.Minimized;
    }

    private void OnRequestGoFullScreen(object sender, EventArgs e)
    {
      Log.Write("OnRequestGoFullScreen {0}", (object) this.DisplayName);
      if (this._noFullScreenBehavior > 0)
        return;
      this.ParentForm.GoFullScreenClient(this, Program.Preferences.FullScreenWindowIsTopMost);
    }

    public void SetClientSizeProperties()
    {
      if (this.IsClientFullScreen)
      {
        this.Client.AdvancedSettings2.SmartSizing = false;
      }
      else
      {
        this.InheritSettings();
        if (this.IsClientInPanel)
        {
          this.Client.AdvancedSettings2.SmartSizing = this.IsThumbnail | this.DisplaySettings.SmartSizeDockedWindow.Value;
        }
        else
        {
          if (!this.IsClientInitialized)
            return;
          this.Client.AdvancedSettings2.SmartSizing = this.DisplaySettings.SmartSizeUndockedWindow.Value;
        }
      }
    }

    private void OnRequestLeaveFullScreen(object sender, EventArgs e)
    {
      Log.Write("OnRequestLeaveFullScreen {0}", (object) this.DisplayName);
      if (this._noFullScreenBehavior > 0)
        return;
      this.ParentForm.LeaveFullScreenClient(this);
      if (this.IsThumbnail)
        return;
      this.SetNormalView();
    }

    private void OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e) => Log.Write("OnFatalError {0}: errorCode={1}", (object) this.DisplayName, (object) e.errorCode);

    public void AddServerRef(ServerRef serverRef) => this._serverRefList.Add(serverRef);

    public TServerRef FindServerRef<TServerRef>() where TServerRef : ServerRef => this._serverRefList.FirstOrDefault<ServerRef>((Func<ServerRef, bool>) (r => r is TServerRef)) as TServerRef;

    public TServerRef FindServerRef<TServerRef>(GroupBase parent) where TServerRef : ServerRef => this._serverRefList.FirstOrDefault<ServerRef>((Func<ServerRef, bool>) (r => r is TServerRef && r.Parent == parent)) as TServerRef;

    public void RemoveServerRef(ServerRef serverRef) => this._serverRefList.Remove(serverRef);

    public void VisitServerRefs(Action<ServerRef> action)
    {
      ServerRef[] array = new ServerRef[this._serverRefList.Count];
      this._serverRefList.CopyTo(array);
      ((IEnumerable<ServerRef>) array).ForEach<ServerRef>(action);
    }

    public override void ChangeImageIndex(ImageConstants index)
    {
      base.ChangeImageIndex(index);
      this.VisitServerRefs((Action<ServerRef>) (r => r.ChangeImageIndex(index)));
    }

    public void SendRemoteAction(RemoteSessionActionType action) => this._client.MsRdpClient8.SendRemoteAction(action);

    internal override void UpdateSettings(NodePropertiesDialog nodeDialog)
    {
      base.UpdateSettings(nodeDialog);
      if (!(nodeDialog is ServerPropertiesDialog))
        return;
      this.Text = this.DisplayName;
      if (this.TreeView == null)
        return;
      this.SetText();
      this.VisitServerRefs((Action<ServerRef>) (r => r.Text = this.Text));
    }

    internal void UpdateFromTemplate(Server template) => this.CopySettings((RdcTreeNode) template, typeof (ServerSettings));

    public override void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (ServerPropertiesDialog propertiesDialog = ServerPropertiesDialog.NewPropertiesDialog(this, parentForm))
      {
        propertiesDialog.SetActiveTab(activeTabName);
        if (propertiesDialog.ShowDialog() != DialogResult.OK)
          return;
        this.UpdateSettings((NodePropertiesDialog) propertiesDialog);
        ServerTree.Instance.OnNodeChanged((RdcTreeNode) this, ChangeType.PropertyChanged);
        ServerTree.Instance.OnGroupChanged(this.Parent as GroupBase, ChangeType.InvalidateUI);
      }
    }

    public override void CollectNodesToInvalidate(bool recurseChildren, HashSet<RdcTreeNode> set)
    {
      set.Add((RdcTreeNode) this);
      this._serverRefList.ForEach((Action<ServerRef>) (r => r.CollectNodesToInvalidate(recurseChildren, set)));
    }

    internal override void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("server");
      this.WriteXmlSettingsGroups(tw);
      tw.WriteEndElement();
    }

    public override bool CanDropOnTarget(RdcTreeNode targetNode)
    {
      if (!(targetNode is GroupBase groupBase))
        groupBase = targetNode.Parent as GroupBase;
      GroupBase groupBase1 = groupBase;
      if (groupBase1 == null || !groupBase1.CanDropServers())
        return false;
      return groupBase1.DropBehavior() == DragDropEffects.Copy || this.AllowEdit(false);
    }

    public override bool ConfirmRemove(bool askUser)
    {
      if (!this.IsConnected)
        return base.ConfirmRemove(askUser);
      FormTools.InformationDialog("There is an active session on " + this.Text + ". Disconnect it before removing the server.");
      return false;
    }

    private Size GetRemoteDesktopSize() => this.RemoteDesktopSettings.DesktopSizeSameAsClientAreaSize.Value ? (this.IsClientDocked ? Program.TheForm.GetClientSize() : this.ServerForm.ClientSize) : (this.RemoteDesktopSettings.DesktopSizeFullScreen.Value ? Screen.GetBounds((Control) this.ParentForm).Size : this.RemoteDesktopSettings.DesktopSize.Value);

    internal override void GoFullScreen()
    {
      if (!this.IsConnected)
        return;
      RdpClient client = this.Client;
      if (client == null)
        return;
      client.Control.Enabled = true;
      client.MsRdpClient.FullScreen = true;
    }

    internal override void LeaveFullScreen()
    {
      if (!this.IsConnected)
        return;
      RdpClient client = this.Client;
      if (client == null)
        return;
      client.MsRdpClient.FullScreen = false;
    }

    internal override void Undock()
    {
      if (!this.IsClientDocked)
        return;
      this.InitClient();
      Program.TheForm.RemoveFromClientPanel(this._client.Control);
      bool visible = this._client.Control.Visible;
      this._client.Control.Enabled = true;
      ServerForm form = new ServerForm(this);
      Program.PluginAction((Action<IPlugin>) (p => p.OnUndockServer((IUndockedServerForm) form)));
      Program.ShowForm((Form) form);
      this._serverBox.SetText();
      if (!visible)
        return;
      this._serverBox.Show();
    }

    internal override void Dock()
    {
      if (this.IsClientUndocked)
      {
        this.ServerForm.Close();
      }
      else
      {
        this._serverBox.SetText();
        if (!this.IsConnected)
        {
          this.DestroyClient();
        }
        else
        {
          Program.TheForm.AddToClientPanel(this._client.Control);
          this.SetClientSizeProperties();
          if (this._serverBox.Visible && !this.UseServerBox)
          {
            this._client.Control.Size = this._serverBox.Size;
            this._client.Control.Location = this._serverBox.Location;
            this._serverBox.Hide();
          }
          else
            this._client.Control.Hide();
          this.EnableDisableClient();
        }
      }
    }

    private void ClientGotFocus(object sender, EventArgs args)
    {
      Action<Server> focusReceived = Server.FocusReceived;
      if (focusReceived == null)
        return;
      focusReceived(this);
    }

    internal void EnableDisableClient()
    {
      if (!this.IsClientInitialized || !this.IsClientDocked)
        return;
      GroupBase parent = this.Parent as GroupBase;
      parent.InheritSettings();
      this.Client.Control.Enabled = !this.IsThumbnail || parent.DisplaySettings.AllowThumbnailSessionInteraction.Value;
    }

    private class DisconnectionReason
    {
      public readonly int Code;
      public readonly string Text;

      public DisconnectionReason(int code, string text)
      {
        this.Code = code;
        this.Text = text;
      }
    }
  }
}
