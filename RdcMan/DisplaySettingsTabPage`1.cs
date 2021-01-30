// Decompiled with JetBrains decompiler
// Type: RdcMan.DisplaySettingsTabPage`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class DisplaySettingsTabPage<TSettingsGroup> : SettingsTabPage<TSettingsGroup>
    where TSettingsGroup : CommonDisplaySettings
  {
    protected DisplaySettingsTabPage(TabbedSettingsDialog dialog, TSettingsGroup settings)
      : base(dialog, settings)
    {
    }

    protected void Create(out int rowIndex, out int tabIndex)
    {
      tabIndex = 0;
      rowIndex = 0;
      this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
      Label label = FormTools.NewLabel("&Thumbnail scale:", 0, rowIndex);
      label.Size = new Size(140, 20);
      RdcNumericUpDown rdcNumericUpDown = new RdcNumericUpDown();
      rdcNumericUpDown.Location = FormTools.NewLocation(1, rowIndex++);
      rdcNumericUpDown.Location = new Point(rdcNumericUpDown.Location.X, rdcNumericUpDown.Location.Y + 2);
      rdcNumericUpDown.Minimum = 1M;
      rdcNumericUpDown.Maximum = 9M;
      rdcNumericUpDown.Setting = this.Settings.ThumbnailScale;
      rdcNumericUpDown.Size = new Size(40, 20);
      rdcNumericUpDown.TabIndex = tabIndex++;
      RdcCheckBox rdcCheckBox1 = FormTools.NewCheckBox("Scale docked remote desktop to fit window", 0, rowIndex++, tabIndex++);
      rdcCheckBox1.Setting = this.Settings.SmartSizeDockedWindow;
      RdcCheckBox rdcCheckBox2 = FormTools.NewCheckBox("Scale undocked remote desktop to fit window", 0, rowIndex++, tabIndex++);
      rdcCheckBox2.Setting = this.Settings.SmartSizeUndockedWindow;
      this.Controls.Add((Control) label, (Control) rdcNumericUpDown, (Control) rdcCheckBox1, (Control) rdcCheckBox2);
    }
  }
}
