// Decompiled with JetBrains decompiler
// Type: RdcMan.VirtualGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal abstract class VirtualGroup : GroupBase
  {
    protected VirtualGroup() => this.ChangeImageIndex(ImageConstants.Group);

    protected override void InitSettings()
    {
      this.Properties = new GroupSettings();
      base.InitSettings();
    }

    public override sealed bool CanAddServers() => false;

    public override sealed bool CanAddGroups() => false;

    public override sealed bool CanDropGroups() => false;

    protected IServerRefFactory ServerRefFactory => this as IServerRefFactory;
  }
}
