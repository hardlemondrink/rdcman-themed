// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectionSettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class ConnectionSettingsTabPage : SettingsTabPage<ConnectionSettings>
  {
    private Label _portDefaultLabel;
    private NumericUpDown _portUpDown;

    public ConnectionSettingsTabPage(TabbedSettingsDialog dialog, ConnectionSettings settings)
      : base(dialog, settings)
    {
      int tabIndex1 = 0;
      int rowIndex1 = 0;
      this.CreateInheritanceControl(ref rowIndex1, ref tabIndex1);
      FormTools.AddCheckBox((Control) this, "&Connect to console", settings.ConnectToConsole, 1, ref rowIndex1, ref tabIndex1);
      FormTools.AddLabeledTextBox((Control) this, "&Start program:", settings.StartProgram, ref rowIndex1, ref tabIndex1);
      FormTools.AddLabeledTextBox((Control) this, "&Working dir:", settings.WorkingDir, ref rowIndex1, ref tabIndex1);
      Label label = FormTools.NewLabel("Por&t:", 0, rowIndex1);
      this._portUpDown = new NumericUpDown();
      this._portUpDown.BeginInit();
      NumericUpDown portUpDown1 = this._portUpDown;
      int rowIndex2 = rowIndex1;
      int rowIndex3 = rowIndex2 + 1;
      Point point = FormTools.NewLocation(1, rowIndex2);
      portUpDown1.Location = point;
      this._portUpDown.Size = new Size(160, 20);
      this._portUpDown.Minimum = 1M;
      this._portUpDown.Maximum = 65535M;
      this._portUpDown.KeyUp += (KeyEventHandler) ((s, e) => this.UpdatePortDefaultLabel());
      this._portUpDown.ValueChanged += (EventHandler) ((s, e) => this.UpdatePortDefaultLabel());
      NumericUpDown portUpDown2 = this._portUpDown;
      int num = tabIndex1;
      int tabIndex2 = num + 1;
      portUpDown2.TabIndex = num;
      this._portUpDown.EndInit();
      this._portDefaultLabel = new Label();
      this._portDefaultLabel.Location = new Point(this._portUpDown.Location.X + this._portUpDown.Width, this._portUpDown.Location.Y - 1);
      this._portDefaultLabel.Size = new Size(140, 20);
      this._portDefaultLabel.TextAlign = ContentAlignment.MiddleLeft;
      FormTools.AddLabeledTextBox((Control) this, "&Load balance config:", settings.LoadBalanceInfo, ref rowIndex3, ref tabIndex2);
      this.Controls.Add((Control) label, (Control) this._portUpDown, (Control) this._portDefaultLabel);
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this._portUpDown.Text = this.Settings.Port.Value.ToString();
    }

    protected override void UpdateSettings()
    {
      base.UpdateSettings();
      this.Settings.Port.Value = (int) this._portUpDown.Value;
    }

    private void UpdatePortDefaultLabel() => this._portDefaultLabel.Text = this._portUpDown.Value == 3389M ? "(default)" : string.Empty;
  }
}
