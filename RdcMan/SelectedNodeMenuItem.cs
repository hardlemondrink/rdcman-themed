// Decompiled with JetBrains decompiler
// Type: RdcMan.SelectedNodeMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  internal class SelectedNodeMenuItem : SelectedNodeMenuItem<RdcTreeNode>
  {
    public SelectedNodeMenuItem(string text, MenuNames name, Action<RdcTreeNode> action)
      : base(text, name, action)
    {
    }

    public SelectedNodeMenuItem(
      string text,
      MenuNames name,
      string shortcut,
      Action<RdcTreeNode> action)
      : base(text, name, shortcut, action)
    {
    }
  }
}
