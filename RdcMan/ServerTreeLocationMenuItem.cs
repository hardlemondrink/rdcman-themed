// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerTreeLocationMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerTreeLocationMenuItem : EnumMenuItem<DockStyle>
  {
    public ServerTreeLocationMenuItem(string text, DockStyle value)
      : base(text, value)
    {
    }

    protected override DockStyle Value
    {
      get => Program.TheForm.ServerTreeLocation;
      set => Program.TheForm.ServerTreeLocation = value;
    }
  }
}
