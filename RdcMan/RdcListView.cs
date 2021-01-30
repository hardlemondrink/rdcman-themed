// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcListView
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  public class RdcListView : ListView
  {
    public event HeaderColumnClickEventHandler HeaderCheckBoxClick;

    public static bool SupportsHeaderCheckBoxes { get; private set; }

    static RdcListView() => RdcListView.SupportsHeaderCheckBoxes = Kernel.MajorVersion >= 6U;

    public unsafe void SetColumnHeaderToCheckBox(int index)
    {
      if (!RdcListView.SupportsHeaderCheckBoxes)
        throw new InvalidOperationException("Header check boxes are not supported on this operating system version");
      if (this.Parent == null)
        throw new InvalidOperationException("Control must have a parent before setting header style");
      if (index < 0 || index >= this.Columns.Count)
        throw new IndexOutOfRangeException("Column index out of range");
      if (!string.IsNullOrEmpty(this.Columns[index].Text))
        throw new InvalidOperationException("Column must have no text");
      IntPtr headerHandle = this.HeaderHandle;
      int windowLong = User.GetWindowLong(headerHandle, -16);
      User.SetWindowLong(headerHandle, -16, windowLong | 1024);
      Structs.HDITEM hditem;
      hditem.mask = 4U;
      Structs.HDITEM* hditemPtr = &hditem;
      User.SendMessage(headerHandle, 4619U, (IntPtr) index, (IntPtr) (void*) hditemPtr);
      hditem.fmt |= 320;
      User.SendMessage(headerHandle, 4620U, (IntPtr) index, (IntPtr) (void*) hditemPtr);
    }

    public unsafe void SetColumnHeaderChecked(int index, bool isChecked)
    {
      IntPtr headerHandle = this.HeaderHandle;
      Structs.HDITEM hditem;
      hditem.mask = 4U;
      Structs.HDITEM* hditemPtr = &hditem;
      User.SendMessage(headerHandle, 4619U, (IntPtr) index, (IntPtr) (void*) hditemPtr);
      if (isChecked)
        hditem.fmt |= 128;
      else
        hditem.fmt &= -129;
      User.SendMessage(headerHandle, 4620U, (IntPtr) index, (IntPtr) (void*) hditemPtr);
    }

    protected override unsafe void WndProc(ref Message m)
    {
      if (m.Msg == 78 && this.HeaderCheckBoxClick != null)
      {
        Structs.NMHEADER structure = (Structs.NMHEADER) Marshal.PtrToStructure(m.LParam, typeof (Structs.NMHEADER));
        if (structure.hdr.code == 4294966980U)
        {
          Structs.HDITEM hditem;
          hditem.mask = 4U;
          Structs.HDITEM* hditemPtr = &hditem;
          User.SendMessage(structure.hdr.hwndFrom, 4619U, (IntPtr) structure.iItem, (IntPtr) (void*) hditemPtr);
          hditem.fmt ^= 128;
          User.SendMessage(structure.hdr.hwndFrom, 4620U, (IntPtr) structure.iItem, (IntPtr) (void*) hditemPtr);
          bool isChecked = (hditem.fmt & 128) != 0;
          this.HeaderCheckBoxClick((object) this, new HeaderColumnClickEventArgs(structure.iItem, isChecked));
          return;
        }
      }
      base.WndProc(ref m);
    }

    private IntPtr HeaderHandle => User.SendMessage(this.Handle, 4127U, (IntPtr) 0, (IntPtr) 0);
  }
}
