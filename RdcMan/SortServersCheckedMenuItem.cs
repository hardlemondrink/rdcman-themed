// Decompiled with JetBrains decompiler
// Type: RdcMan.SortServersCheckedMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class SortServersCheckedMenuItem : EnumMenuItem<SortOrder>
  {
    public SortServersCheckedMenuItem(string text, SortOrder value)
      : base(text, value)
    {
    }

    protected override SortOrder Value
    {
      get => Program.Preferences.ServerSortOrder;
      set
      {
        Program.Preferences.ServerSortOrder = value;
        ServerTree.Instance.SortAllNodes();
        ServerTree.Instance.OnGroupChanged(ServerTree.Instance.RootNode, ChangeType.PropertyChanged);
      }
    }
  }
}
