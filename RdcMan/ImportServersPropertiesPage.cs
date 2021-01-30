// Decompiled with JetBrains decompiler
// Type: RdcMan.ImportServersPropertiesPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ImportServersPropertiesPage : NodePropertiesPage<SettingsGroup>
  {
    private RdcTextBox _fileNameTextBox;
    private RdcTextBox _serversTextBox;

    public ImportServersPropertiesPage(TabbedSettingsDialog dialog)
      : base(dialog, (SettingsGroup) null, "Server Settings")
    {
      int num1 = 0;
      int rowIndex1 = 0;
      Label label1 = new Label();
      label1.Location = FormTools.NewLocation(0, rowIndex1);
      label1.Size = new Size(480, 48);
      label1.Text = "Select a file with server info or enter info in the textbox below. Server names are delimited by commas and newlines. Expansions are permitted.";
      Label label2 = label1;
      int rowIndex2 = rowIndex1 + 2;
      this.Controls.Add((Control) label2);
      Button button1 = new Button();
      Button button2 = button1;
      int num2 = num1;
      int tabIndex1 = num2 + 1;
      button2.TabIndex = num2;
      button1.Text = "&Browse";
      Button browseButton = button1;
      browseButton.Click += new EventHandler(this.OnBrowseClick);
      this._fileNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&File name:", ref rowIndex2, ref tabIndex1);
      this._fileNameTextBox.Enabled = true;
      this._fileNameTextBox.Width -= browseButton.Width + 8;
      browseButton.Location = new Point(this._fileNameTextBox.Right + 8, this._fileNameTextBox.Top);
      int rowIndex3 = rowIndex2;
      int tabIndex2 = tabIndex1;
      int tabIndex3 = tabIndex2 + 1;
      this._serversTextBox = FormTools.NewTextBox(1, rowIndex3, tabIndex2, 7);
      this._serversTextBox.AcceptsReturn = true;
      this._serversTextBox.Enabled = true;
      this._serversTextBox.ScrollBars = ScrollBars.Vertical;
      int rowIndex4 = rowIndex2 + 6;
      this.Controls.Add((Control) browseButton, (Control) this._serversTextBox);
      this.AddParentCombo(ref rowIndex4, ref tabIndex3);
      this._fileNameTextBox.TextChanged += (EventHandler) ((s, e) => this._serversTextBox.Enabled = string.IsNullOrEmpty(this._fileNameTextBox.Text));
      this._serversTextBox.TextChanged += (EventHandler) ((s, e) => this._fileNameTextBox.Enabled = browseButton.Enabled = string.IsNullOrEmpty(this._serversTextBox.Text));
      this.FocusControl = (Control) this._fileNameTextBox;
    }

    public List<string> ExpandedServerNames { get; private set; }

    protected override bool IsValid()
    {
      Control c = (Control) this._serversTextBox;
      string str = this._serversTextBox.Text;
      this.Dialog.SetError((Control) this._serversTextBox, (string) null);
      this.Dialog.SetError((Control) this._fileNameTextBox, (string) null);
      if (!string.IsNullOrEmpty(this._fileNameTextBox.Text))
      {
        c = (Control) this._fileNameTextBox;
        try
        {
          str = File.ReadAllText(this._fileNameTextBox.Text);
        }
        catch (Exception ex)
        {
          this.Dialog.SetError((Control) this._fileNameTextBox, ex.Message);
          return false;
        }
      }
      if (string.IsNullOrWhiteSpace(str))
      {
        this.Dialog.SetError((Control) this._fileNameTextBox, "Please enter a file name");
        return false;
      }
      try
      {
        List<string> stringList = new List<string>();
        foreach (Match match in Regex.Matches(str.Replace(Environment.NewLine, ","), "([^,\\{\\s]*\\{[^\\}]*\\}[^,\\{,\\}\\s]*)|([^,\\{\\}\\s]+)"))
          stringList.AddRange(StringUtilities.ExpandPattern(match.Groups[0].Value.Trim()));
        this.ExpandedServerNames = stringList;
      }
      catch (Exception ex)
      {
        this.Dialog.SetError(c, ex.Message);
      }
      return true;
    }

    protected override bool CanBeParent(GroupBase group) => group.CanAddServers();

    private void OnBrowseClick(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = "Import";
        openFileDialog.DefaultExt = "txt";
        openFileDialog.AddExtension = true;
        openFileDialog.CheckFileExists = true;
        openFileDialog.RestoreDirectory = false;
        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() != DialogResult.OK)
          return;
        this._fileNameTextBox.Text = openFileDialog.FileName;
      }
    }
  }
}
