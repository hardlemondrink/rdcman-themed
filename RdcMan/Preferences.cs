// Decompiled with JetBrains decompiler
// Type: RdcMan.Preferences
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  [SettingsProvider(typeof (RdcSettingsProvider))]
  public sealed class Preferences : ApplicationSettingsBase
  {
    public bool NeedToSave { get; set; }

    public GlobalSettings Settings { get; private set; }

    private Preferences()
    {
      this.Settings = new GlobalSettings();
      string name = Assembly.GetExecutingAssembly().GetName().Name;
      this.SettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.CompanyName, Application.ProductName);
      this.SettingsPath = Path.Combine(this.SettingsDirectory, name + ".settings");
    }

    public object GetTransferValue(string name) => base[name];

    public override object this[string propertyName]
    {
      get => this.Settings.GetValue(propertyName);
      set => this.Settings.SetValue(propertyName, value);
    }

    private string SettingsDirectory { get; set; }

    private string SettingsPath { get; set; }

    public static Preferences Load()
    {
      Preferences prefs = new Preferences();
      if (Program.ResetPreferences)
        return prefs;
      List<string> stringList = new List<string>();
      string str1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft Corporation", Application.ProductName);
      string str2 = Path.Combine(str1, "RDCMan.settings");
      if (File.Exists(str2))
      {
        if (!File.Exists(prefs.SettingsPath))
        {
          Directory.CreateDirectory(prefs.SettingsDirectory);
          File.Move(str2, prefs.SettingsPath);
        }
        try
        {
          Directory.Delete(str1);
        }
        catch
        {
        }
      }
      bool flag = true;
      try
      {
        using (XmlTextReader xmlTextReader = new XmlTextReader(prefs.SettingsPath))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load((XmlReader) xmlTextReader);
          XmlNode lastChild = xmlDocument.LastChild;
          try
          {
            string str3 = lastChild.Attributes["programVersion"].Value;
            prefs.Settings.ReadXml(lastChild, (RdcTreeNode) null, (ICollection<string>) stringList);
            flag = false;
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
      if (flag)
      {
        prefs.Settings.TransferPreferences(prefs);
        if (prefs.DefaultGroupSettings != null)
        {
          XmlTextReader xmlTextReader = new XmlTextReader((Stream) new MemoryStream(prefs.DefaultGroupSettings))
          {
            WhitespaceHandling = WhitespaceHandling.None
          };
          int content = (int) xmlTextReader.MoveToContent();
          XmlNode xmlNode = new XmlDocument().ReadNode((XmlReader) xmlTextReader);
          xmlTextReader.Close();
          GroupBase.SchemaVersion = 2;
          DefaultSettingsGroup.Instance.ReadXml(xmlNode, (ICollection<string>) stringList);
        }
        if (prefs.CredentialsProfiles != null)
        {
          XmlTextReader xmlTextReader = new XmlTextReader((Stream) new MemoryStream(prefs.CredentialsProfiles))
          {
            WhitespaceHandling = WhitespaceHandling.None
          };
          int content = (int) xmlTextReader.MoveToContent();
          XmlNode xmlNode = new XmlDocument().ReadNode((XmlReader) xmlTextReader);
          xmlTextReader.Close();
          Program.CredentialsProfiles.ReadXml(xmlNode, ProfileScope.Global, (RdcTreeNode) DefaultSettingsGroup.Instance, (ICollection<string>) stringList);
        }
      }
      else
      {
        if (prefs.Settings.DefaultGroupSettings != null)
          DefaultSettingsGroup.Instance.ReadXml(prefs.Settings.DefaultGroupSettings.Value.FirstChild, (ICollection<string>) stringList);
        if (prefs.Settings.CredentialsProfiles != null)
        {
          XmlNode firstChild = prefs.Settings.CredentialsProfiles.Value.FirstChild;
          Program.CredentialsProfiles.ReadXml(firstChild, ProfileScope.Global, (RdcTreeNode) DefaultSettingsGroup.Instance, (ICollection<string>) stringList);
        }
      }
      Encryption.DecryptPasswords();
      if (stringList.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder("The following errors were encountered:").AppendLine().AppendLine();
        foreach (string str3 in stringList)
          stringBuilder.AppendLine(str3);
        stringBuilder.AppendLine().Append("Your global configuration has not loaded completely. If it is saved it almost certainly means losing information. Continue?");
        if (FormTools.ExclamationDialog(stringBuilder.ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
          return (Preferences) null;
      }
      return prefs;
    }

    public void LoadBuiltInGroups()
    {
      if (this.Settings.BuiltInGroups.Value == null)
        return;
      GroupBase.SchemaVersion = 3;
      XmlNode firstChild = this.Settings.BuiltInGroups.Value.FirstChild;
      List<string> stringList = new List<string>();
      foreach (XmlNode childNode1 in firstChild.ChildNodes)
      {
        XmlNode childNode = childNode1;
        Program.BuiltInVirtualGroups.Where<IBuiltInVirtualGroup>((Func<IBuiltInVirtualGroup, bool>) (v => childNode.Name.Equals(v.XmlNodeName))).FirstOrDefault<IBuiltInVirtualGroup>()?.ReadXml(childNode, (FileGroup) null, (ICollection<string>) stringList);
      }
    }

    public override void Save()
    {
      if (!this.NeedToSave)
        return;
      using (StringWriter stringWriter = new StringWriter())
      {
        using (XmlTextWriter tw = new XmlTextWriter((TextWriter) stringWriter))
        {
          DefaultSettingsGroup.Instance.WriteXml(tw);
          tw.Flush();
          tw.Close();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringWriter.ToString());
          this.Settings.DefaultGroupSettings.Value = xmlDocument.LastChild;
        }
      }
      using (StringWriter stringWriter = new StringWriter())
      {
        using (XmlTextWriter tw = new XmlTextWriter((TextWriter) stringWriter))
        {
          Program.CredentialsProfiles.WriteXml(tw, (RdcTreeNode) DefaultSettingsGroup.Instance);
          tw.Flush();
          tw.Close();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringWriter.ToString());
          this.Settings.CredentialsProfiles.Value = xmlDocument.LastChild;
        }
      }
      this.CollectFilesToOpen();
      this.SerializeBuiltInGroups();
      this.SerializePluginSettings();
      string temporaryFileName = Helpers.GetTemporaryFileName(this.SettingsPath, ".new");
      using (XmlTextWriter tw = new XmlTextWriter(temporaryFileName, Encoding.UTF8))
      {
        tw.Formatting = Formatting.Indented;
        tw.WriteStartDocument();
        this.Settings.WriteXml(tw, (RdcTreeNode) null);
        tw.WriteEndDocument();
        tw.Close();
        Helpers.MoveTemporaryToPermanent(temporaryFileName, this.SettingsPath, false);
      }
      this.NeedToSave = false;
    }

    private void SerializeBuiltInGroups()
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        using (XmlTextWriter tw = new XmlTextWriter((TextWriter) stringWriter))
        {
          tw.WriteStartElement("groups");
          Program.BuiltInVirtualGroups.ForEach<IBuiltInVirtualGroup>((Action<IBuiltInVirtualGroup>) (virtualGroup =>
          {
            if (string.IsNullOrEmpty(virtualGroup.XmlNodeName))
              return;
            virtualGroup.WriteXml(tw, (FileGroup) null);
          }));
          tw.WriteEndElement();
          tw.Flush();
          tw.Close();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringWriter.ToString());
          this.Settings.BuiltInGroups.Value = xmlDocument.LastChild;
        }
      }
    }

    private void CollectFilesToOpen() => this.FilesToOpen = ServerTree.Instance.Nodes.OfType<FileGroup>().Select<FileGroup, string>((Func<FileGroup, string>) (file => file.Pathname)).ToList<string>();

    private void SerializePluginSettings()
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        using (XmlTextWriter tw = new XmlTextWriter((TextWriter) stringWriter))
        {
          tw.WriteStartElement("plugins");
          Program.PluginAction((Action<IPlugin>) (p =>
          {
            try
            {
              XmlNode xmlNode = p.SaveSettings();
              if (xmlNode == null)
                return;
              tw.WriteStartElement("plugin");
              tw.WriteAttributeString("path", p.GetType().AssemblyQualifiedName);
              xmlNode.WriteTo((XmlWriter) tw);
              tw.WriteEndElement();
            }
            catch
            {
            }
          }));
          tw.WriteEndElement();
          tw.Flush();
          tw.Close();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(stringWriter.ToString());
          this.Settings.PluginSettings.Value = xmlDocument.LastChild;
        }
      }
    }

    internal bool GetBuiltInGroupVisibility(IBuiltInVirtualGroup builtInGroup) => (bool) this[string.Format("Show{0}Group", (object) builtInGroup.ConfigPropertyName)];

    internal void SetBuiltInGroupVisibility(IBuiltInVirtualGroup builtInGroup, bool value) => this[string.Format("Show{0}Group", (object) builtInGroup.ConfigPropertyName)] = (object) value;

    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool AutoSaveFiles
    {
      get => (bool) this[nameof (AutoSaveFiles)];
      set => this[nameof (AutoSaveFiles)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public int AutoSaveInterval
    {
      get => (int) this[nameof (AutoSaveInterval)];
      set => this[nameof (AutoSaveInterval)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool ShowConnectedGroup
    {
      get => (bool) this[nameof (ShowConnectedGroup)];
      set => this[nameof (ShowConnectedGroup)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool ShowFavoritesGroup
    {
      get => (bool) this[nameof (ShowFavoritesGroup)];
      set => this[nameof (ShowFavoritesGroup)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool ShowReconnectGroup
    {
      get => (bool) this[nameof (ShowReconnectGroup)];
      set => this[nameof (ShowReconnectGroup)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool ShowRecentlyUsedGroup
    {
      get => (bool) this[nameof (ShowRecentlyUsedGroup)];
      set => this[nameof (ShowRecentlyUsedGroup)] = (object) value;
    }

    [UserScopedSetting]
    public List<string> FilesToOpen
    {
      get => (List<string>) this[nameof (FilesToOpen)];
      set => this[nameof (FilesToOpen)] = (object) value;
    }

    [UserScopedSetting]
    public byte[] CredentialsProfiles
    {
      get => (byte[]) base[nameof (CredentialsProfiles)];
      set => base[nameof (CredentialsProfiles)] = (object) value;
    }

    [UserScopedSetting]
    public XmlDocument CredentialsProfilesXml
    {
      get => (XmlDocument) this[nameof (CredentialsProfilesXml)];
      set => this[nameof (CredentialsProfilesXml)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool FocusOnClick
    {
      get => (bool) this[nameof (FocusOnClick)];
      set => this[nameof (FocusOnClick)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool DimNodesWhenInactive
    {
      get => (bool) this[nameof (DimNodesWhenInactive)];
      set => this[nameof (DimNodesWhenInactive)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool FullScreenWindowIsTopMost
    {
      get => (bool) this[nameof (FullScreenWindowIsTopMost)];
      set => this[nameof (FullScreenWindowIsTopMost)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool UseMultipleMonitors
    {
      get => (bool) this[nameof (UseMultipleMonitors)];
      set => this[nameof (UseMultipleMonitors)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool EnablePanning
    {
      get => (bool) this[nameof (EnablePanning)];
      set => this[nameof (EnablePanning)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("1")]
    public int PanningAcceleration
    {
      get => (int) this[nameof (PanningAcceleration)];
      set => this[nameof (PanningAcceleration)] = (object) value;
    }

    [DefaultSettingValue("ByName")]
    [UserScopedSetting]
    public SortOrder GroupSortOrder
    {
      get => (SortOrder) this[nameof (GroupSortOrder)];
      set => this[nameof (GroupSortOrder)] = (object) value;
    }

    [DefaultSettingValue("ByStatus")]
    [UserScopedSetting]
    public SortOrder ServerSortOrder
    {
      get => (SortOrder) this[nameof (ServerSortOrder)];
      set => this[nameof (ServerSortOrder)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("Dock")]
    public ControlVisibility ServerTreeVisibility
    {
      get => (ControlVisibility) this[nameof (ServerTreeVisibility)];
      set => this[nameof (ServerTreeVisibility)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public int ServerTreeAutoHidePopUpDelay
    {
      get => (int) this[nameof (ServerTreeAutoHidePopUpDelay)];
      set => this[nameof (ServerTreeAutoHidePopUpDelay)] = (object) value;
    }

    [DefaultSettingValue("Left")]
    [UserScopedSetting]
    public DockStyle ServerTreeLocation
    {
      get => (DockStyle) this[nameof (ServerTreeLocation)];
      set => this[nameof (ServerTreeLocation)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    public bool ThumbnailSizeIsInPixels
    {
      get => (bool) this[nameof (ThumbnailSizeIsInPixels)];
      set => this[nameof (ThumbnailSizeIsInPixels)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("160, 120")]
    public Size ThumbnailSize
    {
      get => (Size) this[nameof (ThumbnailSize)];
      set => this[nameof (ThumbnailSize)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("15")]
    public int ThumbnailPercentage
    {
      get => (int) this[nameof (ThumbnailPercentage)];
      set => this[nameof (ThumbnailPercentage)] = (object) value;
    }

    [UserScopedSetting]
    public byte[] DefaultGroupSettings
    {
      get => (byte[]) base[nameof (DefaultGroupSettings)];
      set => base[nameof (DefaultGroupSettings)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public int PerformanceFlags
    {
      get => (int) this[nameof (PerformanceFlags)];
      set => this[nameof (PerformanceFlags)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool WindowIsMaximized
    {
      get => (bool) this[nameof (WindowIsMaximized)];
      set => this[nameof (WindowIsMaximized)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("200, 200")]
    public Point WindowPosition
    {
      get => (Point) this[nameof (WindowPosition)];
      set => this[nameof (WindowPosition)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("200")]
    public int ServerTreeWidth
    {
      get => (int) this[nameof (ServerTreeWidth)];
      set => this[nameof (ServerTreeWidth)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("Pinned")]
    public RdpClient.ConnectionBarState ConnectionBarState
    {
      get => (RdpClient.ConnectionBarState) this[nameof (ConnectionBarState)];
      set => this[nameof (ConnectionBarState)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("1273, 823")]
    public Size WindowSize
    {
      get => (Size) this[nameof (WindowSize)];
      set => this[nameof (WindowSize)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool LockWindowSize
    {
      get => (bool) this[nameof (LockWindowSize)];
      set => this[nameof (LockWindowSize)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("Insert")]
    public Keys HotKeyAltEsc
    {
      get => (Keys) this[nameof (HotKeyAltEsc)];
      set => this[nameof (HotKeyAltEsc)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("Home")]
    public Keys HotKeyCtrlEsc
    {
      get => (Keys) this[nameof (HotKeyCtrlEsc)];
      set => this[nameof (HotKeyCtrlEsc)] = (object) value;
    }

    [DefaultSettingValue("PageUp")]
    [UserScopedSetting]
    public Keys HotKeyAltTab
    {
      get => (Keys) this[nameof (HotKeyAltTab)];
      set => this[nameof (HotKeyAltTab)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("PageDown")]
    public Keys HotKeyAltShiftTab
    {
      get => (Keys) this[nameof (HotKeyAltShiftTab)];
      set => this[nameof (HotKeyAltShiftTab)] = (object) value;
    }

    [DefaultSettingValue("Delete")]
    [UserScopedSetting]
    public Keys HotKeyAltSpace
    {
      get => (Keys) this[nameof (HotKeyAltSpace)];
      set => this[nameof (HotKeyAltSpace)] = (object) value;
    }

    [DefaultSettingValue("End")]
    [UserScopedSetting]
    public Keys HotKeyCtrlAltDel
    {
      get => (Keys) this[nameof (HotKeyCtrlAltDel)];
      set => this[nameof (HotKeyCtrlAltDel)] = (object) value;
    }

    [DefaultSettingValue("Cancel")]
    [UserScopedSetting]
    public Keys HotKeyFullScreen
    {
      get => (Keys) this[nameof (HotKeyFullScreen)];
      set => this[nameof (HotKeyFullScreen)] = (object) value;
    }

    [DefaultSettingValue("Left")]
    [UserScopedSetting]
    public Keys HotKeyFocusReleaseLeft
    {
      get => (Keys) this[nameof (HotKeyFocusReleaseLeft)];
      set => this[nameof (HotKeyFocusReleaseLeft)] = (object) value;
    }

    [DefaultSettingValue("Right")]
    [UserScopedSetting]
    public Keys HotKeyFocusReleaseRight
    {
      get => (Keys) this[nameof (HotKeyFocusReleaseRight)];
      set => this[nameof (HotKeyFocusReleaseRight)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("2012-06-01 00:00:00Z")]
    public string LastUpdateCheckTimeUtc
    {
      get => (string) this[nameof (LastUpdateCheckTimeUtc)];
      set => this[nameof (LastUpdateCheckTimeUtc)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool HideMainMenu
    {
      get => (bool) this[nameof (HideMainMenu)];
      set => this[nameof (HideMainMenu)] = (object) value;
    }

    [DefaultSettingValue("True")]
    [UserScopedSetting]
    public bool ReconnectOnStartup
    {
      get => (bool) this[nameof (ReconnectOnStartup)];
      set => this[nameof (ReconnectOnStartup)] = (object) value;
    }
  }
}
