// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerMenuItem : RdcMenuItem
  {
    public ServerMenuItem(RdcTreeNode node)
    {
      this.Tag = (object) node;
      this.Text = node.Text;
    }

    public override void Update()
    {
      if (this.DropDownItems.Count != 0)
        return;
      this.Checked = ServerTree.Instance.SelectedNode == this.Tag;
    }

    protected override void OnClick() => ServerTree.Instance.SelectedNode = (TreeNode) this.Tag;
  }
}
