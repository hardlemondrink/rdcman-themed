// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.Current
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Configuration;
using System.Drawing;

namespace RdcMan.Configuration
{
  public static class Current
  {
    public static RdcManSection RdcManSection { get; private set; }

    public static void Read()
    {
      Current.RdcManSection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("rdcman") as RdcManSection;
      DisplaySizeElementCollection displaySizes = Current.RdcManSection.DisplaySizes;
      int length = Math.Min(10, displaySizes.Count);
      SizeHelper.StockSizes = new Size[length];
      for (int index = 0; index < length; ++index)
        SizeHelper.StockSizes[index] = SizeHelper.Parse(displaySizes.GetDisplaySize(index).Size);
    }
  }
}
