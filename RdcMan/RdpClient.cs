// Decompiled with JetBrains decompiler
// Type: RdcMan.RdpClient
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using MSTSCLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class RdpClient
  {
    public const int DefaultRDPPort = 3389;
    public const int DefaultVMConsoleConnectPort = 2179;
    public const int DefaultColorDepth = 24;
    public const int PerfDisableNothing = 0;
    public const int PerfDisableWallpaper = 1;
    public const int PerfDisableFullWindowDrag = 2;
    public const int PerfDisableMenuAnimations = 4;
    public const int PerfDisableTheming = 8;
    public const int PerfEnableEnhancedGraphics = 16;
    public const int PerfDisableCursorShadow = 32;
    public const int PerfDisableCursorBlinking = 64;
    public const int PerfEnableFontSmoothing = 128;
    public const int PerfEnableDesktopComposition = 256;
    public static bool SupportsGatewayCredentials = false;
    public static bool SupportsAdvancedAudioVideoRedirection = false;
    public static bool SupportsMonitorSpanning = false;
    public static bool SupportsPanning = false;
    public static bool SupportsFineGrainedRedirection = false;
    public static bool SupportsRemoteSessionActions = false;
    private Server _server;
    public static int MaxDesktopHeight;
    public static int MaxDesktopWidth;
    public static string RdpControlVersion;
    private static int RdpClientVersion;
    private static RdpClient StaticClient;
    private RdpClient5 _rdpClient5;
    private RdpClient6 _rdpClient6;
    private RdpClient7 _rdpClient7;
    private RdpClient8 _rdpClient8;

    public static string AudioRedirectionModeToString(RdpClient.AudioRedirectionMode mode)
    {
      switch (mode)
      {
        case RdpClient.AudioRedirectionMode.Client:
          return "Bring to this computer";
        case RdpClient.AudioRedirectionMode.Remote:
          return "Leave at remote computer";
        case RdpClient.AudioRedirectionMode.NoSound:
          return "Do not play";
        default:
          throw new Exception("Unexpected AudioRedirectionMode:" + mode.ToString());
      }
    }

    public static string AudioRedirectionQualityToString(RdpClient.AudioRedirectionQuality mode)
    {
      switch (mode)
      {
        case RdpClient.AudioRedirectionQuality.Dynamic:
          return "Dynamically adjusted";
        case RdpClient.AudioRedirectionQuality.High:
          return "High quality";
        case RdpClient.AudioRedirectionQuality.Medium:
          return "Medium quality";
        default:
          throw new Exception("Unexpected AudioRedirectionQuality:" + mode.ToString());
      }
    }

    public static string AudioCaptureRedirectionModeToString(
      RdpClient.AudioCaptureRedirectionMode mode)
    {
      switch (mode)
      {
        case RdpClient.AudioCaptureRedirectionMode.DoNotRecord:
          return "Do not record";
        case RdpClient.AudioCaptureRedirectionMode.Record:
          return "Record from this computer";
        default:
          throw new Exception("Unexpected AudioCaptureRedirectionMode:" + mode.ToString());
      }
    }

    public static string KeyboardHookModeToString(RdpClient.KeyboardHookMode mode)
    {
      switch (mode)
      {
        case RdpClient.KeyboardHookMode.Client:
          return "On the local computer";
        case RdpClient.KeyboardHookMode.Remote:
          return "On the remote computer";
        case RdpClient.KeyboardHookMode.FullScreenClient:
          return "In full screen mode only";
        default:
          throw new Exception("Unexpected KeyboardHookMode:" + mode.ToString());
      }
    }

    public static string GatewayLogonMethodToString(RdpClient.GatewayLogonMethod mode)
    {
      switch (mode)
      {
        case RdpClient.GatewayLogonMethod.NTLM:
          return "Ask for password (NTLM)";
        case RdpClient.GatewayLogonMethod.SmartCard:
          return "Smart card";
        case RdpClient.GatewayLogonMethod.Any:
          return "Allow me to select later";
        default:
          return (string) null;
      }
    }

    public static string GatewayUsageMethodToString(RdpClient.GatewayUsageMethod mode)
    {
      switch (mode)
      {
        case RdpClient.GatewayUsageMethod.NoneDirect:
          return "Do not use a Gateway server";
        case RdpClient.GatewayUsageMethod.NoneDetect:
          return "Automatically detect Gateway";
        default:
          throw new Exception("Unexpected GatewayUsageMethod:" + mode.ToString());
      }
    }

    public static string AuthenticationLevelToString(RdpClient.AuthenticationLevel mode)
    {
      switch (mode)
      {
        case RdpClient.AuthenticationLevel.None:
          return "Connect and don't warn if authentication fails";
        case RdpClient.AuthenticationLevel.Required:
          return "Do not connect if authentication fails";
        case RdpClient.AuthenticationLevel.Warn:
          return "Warn if authentication fails";
        default:
          throw new Exception("Unexpected AuthenticationLevel:" + mode.ToString());
      }
    }

    private RdpClient(MainForm form)
    {
      switch (RdpClient.RdpClientVersion)
      {
        case 6:
          this._rdpClient6 = new RdpClient6(form);
          break;
        case 7:
          this._rdpClient7 = new RdpClient7(form);
          break;
        case 8:
          this._rdpClient8 = new RdpClient8(form);
          break;
        default:
          this._rdpClient5 = new RdpClient5(form);
          break;
      }
    }

    public Size DesktopSize
    {
      get => new Size(this.MsRdpClient.DesktopWidth, this.MsRdpClient.DesktopHeight);
      set
      {
        this.MsRdpClient.DesktopHeight = Math.Min(RdpClient.MaxDesktopHeight, value.Height);
        this.MsRdpClient.DesktopWidth = Math.Min(RdpClient.MaxDesktopWidth, value.Width);
      }
    }

    internal static void Initialize(MainForm form)
    {
      using (RdpClient5 rdpClient5 = new RdpClient5(form))
      {
        RdpClient.RdpControlVersion = rdpClient5.Version;
        string[] strArray = rdpClient5.Version.Split('.');
        RdpClient.RdpClientVersion = 5;
        if (int.Parse(strArray[2]) >= 6001)
        {
          RdpClient.RdpClientVersion = 6;
          RdpClient.SupportsMonitorSpanning = true;
          if (int.Parse(strArray[2]) >= 7600)
          {
            RdpClient.RdpClientVersion = 7;
            if (int.Parse(strArray[2]) >= 9200)
              RdpClient.RdpClientVersion = 8;
          }
        }
        form.RemoveFromClientPanel((Control) rdpClient5);
      }
      RdpClient.StaticClient = new RdpClient(form);
      RdpClient staticClient = RdpClient.StaticClient;
      staticClient.Control.Enabled = false;
      RdpClient.MaxDesktopWidth = 4096;
      RdpClient.MaxDesktopHeight = 2048;
      if (staticClient.AdvancedSettings7 != null)
        RdpClient.SupportsGatewayCredentials = true;
      if (staticClient.AdvancedSettings8 != null)
        RdpClient.SupportsAdvancedAudioVideoRedirection = true;
      if (staticClient.ClientNonScriptable3 != null)
        RdpClient.SupportsFineGrainedRedirection = true;
      if (staticClient.MsRdpClient8 == null)
        return;
      RdpClient.SupportsRemoteSessionActions = true;
    }

    internal static RdpClient AllocClient(Server server, MainForm form) => new RdpClient(form)
    {
      _server = server
    };

    internal static void ReleaseClient(RdpClient client)
    {
      try
      {
        client._server = (Server) null;
        Program.TheForm.RemoveFromClientPanel(client.Control);
      }
      finally
      {
        AxHost rdpClient5 = (AxHost) client._rdpClient5;
        if (rdpClient5 != null)
        {
          client._rdpClient5 = (RdpClient5) null;
          rdpClient5.Dispose();
        }
        AxHost rdpClient6 = (AxHost) client._rdpClient6;
        if (rdpClient6 != null)
        {
          client._rdpClient6 = (RdpClient6) null;
          rdpClient6.Dispose();
        }
        AxHost rdpClient7 = (AxHost) client._rdpClient7;
        if (rdpClient7 != null)
        {
          client._rdpClient7 = (RdpClient7) null;
          rdpClient7.Dispose();
        }
        AxHost rdpClient8 = (AxHost) client._rdpClient8;
        if (rdpClient8 != null)
        {
          client._rdpClient8 = (RdpClient8) null;
          rdpClient8.Dispose();
        }
      }
    }

    public void SetText()
    {
      if (this._rdpClient8 != null)
      {
        this._rdpClient8.ConnectingText = this._server.ConnectingText;
        this._rdpClient8.DisconnectedText = this._server.DisconnectedText;
      }
      else if (this._rdpClient7 != null)
      {
        this._rdpClient7.ConnectingText = this._server.ConnectingText;
        this._rdpClient7.DisconnectedText = this._server.DisconnectedText;
      }
      else if (this._rdpClient6 != null)
      {
        this._rdpClient6.ConnectingText = this._server.ConnectingText;
        this._rdpClient6.DisconnectedText = this._server.DisconnectedText;
      }
      else
      {
        this._rdpClient5.ConnectingText = this._server.ConnectingText;
        this._rdpClient5.DisconnectedText = this._server.DisconnectedText;
      }
    }

    public Control Control
    {
      get
      {
        if (this._rdpClient8 != null)
          return (Control) this._rdpClient8;
        if (this._rdpClient7 != null)
          return (Control) this._rdpClient7;
        return this._rdpClient6 != null ? (Control) this._rdpClient6 : (Control) this._rdpClient5;
      }
    }

    public IMsRdpClient MsRdpClient => this.GetOcx() as IMsRdpClient;

    public IMsRdpClientAdvancedSettings AdvancedSettings2
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.AdvancedSettings2;
        if (this._rdpClient7 != null)
          return this._rdpClient7.AdvancedSettings2;
        return this._rdpClient6 != null ? this._rdpClient6.AdvancedSettings2 : this._rdpClient5.AdvancedSettings2;
      }
    }

    public IMsRdpClientAdvancedSettings4 AdvancedSettings5
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.AdvancedSettings5;
        if (this._rdpClient7 != null)
          return this._rdpClient7.AdvancedSettings5;
        return this._rdpClient6 != null ? this._rdpClient6.AdvancedSettings5 : this._rdpClient5.AdvancedSettings5;
      }
    }

    public IMsRdpClientAdvancedSettings5 AdvancedSettings6
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.AdvancedSettings6;
        if (this._rdpClient7 != null)
          return this._rdpClient7.AdvancedSettings6;
        return this._rdpClient6 != null ? this._rdpClient6.AdvancedSettings6 : this._rdpClient5.AdvancedSettings6;
      }
    }

    public IMsRdpClientAdvancedSettings6 AdvancedSettings7
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.AdvancedSettings7;
        if (this._rdpClient7 != null)
          return this._rdpClient7.AdvancedSettings7;
        return this._rdpClient6 != null ? this._rdpClient6.AdvancedSettings7 : (IMsRdpClientAdvancedSettings6) null;
      }
    }

    public IMsRdpClientAdvancedSettings7 AdvancedSettings8
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.AdvancedSettings8;
        return this._rdpClient7 != null ? this._rdpClient7.AdvancedSettings8 : (IMsRdpClientAdvancedSettings7) null;
      }
    }

    public IMsRdpClientNonScriptable3 ClientNonScriptable3 => this.GetOcx() as IMsRdpClientNonScriptable3;

    public IMsRdpClient8 MsRdpClient8 => this.MsRdpClient as IMsRdpClient8;

    public IMsRdpClientTransportSettings TransportSettings
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.TransportSettings;
        if (this._rdpClient7 != null)
          return this._rdpClient7.TransportSettings;
        return this._rdpClient6 != null ? this._rdpClient6.TransportSettings : this._rdpClient5.TransportSettings;
      }
    }

    public IMsRdpClientTransportSettings2 TransportSettings2
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.TransportSettings2;
        if (this._rdpClient7 != null)
          return this._rdpClient7.TransportSettings2;
        return this._rdpClient6 != null ? this._rdpClient6.TransportSettings2 : (IMsRdpClientTransportSettings2) null;
      }
    }

    public IMsTscSecuredSettings SecuredSettings
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.SecuredSettings;
        if (this._rdpClient7 != null)
          return this._rdpClient7.SecuredSettings;
        return this._rdpClient6 != null ? this._rdpClient6.SecuredSettings : this._rdpClient5.SecuredSettings;
      }
    }

    public IMsRdpClientSecuredSettings SecuredSettings2
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.SecuredSettings2;
        if (this._rdpClient7 != null)
          return this._rdpClient7.SecuredSettings2;
        return this._rdpClient6 != null ? this._rdpClient6.SecuredSettings2 : this._rdpClient5.SecuredSettings2;
      }
    }

    public ITSRemoteProgram RemoteProgram
    {
      get
      {
        if (this._rdpClient8 != null)
          return this._rdpClient8.RemoteProgram;
        if (this._rdpClient7 != null)
          return this._rdpClient7.RemoteProgram;
        return this._rdpClient6 != null ? this._rdpClient6.RemoteProgram : this._rdpClient5.RemoteProgram;
      }
    }

    public object GetOcx()
    {
      if (this._rdpClient8 != null)
        return this._rdpClient8.GetOcx();
      if (this._rdpClient7 != null)
        return this._rdpClient7.GetOcx();
      return this._rdpClient6 != null ? this._rdpClient6.GetOcx() : this._rdpClient5.GetOcx();
    }

    public static IMsRdpDriveCollection DriveCollection => RdpClient.StaticClient.ClientNonScriptable3.DriveCollection;

    public static IMsRdpDeviceCollection DeviceCollection => RdpClient.StaticClient.ClientNonScriptable3.DeviceCollection;

    public void ConnectConnectionHandlers(
      EventHandler onConnected,
      EventHandler onConnecting,
      AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler onDisconnected,
      AxMSTSCLib.IMsTscAxEvents_OnAutoReconnectingEventHandler onAutoReconnecting,
      AxMSTSCLib.IMsTscAxEvents_OnAutoReconnecting2EventHandler onAutoReconnecting2,
      EventHandler onAutoReconnected,
      AxMSTSCLib.IMsTscAxEvents_OnFocusReleasedEventHandler onFocusReleased)
    {
      if (this._rdpClient8 != null)
      {
        this._rdpClient8.OnConnected += onConnected;
        this._rdpClient8.OnConnecting += onConnecting;
        this._rdpClient8.OnDisconnected += onDisconnected;
        this._rdpClient8.OnAutoReconnecting += onAutoReconnecting;
        this._rdpClient8.OnAutoReconnecting2 += onAutoReconnecting2;
        this._rdpClient8.OnAutoReconnected += onAutoReconnected;
        this._rdpClient8.OnFocusReleased += onFocusReleased;
      }
      else if (this._rdpClient7 != null)
      {
        this._rdpClient7.OnConnected += onConnected;
        this._rdpClient7.OnConnecting += onConnecting;
        this._rdpClient7.OnDisconnected += onDisconnected;
        this._rdpClient7.OnAutoReconnecting += onAutoReconnecting;
        this._rdpClient7.OnAutoReconnecting2 += onAutoReconnecting2;
        this._rdpClient7.OnAutoReconnected += onAutoReconnected;
        this._rdpClient7.OnFocusReleased += onFocusReleased;
      }
      else if (this._rdpClient6 != null)
      {
        this._rdpClient6.OnConnected += onConnected;
        this._rdpClient6.OnConnecting += onConnecting;
        this._rdpClient6.OnDisconnected += onDisconnected;
        this._rdpClient6.OnAutoReconnecting += onAutoReconnecting;
        this._rdpClient6.OnAutoReconnecting2 += onAutoReconnecting2;
        this._rdpClient6.OnAutoReconnected += onAutoReconnected;
        this._rdpClient6.OnFocusReleased += onFocusReleased;
      }
      else
      {
        this._rdpClient5.OnConnected += onConnected;
        this._rdpClient5.OnConnecting += onConnecting;
        this._rdpClient5.OnDisconnected += onDisconnected;
        this._rdpClient5.OnAutoReconnecting += onAutoReconnecting;
        this._rdpClient5.OnAutoReconnecting2 += onAutoReconnecting2;
        this._rdpClient5.OnAutoReconnected += onAutoReconnected;
        this._rdpClient5.OnFocusReleased += onFocusReleased;
      }
    }

    public void DisconnectConnectionHandlers(
      EventHandler onConnected,
      EventHandler onConnecting,
      AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler onDisconnected,
      AxMSTSCLib.IMsTscAxEvents_OnAutoReconnectingEventHandler onAutoReconnecting,
      AxMSTSCLib.IMsTscAxEvents_OnAutoReconnecting2EventHandler onAutoReconnecting2,
      EventHandler onAutoReconnected,
      AxMSTSCLib.IMsTscAxEvents_OnFocusReleasedEventHandler onFocusReleased)
    {
      if (this._rdpClient8 != null)
      {
        this._rdpClient8.OnConnected -= onConnected;
        this._rdpClient8.OnConnecting -= onConnecting;
        this._rdpClient8.OnDisconnected -= onDisconnected;
        this._rdpClient8.OnAutoReconnecting -= onAutoReconnecting;
        this._rdpClient8.OnAutoReconnecting2 -= onAutoReconnecting2;
        this._rdpClient8.OnAutoReconnected -= onAutoReconnected;
        this._rdpClient8.OnFocusReleased -= onFocusReleased;
      }
      else if (this._rdpClient7 != null)
      {
        this._rdpClient7.OnConnected -= onConnected;
        this._rdpClient7.OnConnecting -= onConnecting;
        this._rdpClient7.OnDisconnected -= onDisconnected;
        this._rdpClient7.OnAutoReconnecting -= onAutoReconnecting;
        this._rdpClient7.OnAutoReconnecting2 -= onAutoReconnecting2;
        this._rdpClient7.OnAutoReconnected -= onAutoReconnected;
        this._rdpClient7.OnFocusReleased -= onFocusReleased;
      }
      else if (this._rdpClient6 != null)
      {
        this._rdpClient6.OnConnected -= onConnected;
        this._rdpClient6.OnConnecting -= onConnecting;
        this._rdpClient6.OnDisconnected -= onDisconnected;
        this._rdpClient6.OnAutoReconnecting -= onAutoReconnecting;
        this._rdpClient6.OnAutoReconnecting2 -= onAutoReconnecting2;
        this._rdpClient6.OnAutoReconnected -= onAutoReconnected;
        this._rdpClient6.OnFocusReleased -= onFocusReleased;
      }
      else
      {
        this._rdpClient5.OnConnected -= onConnected;
        this._rdpClient5.OnConnecting -= onConnecting;
        this._rdpClient5.OnDisconnected -= onDisconnected;
        this._rdpClient5.OnAutoReconnecting -= onAutoReconnecting;
        this._rdpClient5.OnAutoReconnecting2 -= onAutoReconnecting2;
        this._rdpClient5.OnAutoReconnected -= onAutoReconnected;
        this._rdpClient5.OnFocusReleased -= onFocusReleased;
      }
    }

    public void ConnectContainerHandlers(
      EventHandler onRequestGoFullScreen,
      EventHandler onRequestLeaveFullScreen,
      EventHandler onRequestContainerMinimize,
      AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler onConfirmClose,
      AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler onFatalError)
    {
      if (this._rdpClient8 != null)
      {
        this._rdpClient8.OnRequestGoFullScreen += onRequestGoFullScreen;
        this._rdpClient8.OnRequestLeaveFullScreen += onRequestLeaveFullScreen;
        this._rdpClient8.OnRequestContainerMinimize += onRequestContainerMinimize;
        this._rdpClient8.OnConfirmClose += onConfirmClose;
        this._rdpClient8.OnFatalError += onFatalError;
      }
      else if (this._rdpClient7 != null)
      {
        this._rdpClient7.OnRequestGoFullScreen += onRequestGoFullScreen;
        this._rdpClient7.OnRequestLeaveFullScreen += onRequestLeaveFullScreen;
        this._rdpClient7.OnRequestContainerMinimize += onRequestContainerMinimize;
        this._rdpClient7.OnConfirmClose += onConfirmClose;
        this._rdpClient7.OnFatalError += onFatalError;
      }
      else if (this._rdpClient6 != null)
      {
        this._rdpClient6.OnRequestGoFullScreen += onRequestGoFullScreen;
        this._rdpClient6.OnRequestLeaveFullScreen += onRequestLeaveFullScreen;
        this._rdpClient6.OnRequestContainerMinimize += onRequestContainerMinimize;
        this._rdpClient6.OnConfirmClose += onConfirmClose;
        this._rdpClient6.OnFatalError += onFatalError;
      }
      else
      {
        this._rdpClient5.OnRequestGoFullScreen += onRequestGoFullScreen;
        this._rdpClient5.OnRequestLeaveFullScreen += onRequestLeaveFullScreen;
        this._rdpClient5.OnRequestContainerMinimize += onRequestContainerMinimize;
        this._rdpClient5.OnConfirmClose += onConfirmClose;
        this._rdpClient5.OnFatalError += onFatalError;
      }
    }

    public void DisconnectContainerHandlers(
      EventHandler onRequestGoFullScreen,
      EventHandler onRequestLeaveFullScreen,
      EventHandler onRequestContainerMinimize,
      AxMSTSCLib.IMsTscAxEvents_OnConfirmCloseEventHandler onConfirmClose,
      AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler onFatalError)
    {
      if (this._rdpClient8 != null)
      {
        this._rdpClient8.OnRequestGoFullScreen -= onRequestGoFullScreen;
        this._rdpClient8.OnRequestLeaveFullScreen -= onRequestLeaveFullScreen;
        this._rdpClient8.OnRequestContainerMinimize -= onRequestContainerMinimize;
        this._rdpClient8.OnConfirmClose -= onConfirmClose;
        this._rdpClient8.OnFatalError -= onFatalError;
      }
      else if (this._rdpClient7 != null)
      {
        this._rdpClient7.OnRequestGoFullScreen -= onRequestGoFullScreen;
        this._rdpClient7.OnRequestLeaveFullScreen -= onRequestLeaveFullScreen;
        this._rdpClient7.OnRequestContainerMinimize -= onRequestContainerMinimize;
        this._rdpClient7.OnConfirmClose -= onConfirmClose;
        this._rdpClient7.OnFatalError -= onFatalError;
      }
      else if (this._rdpClient6 != null)
      {
        this._rdpClient6.OnRequestGoFullScreen -= onRequestGoFullScreen;
        this._rdpClient6.OnRequestLeaveFullScreen -= onRequestLeaveFullScreen;
        this._rdpClient6.OnRequestContainerMinimize -= onRequestContainerMinimize;
        this._rdpClient6.OnConfirmClose -= onConfirmClose;
        this._rdpClient6.OnFatalError -= onFatalError;
      }
      else
      {
        this._rdpClient5.OnRequestGoFullScreen -= onRequestGoFullScreen;
        this._rdpClient5.OnRequestLeaveFullScreen -= onRequestLeaveFullScreen;
        this._rdpClient5.OnRequestContainerMinimize -= onRequestContainerMinimize;
        this._rdpClient5.OnConfirmClose -= onConfirmClose;
        this._rdpClient5.OnFatalError -= onFatalError;
      }
    }

    public void Dump()
    {
      try
      {
        if (this._rdpClient8 != null)
        {
          Log.DumpObject<IMsRdpClientAdvancedSettings8>(this._rdpClient8.AdvancedSettings9);
          Log.DumpObject<IMsRdpClientSecuredSettings2>(this._rdpClient8.SecuredSettings3);
          Log.DumpObject<IMsRdpClientTransportSettings3>(this._rdpClient8.TransportSettings3);
          Log.DumpObject<IMsRdpClientNonScriptable5>((IMsRdpClientNonScriptable5) this._rdpClient8.GetOcx());
        }
        else if (this._rdpClient7 != null)
        {
          Log.DumpObject<IMsRdpClientAdvancedSettings7>(this._rdpClient7.AdvancedSettings8);
          Log.DumpObject<IMsRdpClientSecuredSettings2>(this._rdpClient7.SecuredSettings3);
          Log.DumpObject<IMsRdpClientTransportSettings3>(this._rdpClient7.TransportSettings3);
          Log.DumpObject<IMsRdpClientNonScriptable5>((IMsRdpClientNonScriptable5) this._rdpClient7.GetOcx());
        }
        else if (this._rdpClient6 != null)
        {
          Log.DumpObject<IMsRdpClientAdvancedSettings6>(this._rdpClient6.AdvancedSettings7);
          Log.DumpObject<IMsRdpClientSecuredSettings>(this._rdpClient6.SecuredSettings2);
          Log.DumpObject<IMsRdpClientTransportSettings2>(this._rdpClient6.TransportSettings2);
          Log.DumpObject<IMsRdpClientNonScriptable4>((IMsRdpClientNonScriptable4) this._rdpClient6.GetOcx());
        }
        else
        {
          if (this._rdpClient5 == null)
            return;
          Log.DumpObject<IMsRdpClientAdvancedSettings5>(this._rdpClient5.AdvancedSettings6);
          Log.DumpObject<IMsRdpClientSecuredSettings>(this._rdpClient5.SecuredSettings2);
          Log.DumpObject<IMsRdpClientTransportSettings>(this._rdpClient5.TransportSettings);
          Log.DumpObject<IMsRdpClientNonScriptable4>((IMsRdpClientNonScriptable4) this._rdpClient5.GetOcx());
        }
      }
      catch
      {
      }
    }

    public enum ConnectionState
    {
      Disconnected,
      Connecting,
      Connected,
    }

    public enum ConnectionBarState
    {
      AutoHide,
      Pinned,
      Off,
    }

    public enum AudioRedirectionMode
    {
      Client,
      Remote,
      NoSound,
    }

    public enum AudioRedirectionQuality
    {
      Dynamic,
      High,
      Medium,
    }

    public enum AudioCaptureRedirectionMode
    {
      DoNotRecord,
      Record,
    }

    public enum KeyboardHookMode
    {
      Client,
      Remote,
      FullScreenClient,
    }

    public enum GatewayUsageMethod
    {
      NoneDirect,
      ProxyDirect,
      ProxyDetect,
      Default,
      NoneDetect,
    }

    public enum GatewayLogonMethod
    {
      NTLM = 0,
      SmartCard = 1,
      Any = 4,
    }

    public enum AuthenticationLevel
    {
      None,
      Required,
      Warn,
    }

    public delegate void VoidDelegate();
  }
}
