﻿// Decompiled with JetBrains decompiler
// Type: RdcMan.FavoriteServerRef
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class FavoriteServerRef : ServerRef
  {
    public FavoriteServerRef(Server server)
      : base(server)
    {
    }

    public override bool CanRemove(bool popUI) => this.AllowEdit(popUI);
  }
}
