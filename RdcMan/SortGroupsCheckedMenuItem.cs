// Decompiled with JetBrains decompiler
// Type: RdcMan.SortGroupsCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class SortGroupsCheckedMenuItem : EnumMenuItem<SortOrder>
  {
    public SortGroupsCheckedMenuItem(string text, SortOrder value)
      : base(text, value)
    {
    }

    protected override SortOrder Value
    {
      get => Program.Preferences.GroupSortOrder;
      set
      {
        Program.Preferences.GroupSortOrder = value;
        ServerTree.Instance.SortAllNodes();
        ServerTree.Instance.OnGroupChanged(ServerTree.Instance.RootNode, ChangeType.PropertyChanged);
      }
    }
  }
}
