// Decompiled with JetBrains decompiler
// Type: RdcMan.HeaderColumnClickEventArgs
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public class HeaderColumnClickEventArgs : EventArgs
  {
    public int Column { get; private set; }

    public bool IsChecked { get; private set; }

    public HeaderColumnClickEventArgs(int column, bool isChecked)
    {
      this.Column = column;
      this.IsChecked = isChecked;
    }
  }
}
