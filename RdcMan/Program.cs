// Decompiled with JetBrains decompiler
// Type: RdcMan.Program
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using RdcMan.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  internal class Program
  {
    private const string PluginPattern = "Plugin.*.dll";
    internal static CredentialsStore CredentialsProfiles = new CredentialsStore();
    private static ApplicationContext _appContext;
    public static bool ResetPreferences = false;
    private static bool _openFiles = true;
    private static ReconnectServerOptions _reconnectServersAtStart = ReconnectServerOptions.Ask;
    private static readonly List<string> _filesToOpen = new List<string>();
    private static string[] _serversToConnect = (string[]) null;
    private static readonly List<IBuiltInVirtualGroup> _builtInVirtualGroups = new List<IBuiltInVirtualGroup>();

    internal static MainForm TheForm { get; set; }

    internal static Preferences Preferences { get; private set; }

    internal static ManualResetEvent InitializedEvent { get; private set; }

    internal static IEnumerable<IBuiltInVirtualGroup> BuiltInVirtualGroups => (IEnumerable<IBuiltInVirtualGroup>) Program._builtInVirtualGroups;

    private static Dictionary<string, Program.PluginConfig> Plugins { get; set; }

    private static PluginContext PluginContext { get; set; }

    public static void PluginAction(Action<IPlugin> action) => Program.Plugins.Values.ForEach<Program.PluginConfig>((Action<Program.PluginConfig>) (v =>
    {
      IPlugin plugin = v.Plugin;
      if (plugin == null)
        return;
      action(plugin);
    }));

    [STAThread]
    internal static void Main(params string[] args)
    {
      Application.EnableVisualStyles();
      Policies.Read();
      using (Helpers.Timer("parsing command line"))
        Program.ParseCommandLine();
      try
      {
        Current.Read();
      }
      catch (Exception ex)
      {
        FormTools.ErrorDialog("Error reading RDCMan configuration file: {0} The program may not be unstable and/or not fully functional.".InvariantFormat((object) ex.Message));
      }
      using (CompositionContainer compositionContainer = new CompositionContainer((ComposablePartCatalog) new AssemblyCatalog(Assembly.GetCallingAssembly()), new ExportProvider[0]))
      {
        Program._builtInVirtualGroups.AddRange(compositionContainer.GetExportedValues<IBuiltInVirtualGroup>());
        Program._builtInVirtualGroups.Sort((Comparison<IBuiltInVirtualGroup>) ((a, b) => a.Text.CompareTo(b.Text)));
      }
      using (Helpers.Timer("reading preferences"))
      {
        Program.Preferences = Preferences.Load();
        if (Program.Preferences == null)
          Environment.Exit(1);
      }
      Thread thread;
      using (Helpers.Timer("starting message loop thread"))
      {
        Program.InitializedEvent = new ManualResetEvent(false);
        thread = new Thread(new ThreadStart(Program.StartMessageLoop))
        {
          IsBackground = true
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Program.InitializedEvent.WaitOne();
      }
      if (Program.TheForm == null)
        Environment.Exit(1);
      Program.TheForm.Invoke((Delegate) new MethodInvoker(Program.CompleteInitialization));
      thread.Join();
      Log.Write("Exiting");
    }

    private static void CompleteInitialization()
    {
      Program.InstantiatePlugins();
      if (Program._filesToOpen.Count > 0)
        Program.Preferences.FilesToOpen = Program._filesToOpen;
      else if (!Program._openFiles)
        Program.Preferences.FilesToOpen = (List<string>) null;
      List<ServerBase> connectedServers = new List<ServerBase>();
      ServerTree.Instance.Operation(OperationBehavior.SuspendSort, (Action) (() =>
      {
        foreach (IBuiltInVirtualGroup builtInGroup in Program.BuiltInVirtualGroups.Where<IBuiltInVirtualGroup>((Func<IBuiltInVirtualGroup, bool>) (group => group.IsVisibilityConfigurable)))
          builtInGroup.IsInTree = Program.Preferences.GetBuiltInGroupVisibility(builtInGroup);
      }));
      Program.OpenFiles();
      ServerTree.Instance.Operation(OperationBehavior.SuspendGroupChanged, (Action) (() =>
      {
        Program.Preferences.LoadBuiltInGroups();
        ConnectedGroup.Instance.Nodes.ForEach((Action<TreeNode>) (n => connectedServers.Add((ServerBase) ((ServerBase) n).ServerNode)));
        ConnectedGroup.Instance.RemoveChildren();
        ServerTree.Instance.SortBuiltinGroups();
      }));
      ServerTree.Instance.Show();
      ServerTree.Instance.Focus();
      bool isFirstConnection = Program.ReconnectAtStartup(connectedServers);
      if (Program._serversToConnect != null)
        Program.ConnectNamedServers((ICollection<string>) Program._serversToConnect, isFirstConnection);
      if (Program.Preferences.ServerTreeVisibility != ControlVisibility.Dock)
        ServerTree.Instance.Hide();
      Program.PluginAction((Action<IPlugin>) (p => p.PostLoad((IPluginContext) Program.PluginContext)));
      Program.Preferences.NeedToSave = false;
      Program.TheForm.UpdateAutoSaveTimer();
      ThreadPool.QueueUserWorkItem((WaitCallback) (o => Program.CheckForUpdate()));
      Log.Write("Startup completed");
    }

    private static bool ReconnectAtStartup(List<ServerBase> connectedServers)
    {
      IEnumerable<ServerBase> reconnectServers = Enumerable.Empty<ServerBase>();
      switch (Program._reconnectServersAtStart)
      {
        case ReconnectServerOptions.Ask:
          if (Program.Preferences.ReconnectOnStartup && connectedServers.Any<ServerBase>())
          {
            reconnectServers = (IEnumerable<ServerBase>) new List<ServerBase>(Program.ConnectServersDialog((IEnumerable<ServerBase>) connectedServers));
            break;
          }
          break;
        case ReconnectServerOptions.All:
          reconnectServers = (IEnumerable<ServerBase>) connectedServers;
          break;
        case ReconnectServerOptions.None:
          return false;
      }
      return Program.ConnectServers(reconnectServers, true);
    }

    private static void InstantiatePlugins()
    {
      Program.PluginContext = new PluginContext();
      CompositionContainer compositionContainer = new CompositionContainer((ComposablePartCatalog) new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Plugin.*.dll"), new ExportProvider[0]);
      Program.Plugins = new Dictionary<string, Program.PluginConfig>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      StringBuilder stringBuilder = new StringBuilder();
      XmlNode xmlNode = Program.Preferences.Settings.PluginSettings.Value;
      if (xmlNode != null)
      {
        foreach (XmlNode selectNode in xmlNode.SelectNodes("//plugin"))
        {
          try
          {
            string key = selectNode.Attributes["path"].Value;
            if (!string.IsNullOrEmpty(key))
            {
              Program.PluginConfig pluginConfig = new Program.PluginConfig()
              {
                Name = key,
                SettingsNode = selectNode
              };
              Program.Plugins[key] = pluginConfig;
            }
          }
          catch
          {
          }
        }
      }
      foreach (IPlugin plugin in compositionContainer.GetExports<IPlugin>().Select<Lazy<IPlugin>, IPlugin>((Func<Lazy<IPlugin>, IPlugin>) (e => e.Value)))
      {
        string name = plugin.GetType().Assembly.GetName().Name;
        Program.PluginConfig pluginConfig;
        if (!Program.Plugins.TryGetValue(name, out pluginConfig))
        {
          pluginConfig = new Program.PluginConfig()
          {
            Name = name
          };
          Program.Plugins[name] = pluginConfig;
        }
        try
        {
          plugin.PreLoad((IPluginContext) Program.PluginContext, pluginConfig.SettingsNode);
          pluginConfig.Plugin = plugin;
        }
        catch (Exception ex)
        {
          stringBuilder.AppendLine("Error loading '{0}': {1}".InvariantFormat((object) name, (object) ex.Message));
        }
      }
      foreach (Program.PluginConfig pluginConfig in Program.Plugins.Values.Where<Program.PluginConfig>((Func<Program.PluginConfig, bool>) (c => c.Plugin == null)))
        stringBuilder.AppendLine("'{0}' was previously used but is not loaded now".InvariantFormat((object) pluginConfig.Name));
      if (stringBuilder.Length <= 0)
        return;
      stringBuilder.AppendLine().Append("Click Cancel to exit");
      if (FormTools.ExclamationDialog("Some plugins have not been loaded. RDCMan may not function as you expect.{0}{0}".InvariantFormat((object) Environment.NewLine) + stringBuilder.ToString(), MessageBoxButtons.OKCancel) != DialogResult.Cancel)
        return;
      Environment.Exit(1);
    }

    private static void OpenFiles()
    {
      List<string> filesToOpen = Program.Preferences.FilesToOpen;
      if (filesToOpen == null)
        return;
      bool flag = true;
      foreach (string filename in filesToOpen)
      {
        FileGroup fileGroup = RdgFile.OpenFile(filename);
        if (fileGroup != null && flag)
        {
          flag = false;
          ServerTree.Instance.SelectedNode = (TreeNode) fileGroup;
        }
      }
    }

    internal static IEnumerable<ServerBase> ConnectServersDialog(
      IEnumerable<ServerBase> servers)
    {
      using (RdcMan.ConnectServersDialog connectServersDialog = new RdcMan.ConnectServersDialog(servers))
        return connectServersDialog.ShowDialog((IWin32Window) Program.TheForm) == DialogResult.OK ? (IEnumerable<ServerBase>) connectServersDialog.SelectedServers.ToList<ServerBase>() : (IEnumerable<ServerBase>) new ServerBase[0];
    }

    internal static bool ConnectServers(
      IEnumerable<ServerBase> reconnectServers,
      bool isFirstConnection)
    {
      NodeHelper.ThrottledConnect(reconnectServers, (Action<ServerBase>) (server =>
      {
        if (!isFirstConnection)
          return;
        ServerTree.Instance.SelectedNode = (TreeNode) server;
        server.Focus();
        isFirstConnection = false;
      }));
      return isFirstConnection;
    }

    private static bool ConnectNamedServers(ICollection<string> serverNames, bool isFirstConnection)
    {
      HashSet<string> nameHash = new HashSet<string>((IEnumerable<string>) serverNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<ServerBase> serversToConnect = new List<ServerBase>();
      ServerTree.Instance.Nodes.VisitNodes((Action<RdcTreeNode>) (node =>
      {
        if (!(node is Server server) || !nameHash.Contains(server.ServerName))
          return;
        if (!server.IsConnected)
          serversToConnect.Add((ServerBase) server);
        nameHash.Remove(server.ServerName);
      }));
      isFirstConnection = Program.ConnectServers((IEnumerable<ServerBase>) serversToConnect, isFirstConnection);
      if (nameHash.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder("The following servers were not found and could not be connected:").AppendLine().AppendLine();
        foreach (string str in nameHash)
          stringBuilder.AppendLine(str);
        FormTools.InformationDialog(stringBuilder.ToString());
      }
      return isFirstConnection;
    }

    private static void ParseCommandLine()
    {
      ArgumentParser argumentParser = new ArgumentParser();
      argumentParser.AddSwitch("?", false);
      argumentParser.AddSwitch("h", false);
      argumentParser.AddSwitch("reset", false);
      argumentParser.AddSwitch("noopen", false);
      argumentParser.AddSwitch("noconnect", false);
      argumentParser.AddSwitch("reconnect", false);
      argumentParser.AddSwitch("c", true);
      try
      {
        argumentParser.Parse();
      }
      catch (ArgumentException ex)
      {
        FormTools.ErrorDialog(ex.Message);
        Environment.Exit(1);
      }
      if (argumentParser.HasSwitch("?") || argumentParser.HasSwitch("h"))
      {
        Program.Usage();
        Environment.Exit(0);
      }
      if (argumentParser.HasSwitch("reset"))
        Program.ResetPreferences = true;
      if (argumentParser.HasSwitch("noopen"))
        Program._openFiles = false;
      if (argumentParser.HasSwitch("noconnect"))
        Program._reconnectServersAtStart = ReconnectServerOptions.None;
      if (argumentParser.HasSwitch("reconnect"))
        Program._reconnectServersAtStart = ReconnectServerOptions.All;
      if (argumentParser.HasSwitch("c"))
        Program._serversToConnect = argumentParser.SwitchValues["c"].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
      Program._filesToOpen.AddRange((IEnumerable<string>) argumentParser.PlainArgs);
    }

    internal static void Usage() => Process.Start("IExplore.exe", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\help.htm").Replace('\\', '/'));

    private static void StartMessageLoop()
    {
      try
      {
        if (MainForm.Create() == null)
          return;
        Log.Init();
        Program._appContext = new ApplicationContext((Form) Program.TheForm);
        Application.Run(Program._appContext);
      }
      finally
      {
        Program.InitializedEvent.Set();
      }
    }

    internal static void ShowForm(Form form) => Program._appContext.MainForm.Invoke((Delegate) new Program.ShowFormDelegate(Program.ShowFormWorker), (object) form);

    private static void ShowFormWorker(Form form)
    {
      form.Show();
      form.BringToFront();
    }

    private static void CheckForUpdate()
    {
      try
      {
        ProgramUpdateElement updateElement = Current.RdcManSection.ProgramUpdate;
        if (string.IsNullOrEmpty(updateElement.VersionPath) || string.IsNullOrEmpty(updateElement.UpdateUrl))
          return;
        DateTime result1;
        if (!DateTime.TryParse(Program.Preferences.LastUpdateCheckTimeUtc, out result1) || DateTime.UtcNow.Subtract(result1).TotalDays < 1.0)
        {
          Log.Write("Last checked for update on {0}, not checking until tomorrow", (object) result1.ToString("s"));
        }
        else
        {
          Program.Preferences.LastUpdateCheckTimeUtc = DateTime.UtcNow.ToString("u");
          Version result2;
          if (!Version.TryParse(File.ReadAllText(updateElement.VersionPath), out result2))
            return;
          AssemblyName name = Assembly.GetExecutingAssembly().GetName();
          Log.Write("Latest version = {0}", (object) result2);
          if (!(name.Version < result2))
            return;
          Program.TheForm.Invoke((Delegate) (() => FormTools.InformationDialog("There is a new version of RDCMan available from {0}".InvariantFormat((object) updateElement.UpdateUrl))));
        }
      }
      catch (Exception ex)
      {
      }
    }

    private delegate void ShowFormDelegate(Form form);

    private class PluginConfig
    {
      public IPlugin Plugin { get; set; }

      public string Name { get; set; }

      public XmlNode SettingsNode { get; set; }
    }
  }
}
