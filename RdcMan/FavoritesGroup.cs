// Decompiled with JetBrains decompiler
// Type: RdcMan.FavoritesGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.ComponentModel.Composition;

namespace RdcMan
{
  [Export(typeof (IBuiltInVirtualGroup))]
  internal class FavoritesGroup : BuiltInVirtualGroup<FavoriteServerRef>, IServerRefFactory
  {
    public static FavoritesGroup Instance { get; private set; }

    private FavoritesGroup()
    {
      this.Text = "Favorites";
      FavoritesGroup.Instance = this;
    }

    public override FavoriteServerRef AddReference(ServerBase serverBase)
    {
      this.IsInTree = true;
      return base.AddReference(serverBase);
    }

    protected override string XmlNodeName => "favorites";

    public override bool CanDropServers() => true;

    public override bool HandleMove(RdcTreeNode childNode)
    {
      this.AddReference(childNode as ServerBase);
      return true;
    }

    public ServerRef Create(Server server) => (ServerRef) new FavoriteServerRef(server);
  }
}
