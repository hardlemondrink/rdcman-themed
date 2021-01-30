// Decompiled with JetBrains decompiler
// Type: RdcMan.NumericTextBox
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class NumericTextBox : TextBox, ISettingControl
  {
    private readonly int _min;
    private readonly int _max;
    private readonly string _invalidMessage;

    public NumericTextBox(int min, int max, string invalidMessage)
    {
      if (min < 0)
        throw new ArgumentOutOfRangeException(nameof (min));
      if (max < 0)
        throw new ArgumentOutOfRangeException(nameof (max));
      if (min >= max)
        throw new ArgumentException("Minimum must be less than maximum");
      if (string.IsNullOrWhiteSpace(invalidMessage))
        throw new ArgumentOutOfRangeException(nameof (invalidMessage));
      this._min = min;
      this._max = max;
      this._invalidMessage = invalidMessage;
    }

    public IntSetting Setting { get; set; }

    void ISettingControl.UpdateControl()
    {
      if (this.Setting == null)
        return;
      this.Text = this.Setting.Value.ToString();
    }

    void ISettingControl.UpdateSetting()
    {
      if (this.Setting == null)
        return;
      this.Setting.Value = int.Parse(this.Text);
    }

    string ISettingControl.Validate()
    {
      string str = (string) null;
      int num = int.Parse(this.Text);
      if (num < this._min || num > this._max)
        str = this._invalidMessage;
      return str;
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
      if ((keyData & Keys.Modifiers) == Keys.None)
      {
        switch (keyData)
        {
          case Keys.D0:
          case Keys.D1:
          case Keys.D2:
          case Keys.D3:
          case Keys.D4:
          case Keys.D5:
          case Keys.D6:
          case Keys.D7:
          case Keys.D8:
          case Keys.D9:
          case Keys.NumPad0:
          case Keys.NumPad1:
          case Keys.NumPad2:
          case Keys.NumPad3:
          case Keys.NumPad4:
          case Keys.NumPad5:
          case Keys.NumPad6:
          case Keys.NumPad7:
          case Keys.NumPad8:
          case Keys.NumPad9:
            return base.ProcessCmdKey(ref m, keyData);
        }
      }
      if ((keyData & (Keys.Control | Keys.Alt)) != Keys.None)
        return base.ProcessCmdKey(ref m, keyData);
      switch (keyData & Keys.KeyCode)
      {
        case Keys.Back:
        case Keys.Tab:
        case Keys.Return:
        case Keys.Escape:
        case Keys.End:
        case Keys.Home:
        case Keys.Left:
        case Keys.Right:
        case Keys.Delete:
          return base.ProcessCmdKey(ref m, keyData);
        default:
          return true;
      }
    }
  }
}
