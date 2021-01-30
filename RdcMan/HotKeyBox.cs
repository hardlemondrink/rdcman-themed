// Decompiled with JetBrains decompiler
// Type: RdcMan.HotKeyBox
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class HotKeyBox : TextBox, ISettingControl
  {
    private Keys _hotKey;

    public string Prefix { get; set; }

    public EnumSetting<Keys> Setting { get; set; }

    public Keys HotKey
    {
      get => this._hotKey;
      set
      {
        this._hotKey = value;
        this.Text = (this.Prefix + (this.HotKey != Keys.Next ? (this.HotKey != Keys.Cancel ? (this.HotKey < Keys.D0 || this.HotKey > Keys.D9 ? (this.HotKey < Keys.NumPad0 || this.HotKey > Keys.NumPad9 ? this.HotKey.ToString() : ((int) (this.HotKey - 96)).ToString() + " (num pad)") : ((int) (this.HotKey - 48)).ToString()) : "Break") : "PageDown")).ToUpper();
      }
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
      Keys keys = keyData & Keys.KeyCode;
      switch (keys)
      {
        case Keys.Tab:
        case Keys.Escape:
          return base.ProcessCmdKey(ref m, keyData);
        default:
          if ((keyData & Keys.Modifiers) == Keys.None || keys == Keys.Cancel)
            this.HotKey = keys;
          return true;
      }
    }

    void ISettingControl.UpdateControl()
    {
      if (this.Setting == null)
        return;
      this.HotKey = this.Setting.Value;
    }

    void ISettingControl.UpdateSetting()
    {
      if (this.Setting == null)
        return;
      this.Setting.Value = this.HotKey;
    }

    string ISettingControl.Validate() => (string) null;
  }
}
