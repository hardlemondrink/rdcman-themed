// Decompiled with JetBrains decompiler
// Type: RdcMan.RdpClient8
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using AxMSTSCLib;
using System.Windows.Forms;

namespace RdcMan
{
  internal class RdpClient8 : AxMsRdpClient8NotSafeForScripting, IRdpClient
  {
    public RdpClient8(MainForm form)
    {
      this.BeginInit();
      this.Hide();
      form.AddToClientPanel((Control) this);
      this.EndInit();
    }
  }
}
