// Decompiled with JetBrains decompiler
// Type: RdcMan.InheritanceControl
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  public class InheritanceControl
  {
    private const string SourcePrefix = "Source: ";
    public CheckBox FromParentCheck;
    private Button _sourceButton;
    private Label _disabledLabel;
    protected readonly TabbedSettingsDialog _dialog;
    private RdcTreeNode _sourceNode;
    private readonly string _settingsGroupName;
    private bool _enabled;

    public event Action<bool> EnabledChanged;

    public InheritanceControl(TabbedSettingsDialog dialog, string settingsGroupName)
    {
      this._dialog = dialog;
      this._settingsGroupName = settingsGroupName;
      this._sourceNode = (RdcTreeNode) DefaultSettingsGroup.Instance;
      this._enabled = true;
    }

    public void Create(Control parent, ref int rowIndex, ref int tabIndex)
    {
      Label label = new Label();
      label.Enabled = true;
      label.Location = new Point(0, (parent.Height - 20) / 2);
      label.Size = new Size(parent.Width, 20);
      label.TextAlign = ContentAlignment.MiddleCenter;
      label.Visible = false;
      this._disabledLabel = label;
      this.FromParentCheck = (CheckBox) FormTools.NewCheckBox("In&herit from parent", 1, rowIndex++, tabIndex++);
      this.FromParentCheck.CheckedChanged += new EventHandler(this.CheckChangedHandler);
      Button button = new Button();
      button.Location = FormTools.NewLocation(1, rowIndex++);
      this._sourceButton = button;
      this._sourceButton.Size = new Size(340, this._sourceButton.Height);
      this._sourceButton.Click += new EventHandler(this.SourceButton_Click);
      this._sourceButton.TextChanged += new EventHandler(this.SourceButton_TextChanged);
      parent.Controls.Add((Control) this._disabledLabel);
      parent.Controls.Add((Control) this.FromParentCheck);
      parent.Controls.Add((Control) this._sourceButton);
    }

    private void SourceButton_Click(object sender, EventArgs e)
    {
      string activeTabName = this._sourceButton.Parent is TabPage parent ? parent.Text : string.Empty;
      this._sourceNode.DoPropertiesDialog(this._sourceButton.FindForm(), activeTabName);
    }

    private void SourceButton_TextChanged(object sender, EventArgs e)
    {
      string text = this._sourceButton.Text;
      Graphics graphics = this._sourceButton.CreateGraphics();
      bool flag = false;
      for (SizeF sizeF = graphics.MeasureString(text, this._sourceButton.Font); Math.Round((double) sizeF.Width, 1) > (double) this._sourceButton.Width; sizeF = graphics.MeasureString(text, this._sourceButton.Font))
      {
        int num = (int) Math.Round((Math.Round((double) sizeF.Width, 0) - (double) this._sourceButton.Width) / (double) this._sourceButton.Font.Size, 0) + 4;
        text = "Source: ..." + text.Substring(num + "Source: ".Length);
        flag = true;
      }
      if (!flag)
        return;
      this._sourceButton.Text = text;
    }

    public void UpdateControlsFromSettings(InheritSettingsType settings)
    {
      bool flag = settings.Mode == InheritanceMode.FromParent;
      if (flag != this.FromParentCheck.Checked)
        this.FromParentCheck.Checked = flag;
      else
        this.OnSettingChanged();
    }

    public void Enable(bool value, string reason)
    {
      this._enabled = value;
      this._disabledLabel.Text = "These settings are unavailable {0}".InvariantFormat((object) reason);
      foreach (Control control in (ArrangedElementCollection) this.FromParentCheck.Parent.Controls)
        control.Visible = this._enabled;
      this._disabledLabel.Enabled = !this._enabled;
      this._disabledLabel.Visible = !this._enabled;
      if (!this._enabled)
        return;
      this.OnSettingChanged();
    }

    private void CheckChangedHandler(object sender, EventArgs e) => this.OnSettingChanged();

    private void OnSettingChanged()
    {
      CheckBox fromParentCheck = this.FromParentCheck;
      this.EnableDisableControls(!fromParentCheck.Checked);
      if (fromParentCheck.Checked)
      {
        GroupBase groupBase = this._dialog.TabPages.OfType<INodePropertiesPage>().First<INodePropertiesPage>().ParentGroup;
        if (groupBase != this._sourceNode)
        {
          if (groupBase == null)
          {
            this._sourceNode = (RdcTreeNode) DefaultSettingsGroup.Instance;
          }
          else
          {
            while (true)
            {
              SettingsGroup settingsGroupByName = groupBase.GetSettingsGroupByName(this._settingsGroupName);
              if (settingsGroupByName.InheritSettingsType.Mode == InheritanceMode.FromParent)
                groupBase = settingsGroupByName.InheritSettingsType.GetInheritedSettingsNode((RdcTreeNode) groupBase);
              else
                break;
            }
            this._sourceNode = (RdcTreeNode) groupBase;
          }
        }
        if (this._sourceNode != DefaultSettingsGroup.Instance)
          this._sourceButton.Text = "Source: " + this._sourceNode.FullPath;
        else
          this._sourceButton.Text = "Source: Default settings group";
        this._sourceButton.Show();
      }
      else
        this._sourceButton.Hide();
    }

    public void EnableDisableControls(bool enable)
    {
      foreach (Control control in (ArrangedElementCollection) this.FromParentCheck.Parent.Controls)
      {
        if (control != this.FromParentCheck && control != this._sourceButton)
          control.Enabled = enable;
      }
      if (this.EnabledChanged == null)
        return;
      this.EnabledChanged(enable);
    }

    public InheritanceMode GetInheritanceMode() => this.FromParentCheck.Checked ? InheritanceMode.FromParent : InheritanceMode.None;
  }
}
