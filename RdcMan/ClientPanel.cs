// Decompiled with JetBrains decompiler
// Type: RdcMan.ClientPanel
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using Win32;

namespace RdcMan
{
  internal class ClientPanel : Control
  {
    private const int ThumbnailHorzSpace = 8;
    private const int ThumbnailVertSpace = 6;
    private static readonly int ThumbnailLabelHeight = ServerLabel.Height;
    private Dictionary<TreeNode, ThumbnailLayout> _layoutHash;
    private readonly Dictionary<TreeNode, int> _groupScrollPosition;
    private ThumbnailLayout _layout;
    private Size _savedSize;
    private int _thumbnailUnitWidth;
    private int _thumbnailUnitHeight;
    private bool[] _thumbnailDrawn;
    private readonly VScrollBar _verticalScrollBar;

    public ClientPanel()
    {
      this.TabStop = false;
      VScrollBar vscrollBar = new VScrollBar();
      vscrollBar.Dock = DockStyle.Right;
      vscrollBar.TabStop = false;
      vscrollBar.Visible = false;
      this._verticalScrollBar = vscrollBar;
      this._verticalScrollBar.Scroll += new ScrollEventHandler(this.OnScroll);
      this.Controls.Add((Control) this._verticalScrollBar);
      this.Dock = DockStyle.Fill;
      this.DoubleBuffered = false;
      this._groupScrollPosition = new Dictionary<TreeNode, int>();
      ServerTree.Instance.GroupChanged += new Action<GroupChangedEventArgs>(this.OnGroupChanged);
      ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(this.OnServerChanged);
      this._layoutHash = new Dictionary<TreeNode, ThumbnailLayout>();
    }

    private void OnGroupChanged(GroupChangedEventArgs obj)
    {
      if (obj.Group == ServerTree.Instance.RootNode && !obj.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged) || obj.Group != ServerTree.Instance.RootNode && !obj.ChangeType.HasFlag((Enum) ChangeType.InvalidateUI))
        return;
      ThumbnailLayout layout = this._layout;
      GroupBase group = layout?.Group;
      bool flag = false;
      if (layout != null)
      {
        ThumbnailLayout thumbnailLayout = this.CreateThumbnailLayout(group);
        if (!obj.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged) && thumbnailLayout.Equals(layout))
        {
          Log.Write("layout unchanged, not redrawing");
          thumbnailLayout.Dispose();
          this.UpdateNonLayoutSettings(layout);
        }
        else
        {
          this.HideGroup(group);
          layout.Dispose();
          this._layoutHash[(TreeNode) group] = thumbnailLayout;
          this.ShowGroup(group);
          flag = true;
        }
      }
      TreeNode key = (TreeNode) obj.Group;
      if (key != ServerTree.Instance.RootNode)
      {
        for (; key != null; key = key.Parent)
        {
          if (key == group)
          {
            if (!flag)
              break;
          }
          else
          {
            ThumbnailLayout thumbnailLayout;
            if (this._layoutHash.TryGetValue(key, out thumbnailLayout))
            {
              this._layoutHash.Remove(key);
              thumbnailLayout.Dispose();
            }
          }
        }
      }
      else
        this.ResetLayout();
      if (layout != null || !(ServerTree.Instance.SelectedNode is ServerBase selectedNode))
        return;
      this.UpdateNonLayoutSettings(selectedNode.ServerNode);
    }

    private void UpdateNonLayoutSettings(ThumbnailLayout shownLayout) => ((IEnumerable<ServerLabel>) shownLayout.LabelArray).ForEach<ServerLabel>((Action<ServerLabel>) (l => this.UpdateNonLayoutSettings(l.Server)));

    private void UpdateNonLayoutSettings(Server server)
    {
      server.SetClientSizeProperties();
      server.EnableDisableClient();
    }

    private void OnServerChanged(ServerChangedEventArgs obj)
    {
      if (!obj.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      using (Helpers.Timer("thumbnail ServerChanged handler"))
      {
        Server serverNode = obj.Server.ServerNode;
        foreach (ThumbnailLayout thumbnailLayout in this._layoutHash.Values)
        {
          foreach (ServerLabel label in thumbnailLayout.LabelArray)
          {
            if (label.Server == serverNode)
              label.CopyServerData();
          }
        }
      }
    }

    public RdcTreeNode GetSelectedNode(Control active) => active is ServerLabel serverLabel ? (RdcTreeNode) serverLabel.AssociatedNode : (RdcTreeNode) null;

    private void ResetLayout()
    {
      foreach (ThumbnailLayout thumbnailLayout in this._layoutHash.Values.Where<ThumbnailLayout>((Func<ThumbnailLayout, bool>) (l => l != this._layout)).ToList<ThumbnailLayout>())
      {
        thumbnailLayout.Dispose();
        this._layoutHash.Remove((TreeNode) thumbnailLayout.Group);
      }
    }

    [Conditional("DEBUG")]
    private void AssertValid()
    {
      foreach (Control control in (ArrangedElementCollection) this.Controls)
        ;
    }

    public void ShowGroup(GroupBase group)
    {
      bool flag = true;
      if (this._layout != null && this._layout.Group == group)
        flag = false;
      if (!this._layoutHash.TryGetValue((TreeNode) group, out this._layout))
      {
        this._layout = this.CreateThumbnailLayout(group);
        this._layoutHash.Add((TreeNode) group, this._layout);
      }
      this.ComputeScrollBarLimits();
      if (flag)
      {
        int num;
        if (!this._groupScrollPosition.TryGetValue((TreeNode) group, out num))
          num = 0;
        this.SetScrollPosition(num);
      }
      this._thumbnailDrawn = new bool[this._layout.NodeCount];
      this.DrawThumbnails(group);
    }

    private ThumbnailLayout CreateThumbnailLayout(GroupBase group)
    {
      ThumbnailLayout thumbnailLayout;
      using (Helpers.Timer("creating thumbnail layout for {0}", (object) group.Text))
      {
        thumbnailLayout = new ThumbnailLayout(group);
        int numAcross;
        if (Program.Preferences.ThumbnailSizeIsInPixels)
        {
          Size thumbnailSize = Program.Preferences.ThumbnailSize;
          this._thumbnailUnitWidth = thumbnailSize.Width;
          this._thumbnailUnitHeight = thumbnailSize.Height;
          numAcross = this.ComputeNumAcross(this.ClientSize.Width, this._thumbnailUnitWidth);
        }
        else
        {
          numAcross = 100 / Program.Preferences.ThumbnailPercentage;
          this._thumbnailUnitWidth = (this.ClientSize.Width - this._verticalScrollBar.Width - 8) / numAcross - 8;
          this._thumbnailUnitHeight = (this.ClientSize.Height - 6) / numAcross - ClientPanel.ThumbnailLabelHeight - 6;
          Program.Preferences.ThumbnailSize = new Size(this._thumbnailUnitWidth, this._thumbnailUnitHeight);
        }
        thumbnailLayout.Compute(numAcross);
      }
      return thumbnailLayout;
    }

    public void HideGroup(GroupBase group)
    {
      if (this._layout != null)
      {
        if (this._layout.Group != group)
          return;
        this._groupScrollPosition[(TreeNode) group] = this._verticalScrollBar.Value;
        try
        {
          ServerLabel activeControl = Program.TheForm.ActiveControl as ServerLabel;
          foreach (ServerLabel label in this._layout.LabelArray)
          {
            if (activeControl == label)
              Program.TheForm.ActiveControl = (Control) this;
            label.Server.Hide();
            this.Controls.Remove((Control) label);
          }
        }
        finally
        {
          this._layout = (ThumbnailLayout) null;
        }
      }
      this._verticalScrollBar.Hide();
    }

    public void ScrollServerIntoView(ServerLabel label)
    {
      int thumbnailIndex = label.ThumbnailIndex;
      if (!this._layout.IsServerPositionComputed(thumbnailIndex))
        this.ComputeThumbnailPosition(label);
      Rectangle thumbnailAbsoluteBounds = this._layout.GetThumbnailAbsoluteBounds(thumbnailIndex);
      int num = thumbnailAbsoluteBounds.Bottom + ServerLabel.Height;
      int height = this.ClientSize.Height;
      if (thumbnailAbsoluteBounds.Top >= this._verticalScrollBar.Value && num <= this._verticalScrollBar.Value + height - 1)
        return;
      int oldValue = this._verticalScrollBar.Value;
      this.SetScrollPosition(oldValue >= thumbnailAbsoluteBounds.Top ? thumbnailAbsoluteBounds.Top : num - height + 1);
      this.DrawAndScroll(oldValue, this._verticalScrollBar.Value);
    }

    private void ComputeScrollBarLimits()
    {
      int unitHeight = this.UnitHeight;
      int num = (this._layout.LowestTileY + 1) * unitHeight;
      int height = this.ClientSize.Height;
      if (!(this._verticalScrollBar.Visible = num - height > 0))
        return;
      this._verticalScrollBar.LargeChange = height;
      this._verticalScrollBar.SmallChange = unitHeight;
      this._verticalScrollBar.Maximum = num;
      this.SetScrollPosition(this._verticalScrollBar.Value);
    }

    private int UnitHeight => this._thumbnailUnitHeight + ClientPanel.ThumbnailLabelHeight + 6;

    private int GetServerHeight(int serverScale) => this._thumbnailUnitHeight * serverScale + (ClientPanel.ThumbnailLabelHeight + 6) * (serverScale - 1);

    private void SetScrollPosition(int value) => this._verticalScrollBar.Value = Math.Min(value, this._verticalScrollBar.Maximum - this._verticalScrollBar.LargeChange + 1);

    private void DrawThumbnails(GroupBase group)
    {
      if (this._layout.NodeCount == 0)
        return;
      using (Helpers.Timer("drawing {0} ({1} thumbnails)", (object) group.Text, (object) this._layout.NodeCount))
        this.DrawThumbnails(this._verticalScrollBar.Value, this._verticalScrollBar.Value, this.ClientSize.Height);
    }

    private void DrawThumbnails(int oldValue, int newValue, int height)
    {
      foreach (int index in this.GetUndrawnServersInViewport(newValue, height))
      {
        this._thumbnailDrawn[index - 1] = true;
        ServerLabel label = this._layout.LabelArray[index - 1];
        if (!this._layout.IsServerPositionComputed(index))
          this.ComputeThumbnailPosition(label);
        this.DrawThumbnail(label, oldValue);
      }
    }

    private void DrawThumbnail(ServerLabel label, int windowTop)
    {
      Rectangle thumbnailAbsoluteBounds = this._layout.GetThumbnailAbsoluteBounds(label.ThumbnailIndex);
      int y = thumbnailAbsoluteBounds.Top - windowTop;
      int top = y + ClientPanel.ThumbnailLabelHeight - 1;
      Server server = label.Server;
      server.SetThumbnailView(thumbnailAbsoluteBounds.X, top, thumbnailAbsoluteBounds.Width, thumbnailAbsoluteBounds.Height);
      label.Size = new Size(thumbnailAbsoluteBounds.Width, ClientPanel.ThumbnailLabelHeight);
      label.Location = new Point(thumbnailAbsoluteBounds.X, y);
      this.Controls.Add((Control) label);
      label.Show();
      server.Show();
      if (Program.TheForm.ActiveControl != this || label.ThumbnailIndex != this._layout.FocusedServerIndex)
        return;
      label.Focus();
      this._layout.FocusedServerIndex = 0;
    }

    private unsafe void DrawAndScroll(int oldValue, int newValue)
    {
      using (Helpers.Timer("scrolling thumbnails {0} => {1}", (object) oldValue, (object) newValue))
      {
        Size clientSize = this.ClientSize;
        int height = clientSize.Height;
        this.DrawThumbnails(oldValue, newValue, height);
        if (oldValue == newValue)
          return;
        Structs.RECT rect1 = new Structs.RECT()
        {
          top = 0,
          bottom = clientSize.Height,
          left = 0,
          right = this._verticalScrollBar.Left - 1
        };
        Structs.RECT rect2 = new Structs.RECT()
        {
          top = -oldValue,
          bottom = this._verticalScrollBar.Maximum - oldValue,
          left = 0,
          right = this._verticalScrollBar.Left - 1
        };
        Structs.RECT* rectPtr1 = &rect1;
        Structs.RECT* rectPtr2 = &rect2;
        User.ScrollWindowEx(this.Handle, 0, oldValue - newValue, (IntPtr) (void*) rectPtr2, (IntPtr) (void*) rectPtr1, (IntPtr) (void*) null, (IntPtr) (void*) null, 7U);
      }
    }

    protected override void OnClientSizeChanged(EventArgs e)
    {
      if (Program.TheForm.IsFullScreen)
        return;
      Size clientSize = this.ClientSize;
      try
      {
        switch (ServerTree.Instance.SelectedNode)
        {
          case GroupBase group:
            if (this._layout == null || this._layout.Group != group)
              return;
            if (Program.Preferences.ThumbnailSizeIsInPixels)
            {
              int width = Program.Preferences.ThumbnailSize.Width;
              if (this.ComputeNumAcross(clientSize.Width, width) == this.ComputeNumAcross(this._savedSize.Width, width))
              {
                if (this._savedSize.Height == clientSize.Height)
                  return;
                int oldValue = this._verticalScrollBar.Value;
                this.ComputeScrollBarLimits();
                this.DrawAndScroll(oldValue, this._verticalScrollBar.Value);
                return;
              }
            }
            this.HideGroup(group);
            this.ResetLayout();
            this.ShowGroup(group);
            return;
          case ServerBase serverBase:
            if (serverBase.IsThumbnail)
              throw new InvalidOperationException("Selected server is a thumbnail");
            serverBase.ServerNode.SetNormalView();
            break;
        }
        this.ResetLayout();
      }
      finally
      {
        this._savedSize = clientSize;
      }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      if (!(ServerTree.Instance.SelectedNode is GroupBase))
        return;
      int oldValue = this._verticalScrollBar.Value;
      this.SetScrollPosition(Math.Max(0, oldValue - Math.Sign(e.Delta) * this._verticalScrollBar.SmallChange));
      this.DrawAndScroll(oldValue, this._verticalScrollBar.Value);
    }

    private int ComputeNumAcross(int totalWidth, int unitWidth)
    {
      totalWidth -= this._verticalScrollBar.Width;
      totalWidth -= 8;
      return totalWidth / (unitWidth + 8);
    }

    private void ComputeThumbnailPosition(ServerLabel label)
    {
      int serverScale = label.Server.DisplaySettings.ThumbnailScale.Value;
      int width = this._thumbnailUnitWidth * serverScale + 8 * (serverScale - 1);
      int serverHeight = this.GetServerHeight(serverScale);
      int thumbnailIndex = label.ThumbnailIndex;
      int num1 = this._layout.ServerTileY[thumbnailIndex];
      int num2 = this._layout.ServerTileX[thumbnailIndex];
      int x = (num2 + 1) * 8 + num2 * this._thumbnailUnitWidth;
      int y = (num1 + 1) * 6 + num1 * (this._thumbnailUnitHeight + ClientPanel.ThumbnailLabelHeight);
      this._layout.SetThumbnailAbsoluteBounds(thumbnailIndex, x, y, width, serverHeight);
    }

    private IEnumerable<int> GetUndrawnServersInViewport(int position, int height)
    {
      HashSet<int> intSet = new HashSet<int>();
      if (this._layout.NodeCount == 0)
        return (IEnumerable<int>) intSet;
      int upperBound = this._layout.ServerLayoutToIndex.GetUpperBound(1);
      int unitHeight = this.UnitHeight;
      int num1 = position / unitHeight;
      int num2 = Math.Min((position + height - 1) / unitHeight, this._layout.ServerLayoutToIndex.GetUpperBound(0));
      ServerLabel label;
      for (int index1 = num1; index1 <= num2; ++index1)
      {
        for (int index2 = 0; index2 <= upperBound; index2 += label.Server.DisplaySettings.ThumbnailScale.Value)
        {
          int num3 = this._layout.ServerLayoutToIndex[index1, index2];
          if (num3 != 0)
          {
            if (!this._thumbnailDrawn[num3 - 1])
              intSet.Add(num3);
            label = this._layout.LabelArray[num3 - 1];
          }
          else
            break;
        }
      }
      return (IEnumerable<int>) intSet;
    }

    private void OnScroll(object sender, ScrollEventArgs e) => this.DrawAndScroll(e.OldValue, e.NewValue);

    public void RecordLastFocusedServerLabel(ServerLabel label) => this._layout.FocusedServerIndex = label.ThumbnailIndex;

    protected override void OnEnter(EventArgs e)
    {
      bool flag = true;
      base.OnEnter(e);
      if (this._layout != null)
      {
        if (this._layout.NodeCount > 0)
        {
          ServerLabel label = this._layout.LabelArray[this._layout.FocusedServerIndex - 1];
          if (label.Parent == this)
          {
            label.Focus();
            flag = false;
          }
        }
      }
      else if (ServerTree.Instance.SelectedNode is ServerBase selectedNode)
      {
        selectedNode.Focus();
        flag = false;
      }
      if (!flag)
        return;
      this.Focus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (!(Program.TheForm.ActiveControl is ServerLabel serverLabel))
      {
        if (this._layout == null || Program.TheForm.ActiveControl != this)
          return base.ProcessCmdKey(ref msg, keyData);
        serverLabel = this._layout.LabelArray[this._layout.FocusedServerIndex - 1];
      }
      this._layout.EnsureTabIndex();
      int serverScale = serverLabel.Server.DisplaySettings.ThumbnailScale.Value;
      int thumbnailIndex = serverLabel.ThumbnailIndex;
      int index1 = this._layout.ServerTileX[thumbnailIndex];
      int num1 = this._layout.ServerTileY[thumbnailIndex];
      int tabIndex = serverLabel.TabIndex;
      int index2;
      switch (keyData)
      {
        case Keys.Tab:
        case Keys.Tab | Keys.Shift:
          ServerTree.Instance.Focus();
          return true;
        case Keys.Prior:
          if (num1 == 0)
            return true;
          int num2 = Math.Max((int) Math.Floor((double) serverLabel.Top / (double) this.UnitHeight), this.Height / this.UnitHeight);
          index2 = Math.Max(0, num1 - num2);
          break;
        case Keys.Next:
          if (num1 == this._layout.LowestTileY)
            return true;
          int num3 = Math.Max((int) Math.Floor((double) (serverLabel.Top + this.GetServerHeight(serverScale)) / (double) this.UnitHeight), this.Height / this.UnitHeight);
          index2 = Math.Min(this._layout.LowestTileY, num1 + num3);
          break;
        case Keys.End:
          index1 = this._layout.ServerLayoutToIndex.GetUpperBound(1);
          index2 = this._layout.LowestTileY;
          break;
        case Keys.Home:
          index1 = 0;
          index2 = 0;
          break;
        case Keys.Left:
          int index3;
          if ((index3 = tabIndex - 1) == 0)
            return true;
          int index4 = this._layout.TabIndexToServerIndex[index3];
          index1 = this._layout.ServerTileX[index4];
          index2 = this._layout.ServerTileY[index4];
          break;
        case Keys.Up:
          if ((index2 = num1 - 1) < 0)
            return true;
          break;
        case Keys.Right:
          int index5;
          if ((index5 = tabIndex + 1) > this._layout.NodeCount)
            return true;
          int index6 = this._layout.TabIndexToServerIndex[index5];
          index1 = this._layout.ServerTileX[index6];
          index2 = this._layout.ServerTileY[index6];
          break;
        case Keys.Down:
          index2 = num1 + serverScale;
          if (index2 > this._layout.LowestTileY)
            return true;
          break;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
      int num4;
      while (true)
      {
        num4 = this._layout.ServerLayoutToIndex[index2, index1];
        if (num4 == 0)
          --index1;
        else
          break;
      }
      ServerLabel label = this._layout.LabelArray[num4 - 1];
      this.ScrollServerIntoView(label);
      label.Focus();
      return true;
    }
  }
}
