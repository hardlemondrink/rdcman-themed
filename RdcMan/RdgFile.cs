// Decompiled with JetBrains decompiler
// Type: RdcMan.RdgFile
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  internal static class RdgFile
  {
    private const int CurrentSchemaVersion = 3;
    private const string SaveFileFilter = "RDCMan Groups (*.rdg)|*.rdg";
    private const string OpenFileFilter = "RDCMan Groups (*.rdg)|*.rdg";
    private static int _saveInProgress;
    private static string CurrentWorkingDirectory;

    public static FileGroup NewFile()
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = "New File";
        saveFileDialog.Filter = "RDCMan Groups (*.rdg)|*.rdg";
        saveFileDialog.AddExtension = true;
        saveFileDialog.CheckPathExists = true;
        saveFileDialog.InitialDirectory = RdgFile.GetWorkingDirectory();
        saveFileDialog.RestoreDirectory = false;
        if (saveFileDialog.ShowDialog() != DialogResult.OK)
          return (FileGroup) null;
        FileGroup file = new FileGroup(saveFileDialog.FileName);
        ServerTree.Instance.AddNode((RdcTreeNode) file, ServerTree.Instance.RootNode);
        int num = (int) RdgFile.DoSaveWithRetry(file);
        return file;
      }
    }

    public static void CloseFileGroup(FileGroup file)
    {
      bool anyConnected;
      file.AnyOrAllConnected(out anyConnected, out bool _);
      if (anyConnected && FormTools.YesNoDialog("There are active connections from " + file.Text + ". Are you sure you want to close it?") == DialogResult.No || RdgFile.SaveFileGroup(file) == SaveResult.Cancel)
        return;
      ServerTree.Instance.RemoveNode((RdcTreeNode) file);
      Program.Preferences.NeedToSave = true;
    }

    public static FileGroup OpenFile()
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = "Open";
        openFileDialog.DefaultExt = "rdg";
        openFileDialog.AddExtension = true;
        openFileDialog.CheckFileExists = true;
        openFileDialog.InitialDirectory = RdgFile.GetWorkingDirectory();
        openFileDialog.RestoreDirectory = false;
        openFileDialog.Filter = "RDCMan Groups (*.rdg)|*.rdg";
        if (openFileDialog.ShowDialog() != DialogResult.OK)
          return (FileGroup) null;
        RdgFile.CurrentWorkingDirectory = Path.GetDirectoryName(openFileDialog.FileName);
        return RdgFile.OpenFile(openFileDialog.FileName);
      }
    }

    public static FileGroup OpenFile(string filename)
    {
      using (Helpers.Timer("reading {0}", (object) filename))
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlTextReader xmlTextReader = (XmlTextReader) null;
        XmlNode topNode;
        try
        {
          xmlTextReader = new XmlTextReader(filename);
          xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
          int content = (int) xmlTextReader.MoveToContent();
          topNode = xmlDocument.ReadNode((XmlReader) xmlTextReader);
        }
        catch (Exception ex)
        {
          FormTools.ErrorDialog(ex.Message);
          return (FileGroup) null;
        }
        finally
        {
          xmlTextReader?.Close();
        }
        if (topNode == null)
          throw new FileLoadException(filename + ": File format error");
        FileGroup fileGroup = new FileGroup(filename);
        FileGroup fileGroup1 = ServerTree.Instance.Nodes.OfType<FileGroup>().Where<FileGroup>((Func<FileGroup, bool>) (f => f.Pathname.Equals(fileGroup.Pathname, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<FileGroup>();
        if (fileGroup1 != null)
        {
          FormTools.InformationDialog("{0} is already open as '{1}'".CultureFormat((object) fileGroup.Pathname, (object) fileGroup1.Text));
          return fileGroup1;
        }
        try
        {
          List<string> errors = new List<string>();
          ServerTree.Instance.Operation(OperationBehavior.RestoreSelected | OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
          {
            ServerTree.Instance.AddNode((RdcTreeNode) fileGroup, ServerTree.Instance.RootNode);
            if (!RdgFile.ReadXml(topNode, fileGroup, (ICollection<string>) errors))
              throw new Exception(string.Empty);
          }));
          if (errors.Count > 0)
          {
            StringBuilder stringBuilder = new StringBuilder("The following errors were encountered:").AppendLine().AppendLine();
            foreach (string str in errors)
              stringBuilder.AppendLine(str);
            stringBuilder.AppendLine().Append("The file was not loaded completely. If it is saved it almost certainly means losing information. Continue?");
            if (FormTools.ExclamationDialog(stringBuilder.ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
              throw new Exception(string.Empty);
          }
          using (Helpers.Timer("sorting root, builtin groups and file"))
          {
            ServerTree.Instance.SortRoot();
            foreach (GroupBase builtInVirtualGroup in Program.BuiltInVirtualGroups)
            {
              ServerTree.Instance.SortGroup(builtInVirtualGroup);
              ServerTree.Instance.OnGroupChanged(builtInVirtualGroup, ChangeType.TreeChanged);
            }
            ServerTree.Instance.SortGroup((GroupBase) fileGroup, true);
            ServerTree.Instance.OnGroupChanged((GroupBase) fileGroup, ChangeType.TreeChanged);
          }
          SmartGroup.RefreshAll(fileGroup);
          fileGroup.VisitNodes((Action<RdcTreeNode>) (node =>
          {
            if (!(node is GroupBase groupBase) || !groupBase.Properties.Expanded.Value)
              return;
            groupBase.Expand();
          }));
          Encryption.DecryptPasswords();
          fileGroup.CheckCredentials();
          fileGroup.VisitNodes((Action<RdcTreeNode>) (n => n.ResetInheritance()));
          fileGroup.HasChangedSinceWrite = false;
          Program.Preferences.NeedToSave = true;
          return fileGroup;
        }
        catch (Exception ex)
        {
          if (!string.IsNullOrEmpty(ex.Message))
            FormTools.ErrorDialog(ex.Message);
          ServerTree.Instance.RemoveNode((RdcTreeNode) fileGroup);
          return (FileGroup) null;
        }
      }
    }

    private static bool ReadXml(XmlNode topNode, FileGroup fileGroup, ICollection<string> errors)
    {
      string str = "unknown";
      int num = 0;
      try
      {
        str = topNode.Attributes.GetNamedItem("programVersion").InnerText;
      }
      catch
      {
      }
      try
      {
        num = int.Parse(topNode.Attributes.GetNamedItem("schemaVersion").InnerText);
      }
      catch
      {
      }
      fileGroup.SchemaVersion = num;
      if (num > 3)
      {
        if (FormTools.YesNoDialog("{0} was written by a newer version of RDCMan ({1}). It may not load properly. If it does and is saved by this version, it will revert to the older file schema possibly losing information. Continue?".CultureFormat((object) fileGroup.GetFilename(), (object) str)) == DialogResult.No)
          return false;
      }
      GroupBase.SchemaVersion = num;
      if (GroupBase.SchemaVersion <= 2)
        fileGroup.EncryptionSettings.InheritSettingsType.Mode = InheritanceMode.None;
      Dictionary<string, Helpers.ReadXmlDelegate> nodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>();
      nodeActions["file"] = (Helpers.ReadXmlDelegate) ((childNode, group, errors2) => (group as GroupBase).ReadXml(childNode, errors2));
      foreach (IBuiltInVirtualGroup builtInVirtualGroup in Program.BuiltInVirtualGroups.Where<IBuiltInVirtualGroup>((Func<IBuiltInVirtualGroup, bool>) (v => !string.IsNullOrEmpty(v.XmlNodeName))))
      {
        IBuiltInVirtualGroup virtualGroup = builtInVirtualGroup;
        nodeActions[virtualGroup.XmlNodeName] = (Helpers.ReadXmlDelegate) ((childNode, group, errors2) => virtualGroup.ReadXml(childNode, fileGroup, errors2));
      }
      nodeActions["version"] = (Helpers.ReadXmlDelegate) ((childNode, group, errors2) => {});
      LongRunningActionForm.PerformOperation("Opening " + fileGroup.Pathname, false, (Action) (() =>
      {
        foreach (XmlNode childNode in topNode.ChildNodes)
        {
          Helpers.ReadXmlDelegate readXmlDelegate;
          if (nodeActions.TryGetValue(childNode.Name, out readXmlDelegate))
            readXmlDelegate(childNode, (RdcTreeNode) fileGroup, errors);
          else
            errors.Add("Unexpected Xml node {0} in '{1}'".CultureFormat((object) childNode.GetFullPath(), (object) fileGroup.GetFilename()));
        }
      }));
      return true;
    }

    public static SaveResult SaveFileGroup(FileGroup file)
    {
      if (Interlocked.CompareExchange(ref RdgFile._saveInProgress, 1, 0) == 1)
        return SaveResult.NoSave;
      try
      {
        return RdgFile.DoSaveWithRetry(file);
      }
      finally
      {
        RdgFile._saveInProgress = 0;
      }
    }

    public static SaveResult SaveAll()
    {
      if (Interlocked.CompareExchange(ref RdgFile._saveInProgress, 1, 0) == 1)
        return SaveResult.NoSave;
      try
      {
        return RdgFile.DoSaveAll(false);
      }
      finally
      {
        RdgFile._saveInProgress = 0;
      }
    }

    private static SaveResult DoSaveAll(bool conditional)
    {
      foreach (FileGroup file in ServerTree.Instance.Nodes.OfType<FileGroup>())
      {
        if (!conditional || file.HasChangedSinceWrite)
        {
          SaveResult saveResult = RdgFile.DoSaveWithRetry(file);
          if (saveResult == SaveResult.Cancel)
            return saveResult;
        }
      }
      return SaveResult.Save;
    }

    public static SaveResult DoSaveWithRetry(FileGroup file)
    {
      if (!file.AllowEdit(false))
        return SaveResult.NoSave;
      SaveResult saveResult;
      do
      {
        saveResult = RdgFile.SaveFile(file);
        if (saveResult == SaveResult.Cancel)
          return saveResult;
      }
      while (saveResult == SaveResult.Retry);
      return SaveResult.Save;
    }

    public static SaveResult SaveAs(FileGroup file)
    {
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Title = "Save";
        saveFileDialog.Filter = "RDCMan Groups (*.rdg)|*.rdg";
        saveFileDialog.AddExtension = true;
        saveFileDialog.CheckPathExists = true;
        saveFileDialog.FileName = Path.GetFileName(file.Pathname);
        saveFileDialog.InitialDirectory = Path.GetDirectoryName(file.Pathname);
        saveFileDialog.RestoreDirectory = false;
        SaveResult saveResult;
        do
        {
          switch (saveFileDialog.ShowDialog())
          {
            case DialogResult.OK:
              file.Pathname = Path.Combine(Directory.GetCurrentDirectory(), saveFileDialog.FileName);
              saveResult = RdgFile.SaveFile(file);
              continue;
            case DialogResult.Cancel:
              return SaveResult.Cancel;
            default:
              return SaveResult.NoSave;
          }
        }
        while (saveResult == SaveResult.Retry);
        return saveResult;
      }
    }

    private static SaveResult SaveFile(FileGroup fileGroup)
    {
      string temporaryFileName = Helpers.GetTemporaryFileName(fileGroup.Pathname, ".new");
      XmlTextWriter tw = (XmlTextWriter) null;
      try
      {
        tw = new XmlTextWriter(temporaryFileName, Encoding.UTF8);
        tw.Formatting = Formatting.Indented;
        tw.Indentation = 2;
        tw.WriteStartDocument();
        tw.WriteStartElement("RDCMan");
        tw.WriteAttributeString("programVersion", Program.TheForm.VersionText);
        tw.WriteAttributeString("schemaVersion", 3.ToString());
        fileGroup.WriteXml(tw);
        foreach (IBuiltInVirtualGroup builtInVirtualGroup in Program.BuiltInVirtualGroups.Where<IBuiltInVirtualGroup>((Func<IBuiltInVirtualGroup, bool>) (v => !string.IsNullOrEmpty(v.XmlNodeName))))
          builtInVirtualGroup.WriteXml(tw, fileGroup);
        tw.WriteEndElement();
        tw.WriteEndDocument();
        tw.Close();
        tw = (XmlTextWriter) null;
        Helpers.MoveTemporaryToPermanent(temporaryFileName, fileGroup.Pathname, fileGroup.SchemaVersion != 3);
        fileGroup.SchemaVersion = 3;
        fileGroup.HasChangedSinceWrite = false;
        return SaveResult.Save;
      }
      catch (Exception ex)
      {
        tw?.Close();
        switch (FormTools.YesNoCancelDialog(ex.Message + "\n\nTry again? (Selecting Cancel will preserve the original file)"))
        {
          case DialogResult.Cancel:
            return SaveResult.Cancel;
          case DialogResult.Yes:
            return SaveResult.Retry;
          default:
            return SaveResult.NoSave;
        }
      }
    }

    public static bool AutoSave()
    {
      if (Interlocked.CompareExchange(ref RdgFile._saveInProgress, 1, 0) == 1)
        return false;
      try
      {
        int num = (int) RdgFile.DoSaveAll(true);
      }
      finally
      {
        RdgFile._saveInProgress = 0;
      }
      return true;
    }

    public static SaveResult ShouldSaveFile(FileGroup file)
    {
      if (!file.AllowEdit(false))
        return SaveResult.NoSave;
      if (Program.Preferences.AutoSaveFiles || !file.HasChangedSinceWrite)
        return SaveResult.AutoSave;
      switch (FormTools.YesNoCancelDialog("Save changes to " + file.GetFilename() + "?", MessageBoxDefaultButton.Button1))
      {
        case DialogResult.Cancel:
          return SaveResult.Cancel;
        case DialogResult.No:
          return SaveResult.NoSave;
        default:
          return SaveResult.Save;
      }
    }

    private static string GetWorkingDirectory()
    {
      FileGroup selectedFile = ServerTree.Instance.GetSelectedFile();
      return selectedFile != null ? selectedFile.GetDirectory() : RdgFile.CurrentWorkingDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }
  }
}
