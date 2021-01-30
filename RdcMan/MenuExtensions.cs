// Decompiled with JetBrains decompiler
// Type: RdcMan.MenuExtensions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public static class MenuExtensions
  {
    public static ToolStripMenuItem Add(
      this ToolStrip menu,
      string text,
      MenuNames nameConstant)
    {
      return menu.Items.Add(text, nameConstant);
    }

    public static ToolStripMenuItem Add(
      this ToolStripItemCollection menuItems,
      string text,
      MenuNames nameConstant)
    {
      ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(text);
      toolStripMenuItem.Name = nameConstant.ToString();
      menuItems.Add((ToolStripItem) toolStripMenuItem);
      return toolStripMenuItem;
    }
  }
}
