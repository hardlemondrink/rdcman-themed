// Decompiled with JetBrains decompiler
// Type: RdcMan.RuleProperty
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public class RuleProperty
  {
    public RuleProperty(ServerProperty property) => this.ServerProperty = property;

    public ServerProperty ServerProperty { get; private set; }

    public object GetValue(Server server, out bool isString)
    {
      switch (this.ServerProperty)
      {
        case ServerProperty.DisplayName:
          isString = true;
          return (object) server.DisplayName;
        case ServerProperty.ServerName:
          isString = true;
          return (object) server.ServerName;
        case ServerProperty.Parent:
          isString = true;
          return (object) server.ParentPath;
        case ServerProperty.Comment:
          isString = true;
          return (object) server.Properties.Comment.Value;
        default:
          throw new NotImplementedException();
      }
    }
  }
}
