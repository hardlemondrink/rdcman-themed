// Decompiled with JetBrains decompiler
// Type: RdcMan.RemoteDesktopTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public class RemoteDesktopTabPage : SettingsTabPage<RemoteDesktopSettings>
  {
    private readonly GroupBox _rdsSizeGroup;
    private readonly RadioButton _rdsCustomRadio;
    private readonly Button _rdsCustomButton;

    public RemoteDesktopTabPage(TabbedSettingsDialog dialog, RemoteDesktopSettings settings)
      : base(dialog, settings)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
      ValueComboBox<int> valueComboBox = FormTools.AddLabeledValueDropDown<int>((Control) this, "Color Depth", (Setting<int>) settings.ColorDepth, ref rowIndex, ref tabIndex, (Func<int, string>) (v => v.ToString()), (IEnumerable<int>) new int[5]
      {
        8,
        15,
        16,
        24,
        32
      });
      RdcRadioButton rdcRadioButton1 = new RdcRadioButton();
      rdcRadioButton1.Setting = settings.DesktopSizeSameAsClientAreaSize;
      rdcRadioButton1.Size = new Size(140, 24);
      rdcRadioButton1.Text = "&Same as client area";
      RadioButton radioButton1 = (RadioButton) rdcRadioButton1;
      RdcRadioButton rdcRadioButton2 = new RdcRadioButton();
      rdcRadioButton2.Setting = settings.DesktopSizeFullScreen;
      rdcRadioButton2.Size = new Size(140, 24);
      rdcRadioButton2.Text = "&Full screen";
      RadioButton radioButton2 = (RadioButton) rdcRadioButton2;
      this._rdsCustomRadio = new RadioButton();
      this._rdsCustomButton = new Button();
      this._rdsCustomRadio.Size = new Size(72, 24);
      this._rdsCustomRadio.Text = "&Custom";
      this._rdsSizeGroup = new GroupBox();
      this._rdsSizeGroup.Controls.AddRange(FormTools.NewSizeRadios());
      this._rdsSizeGroup.Controls.Add((Control) radioButton1);
      this._rdsSizeGroup.Controls.Add((Control) radioButton2);
      this._rdsSizeGroup.Controls.Add((Control) this._rdsCustomRadio);
      this._rdsSizeGroup.Text = "Remote Desktop Size";
      FormTools.LayoutGroupBox(this._rdsSizeGroup, 2, (Control) valueComboBox);
      this._rdsCustomButton.Location = new Point(this._rdsCustomRadio.Right + 10, this._rdsCustomRadio.Location.Y);
      this._rdsCustomButton.TabIndex = this._rdsCustomRadio.TabIndex + 1;
      this._rdsCustomButton.Click += new EventHandler(this.CustomSizeClick);
      this._rdsCustomButton.Text = Program.TheForm.GetClientSize().ToFormattedString();
      this._rdsSizeGroup.Controls.Add((Control) this._rdsCustomButton);
      this.Controls.Add((Control) this._rdsSizeGroup);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      Size size = this.Settings.DesktopSize.Value;
      if (!this.Settings.DesktopSizeSameAsClientAreaSize.Value && !this.Settings.DesktopSizeFullScreen.Value)
      {
        RadioButton radioButton = this._rdsSizeGroup.Controls.OfType<RadioButton>().Where<RadioButton>((Func<RadioButton, bool>) (r =>
        {
          Size? tag = (Size?) r.Tag;
          Size size1 = size;
          return tag.HasValue && tag.GetValueOrDefault() == size1;
        })).FirstOrDefault<RadioButton>();
        if (radioButton != null)
          radioButton.Checked = true;
        else
          this._rdsCustomRadio.Checked = true;
      }
      this._rdsCustomButton.Text = size.ToFormattedString();
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      if (this.Settings.DesktopSizeSameAsClientAreaSize.Value || this.Settings.DesktopSizeFullScreen.Value)
        return;
      string text = this._rdsCustomButton.Text;
      if (!this._rdsCustomRadio.Checked)
        text = this._rdsSizeGroup.Controls.OfType<RadioButton>().Where<RadioButton>((Func<RadioButton, bool>) (r => r.Checked)).First<RadioButton>().Text;
      this.Settings.DesktopSize.Value = SizeHelper.Parse(text);
    }

    private void CustomSizeClick(object sender, EventArgs e)
    {
      Button button = sender as Button;
      (button.Parent.GetNextControl((Control) button, false) as RadioButton).Checked = true;
      using (CustomSizeDialog customSizeDialog = new CustomSizeDialog(SizeHelper.Parse(button.Text)))
      {
        if (customSizeDialog.ShowDialog() != DialogResult.OK)
          return;
        button.Text = customSizeDialog.WidthText + SizeHelper.Separator + customSizeDialog.HeightText;
      }
    }
  }
}
