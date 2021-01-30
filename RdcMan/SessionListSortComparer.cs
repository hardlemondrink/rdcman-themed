// Decompiled with JetBrains decompiler
// Type: RdcMan.SessionListSortComparer
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections;
using System.Windows.Forms;

namespace RdcMan
{
  public class SessionListSortComparer : IComparer
  {
    private readonly int[] _sortOrder;

    public SessionListSortComparer(int[] sortOrder) => this._sortOrder = sortOrder;

    public int Compare(object obj1, object obj2)
    {
      ListViewItem listViewItem1 = obj1 as ListViewItem;
      ListViewItem listViewItem2 = obj2 as ListViewItem;
      foreach (int index in this._sortOrder)
      {
        int num = string.Compare(listViewItem1.SubItems[index].Text, listViewItem2.SubItems[index].Text);
        if (num != 0)
          return num;
      }
      return string.Compare(listViewItem1.SubItems[0].Text, listViewItem2.SubItems[0].Text);
    }
  }
}
