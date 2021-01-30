// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupChangedEventArgs
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public class GroupChangedEventArgs : EventArgs
  {
    public GroupChangedEventArgs(GroupBase group, ChangeType changeType)
    {
      this.Group = group;
      this.ChangeType = changeType;
    }

    public GroupBase Group { get; private set; }

    public ChangeType ChangeType { get; private set; }
  }
}
