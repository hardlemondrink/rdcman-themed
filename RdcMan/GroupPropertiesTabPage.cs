// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupPropertiesTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class GroupPropertiesTabPage : GroupBasePropertiesTabPage<GroupSettings>
  {
    public GroupPropertiesTabPage(TabbedSettingsDialog dialog, GroupSettings settings)
      : base(dialog, settings, settings.Name)
    {
      int rowIndex = 0;
      int tabIndex = 0;
      this.AddGroupName(ref rowIndex, ref tabIndex);
      this.AddParentCombo(ref rowIndex, ref tabIndex);
      this.AddComment(ref rowIndex, ref tabIndex).Setting = this.Settings.Comment;
    }
  }
}
