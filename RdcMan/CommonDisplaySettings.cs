// Decompiled with JetBrains decompiler
// Type: RdcMan.CommonDisplaySettings
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public abstract class CommonDisplaySettings : SettingsGroup
  {
    public const string TabName = "Display Settings";

    protected CommonDisplaySettings()
      : base("Display Settings", "displaySettings")
    {
    }

    protected override void Copy(RdcTreeNode node) => this.Copy((SettingsGroup) node.DisplaySettings);

    [Setting("thumbnailScale", DefaultValue = 1)]
    public IntSetting ThumbnailScale { get; protected set; }

    [Setting("smartSizeDockedWindows")]
    public BoolSetting SmartSizeDockedWindow { get; protected set; }

    [Setting("smartSizeUndockedWindows")]
    public BoolSetting SmartSizeUndockedWindow { get; protected set; }
  }
}
