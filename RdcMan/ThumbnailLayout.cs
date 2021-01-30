// Decompiled with JetBrains decompiler
// Type: RdcMan.ThumbnailLayout
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ThumbnailLayout : IDisposable, IEquatable<ThumbnailLayout>
  {
    public int[] ServerTileX;
    public int[] ServerTileY;
    public int[,] ServerLayoutToIndex;
    public ServerLabel[] LabelArray;
    private int[] _tabIndexToServerIndex;
    private Rectangle[] _thumbnailAbsoluteBounds;
    private bool[] _isServerPositionComputed;
    private int _maxNodeIndex;
    private bool _disposed;

    public int FocusedServerIndex { get; set; }

    public int[] TabIndexToServerIndex
    {
      get
      {
        this.EnsureTabIndex();
        return this._tabIndexToServerIndex;
      }
    }

    public bool IsServerPositionComputed(int index) => this._isServerPositionComputed[index];

    public int NodeCount => this.LabelArray.Length;

    public int LowestTileY { get; private set; }

    public GroupBase Group { get; private set; }

    public ThumbnailLayout(GroupBase group) => this.Group = group;

    ~ThumbnailLayout() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        ((IEnumerable<ServerLabel>) this.LabelArray).ForEach<ServerLabel>((Action<ServerLabel>) (l => l.Dispose()));
        this.LabelArray = (ServerLabel[]) null;
      }
      this._disposed = true;
    }

    public override string ToString() => "{0} ({1} servers)".InvariantFormat((object) this.Group.Text, (object) this.NodeCount);

    public void Compute(int numAcross)
    {
      using (Helpers.Timer("computing thumbnail layout"))
        this.LabelArray = this.CreateThumbnailList().ToArray();
      if (this.NodeCount == 0)
        return;
      using (Helpers.Timer("sorting {0} thumbnails", (object) this.NodeCount))
        Array.Sort<ServerLabel>(this.LabelArray, (IComparer<ServerLabel>) new ThumbnailLayout.LayoutComparer());
      this.SetThumbnailIndex();
      this.FocusedServerIndex = 1;
      this._maxNodeIndex = this.NodeCount + 1;
      int num1 = 0;
      List<List<ServerLabel>> serverLabelListList = new List<List<ServerLabel>>();
      List<ServerLabel> serverLabelList1 = (List<ServerLabel>) null;
      foreach (ServerLabel label in this.LabelArray)
      {
        int num2 = label.Server.DisplaySettings.ThumbnailScale.Value;
        if (serverLabelList1 != null && num1 == num2)
        {
          serverLabelList1.Add(label);
        }
        else
        {
          serverLabelList1 = new List<ServerLabel>();
          serverLabelList1.Add(label);
          serverLabelListList.Add(serverLabelList1);
          num1 = num2;
        }
      }
      int val2 = this.LabelArray[0].Server.DisplaySettings.ThumbnailScale.Value;
      this.ServerLayoutToIndex = new int[this.NodeCount * val2, Math.Max(numAcross, val2)];
      this._isServerPositionComputed = new bool[this._maxNodeIndex];
      this._thumbnailAbsoluteBounds = new Rectangle[this._maxNodeIndex];
      this.ServerTileX = new int[this._maxNodeIndex];
      this.ServerTileY = new int[this._maxNodeIndex];
      using (Helpers.Timer("laying out {0} thumbnails", (object) this.NodeCount))
      {
        int num2 = 0;
        int num3 = 0;
        while (serverLabelListList.Count > 0)
        {
          bool flag1 = false;
          int num4 = -1;
          for (int index1 = 0; index1 < serverLabelListList.Count; ++index1)
          {
            List<ServerLabel> serverLabelList2 = serverLabelListList[index1];
            ServerLabel serverLabel = serverLabelList2[0];
            int val1 = serverLabel.Server.DisplaySettings.ThumbnailScale.Value;
            if (num4 == -1 || val1 <= num4)
            {
              bool flag2 = false;
              if (num2 == 0 || num2 + val1 <= numAcross)
              {
                int num5 = Math.Min(val1, numAcross);
                bool flag3 = true;
                for (int index2 = 0; index2 < num5; ++index2)
                {
                  for (int index3 = 0; index3 < val1; ++index3)
                  {
                    if (this.ServerLayoutToIndex[num3 + index3, num2 + index2] != 0)
                    {
                      num4 = index2;
                      flag3 = false;
                      break;
                    }
                  }
                  if (!flag3)
                    break;
                }
                if (flag3)
                  flag2 = true;
              }
              if (flag2)
              {
                int thumbnailIndex = serverLabel.ThumbnailIndex;
                for (int index2 = 0; index2 < val1; ++index2)
                {
                  for (int index3 = 0; index3 < val1; ++index3)
                    this.ServerLayoutToIndex[num3 + index3, num2 + index2] = thumbnailIndex;
                }
                this.ServerTileX[thumbnailIndex] = num2;
                this.ServerTileY[thumbnailIndex] = num3;
                this.LowestTileY = Math.Max(this.LowestTileY, num3 + val1 - 1);
                flag1 = true;
                serverLabelList2.Remove(serverLabel);
                if (serverLabelList2.Count == 0)
                  serverLabelListList.Remove(serverLabelList2);
                num2 += val1;
                if (num2 >= numAcross)
                {
                  num2 = 0;
                  ++num3;
                  break;
                }
                break;
              }
            }
            else
              break;
          }
          if (!flag1 && ++num2 >= numAcross)
          {
            num2 = 0;
            ++num3;
          }
        }
      }
    }

    public void SetThumbnailIndex()
    {
      int num = 0;
      foreach (ServerLabel label in this.LabelArray)
        label.ThumbnailIndex = ++num;
    }

    public void SetThumbnailAbsoluteBounds(int serverIndex, int x, int y, int width, int height)
    {
      this._thumbnailAbsoluteBounds[serverIndex] = new Rectangle(x, y, width, height);
      this._isServerPositionComputed[serverIndex] = true;
    }

    public Rectangle GetThumbnailAbsoluteBounds(int serverIndex) => this._thumbnailAbsoluteBounds[serverIndex];

    public void EnsureTabIndex()
    {
      if (this._tabIndexToServerIndex != null)
        return;
      int upperBound = this.ServerLayoutToIndex.GetUpperBound(1);
      int lowestTileY = this.LowestTileY;
      int index1 = 1;
      this._tabIndexToServerIndex = new int[this._maxNodeIndex];
      ServerLabel label;
      for (int index2 = 0; index2 <= lowestTileY; ++index2)
      {
        for (int index3 = 0; index3 <= upperBound; index3 += label.Server.DisplaySettings.ThumbnailScale.Value)
        {
          int index4 = this.ServerLayoutToIndex[index2, index3];
          if (index4 != 0)
          {
            label = this.LabelArray[index4 - 1];
            if (this.ServerTileY[index4] == index2)
            {
              this._tabIndexToServerIndex[index1] = label.ThumbnailIndex;
              label.TabIndex = index1++;
            }
          }
          else
            break;
        }
      }
    }

    private List<ServerLabel> CreateThumbnailList()
    {
      List<ServerLabel> labelList = new List<ServerLabel>();
      HashSet<Server> set = new HashSet<Server>();
      bool useActualNode = this.Group is VirtualGroup;
      this.Group.VisitNodes((Action<RdcTreeNode>) (node =>
      {
        if (node is GroupBase groupBase)
        {
          groupBase.InheritSettings();
        }
        else
        {
          ServerBase serverBase = node as ServerBase;
          Server serverNode = serverBase.ServerNode;
          if (set.Contains(serverNode) || !(serverNode.Parent is GroupBase parent))
            return;
          parent.InheritSettings();
          if (!parent.DisplaySettings.ShowDisconnectedThumbnails.Value && !serverNode.IsConnected)
            return;
          labelList.Add(new ServerLabel(useActualNode ? serverBase : (ServerBase) serverNode));
          set.Add(serverNode);
        }
      }));
      return labelList;
    }

    public bool Equals(ThumbnailLayout other)
    {
      if (this.Group != other.Group || this.NodeCount != other.NodeCount)
        return false;
      for (int index = 0; index < this.NodeCount; ++index)
      {
        if (this.LabelArray[index].AssociatedNode != other.LabelArray[index].AssociatedNode || this.ServerTileX[index] != other.ServerTileX[index] || this.ServerTileY[index] != other.ServerTileY[index])
          return false;
      }
      return true;
    }

    private class LayoutComparer : IComparer<ServerLabel>
    {
      public int Compare(ServerLabel label1, ServerLabel label2)
      {
        Server server1 = label1.Server;
        Server server2 = label2.Server;
        server1.InheritSettings();
        server2.InheritSettings();
        int num1 = server1.DisplaySettings.ThumbnailScale.Value;
        int num2 = server2.DisplaySettings.ThumbnailScale.Value - num1;
        if (num2 != 0)
          return num2;
        List<TreeNode> path1 = label1.AssociatedNode.GetPath();
        List<TreeNode> path2 = label2.AssociatedNode.GetPath();
        int num3 = Math.Min(path1.Count, path2.Count);
        for (int index = 0; index < num3; ++index)
        {
          num2 = path1[index].Index - path2[index].Index;
          if (num2 != 0)
            return num2;
        }
        return num2;
      }
    }
  }
}
