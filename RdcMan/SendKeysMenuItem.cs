// Decompiled with JetBrains decompiler
// Type: RdcMan.SendKeysMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Text;
using System.Windows.Forms;

namespace RdcMan
{
  internal class SendKeysMenuItem : ToolStripMenuItem
  {
    public Keys[] KeyCodes;

    public SendKeysMenuItem(string name, Keys[] keyCodes)
    {
      this.KeyCodes = keyCodes;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Keys keyCode in keyCodes)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append("+");
        switch (keyCode)
        {
          case Keys.ShiftKey:
            stringBuilder.Append("Shift");
            break;
          case Keys.ControlKey:
            stringBuilder.Append("Control");
            break;
          case Keys.Menu:
            stringBuilder.Append("Alt");
            break;
          default:
            stringBuilder.Append(keyCode.ToString());
            break;
        }
      }
      if (name != null)
        this.Text = name + " (" + stringBuilder.ToString() + ")";
      else
        this.Text = stringBuilder.ToString();
    }
  }
}
