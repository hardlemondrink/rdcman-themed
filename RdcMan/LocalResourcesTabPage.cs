// Decompiled with JetBrains decompiler
// Type: RdcMan.LocalResourcesTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using MSTSCLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public class LocalResourcesTabPage : SettingsTabPage<LocalResourcesSettings>
  {
    private bool _processingAfterCheck;
    private TreeNode _redirectDrivesCheckBox;
    private TreeNode _redirectPrintersCheckBox;
    private TreeNode _redirectPortsCheckBox;
    private TreeNode _redirectSmartCardsCheckBox;
    private TreeNode _redirectClipboardCheckBox;
    private TreeNode _redirectPnpDevicesCheckBox;

    public LocalResourcesTabPage(TabbedSettingsDialog dialog, LocalResourcesSettings settings)
      : base(dialog, settings)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
      FormTools.AddLabeledEnumDropDown<RdpClient.AudioRedirectionMode>((Control) this, "Remote &sound", this.Settings.AudioRedirectionMode, ref rowIndex, ref tabIndex, new Func<RdpClient.AudioRedirectionMode, string>(RdpClient.AudioRedirectionModeToString));
      if (RdpClient.SupportsAdvancedAudioVideoRedirection)
      {
        FormTools.AddLabeledEnumDropDown<RdpClient.AudioRedirectionQuality>((Control) this, "Sound &quality", this.Settings.AudioRedirectionQuality, ref rowIndex, ref tabIndex, new Func<RdpClient.AudioRedirectionQuality, string>(RdpClient.AudioRedirectionQualityToString));
        FormTools.AddLabeledEnumDropDown<RdpClient.AudioCaptureRedirectionMode>((Control) this, "Remote &recording", this.Settings.AudioCaptureRedirectionMode, ref rowIndex, ref tabIndex, new Func<RdpClient.AudioCaptureRedirectionMode, string>(RdpClient.AudioCaptureRedirectionModeToString));
      }
      FormTools.AddLabeledEnumDropDown<RdpClient.KeyboardHookMode>((Control) this, "&Windows key combos", this.Settings.KeyboardHookMode, ref rowIndex, ref tabIndex, new Func<RdpClient.KeyboardHookMode, string>(RdpClient.KeyboardHookModeToString));
      Label label = FormTools.NewLabel("Redirect options", 0, rowIndex);
      TreeView treeView1 = new TreeView();
      treeView1.Location = FormTools.NewLocation(1, rowIndex);
      treeView1.Size = new Size(340, 140);
      treeView1.CheckBoxes = true;
      treeView1.Scrollable = true;
      treeView1.ShowLines = false;
      TreeView treeView2 = treeView1;
      treeView2.AfterCheck += new TreeViewEventHandler(this.RedirectView_AfterCheck);
      this._redirectClipboardCheckBox = treeView2.Nodes.Add("Clipboard");
      this._redirectPrintersCheckBox = treeView2.Nodes.Add("Printers");
      this._redirectSmartCardsCheckBox = treeView2.Nodes.Add("Smart cards");
      this._redirectPortsCheckBox = treeView2.Nodes.Add("Ports");
      this._redirectDrivesCheckBox = treeView2.Nodes.Add("Drives");
      this._redirectPnpDevicesCheckBox = treeView2.Nodes.Add("PnP devices");
      if (RdpClient.SupportsFineGrainedRedirection)
      {
        IMsRdpDriveCollection driveCollection = RdpClient.DriveCollection;
        for (uint index = 0; index < driveCollection.DriveCount; ++index)
        {
          IMsRdpDrive msRdpDrive = driveCollection.get_DriveByIndex(index);
          this._redirectDrivesCheckBox.Nodes.Add(msRdpDrive.Name.Substring(0, msRdpDrive.Name.Length - 1));
        }
      }
      this.Controls.Add((Control) label);
      this.Controls.Add((Control) treeView2);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this._redirectDrivesCheckBox.Checked = this.Settings.RedirectDrives.Value;
      this._redirectPortsCheckBox.Checked = this.Settings.RedirectPorts.Value;
      this._redirectPrintersCheckBox.Checked = this.Settings.RedirectPrinters.Value;
      this._redirectSmartCardsCheckBox.Checked = this.Settings.RedirectSmartCards.Value;
      this._redirectClipboardCheckBox.Checked = this.Settings.RedirectClipboard.Value;
      this._redirectPnpDevicesCheckBox.Checked = this.Settings.RedirectPnpDevices.Value;
      foreach (string str in this.Settings.RedirectDrivesList.Value)
      {
        foreach (TreeNode node in this._redirectDrivesCheckBox.Nodes)
        {
          if (node.Text == str)
          {
            this._redirectDrivesCheckBox.Expand();
            node.Checked = true;
          }
        }
      }
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      this.Settings.RedirectDrives.Value = this._redirectDrivesCheckBox.Checked;
      List<string> stringList = new List<string>();
      foreach (TreeNode node in this._redirectDrivesCheckBox.Nodes)
      {
        if (node.Checked)
          stringList.Add(node.Text);
      }
      this.Settings.RedirectDrivesList.Value = stringList;
      this.Settings.RedirectPorts.Value = this._redirectPortsCheckBox.Checked;
      this.Settings.RedirectPrinters.Value = this._redirectPrintersCheckBox.Checked;
      this.Settings.RedirectSmartCards.Value = this._redirectSmartCardsCheckBox.Checked;
      this.Settings.RedirectClipboard.Value = this._redirectClipboardCheckBox.Checked;
      this.Settings.RedirectPnpDevices.Value = this._redirectPnpDevicesCheckBox.Checked;
    }

    private void RedirectView_AfterCheck(object sender, TreeViewEventArgs e)
    {
      if (this._processingAfterCheck)
        return;
      this._processingAfterCheck = true;
      if (e.Node.Nodes.Count == 0 && e.Node.Parent != null)
      {
        e.Node.Parent.Checked = e.Node.Parent.Nodes.Cast<TreeNode>().All<TreeNode>((Func<TreeNode, bool>) (node => node.Checked));
      }
      else
      {
        foreach (TreeNode node in e.Node.Nodes)
          node.Checked = e.Node.Checked;
      }
      this._processingAfterCheck = false;
    }
  }
}
