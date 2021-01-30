// Decompiled with JetBrains decompiler
// Type: RdcMan.SizeHelper
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;

namespace RdcMan
{
  public static class SizeHelper
  {
    public static Size[] StockSizes;
    public static readonly string Separator = " x ";
    public static readonly string AltSeparator = ", ";

    public static Size Parse(string dim)
    {
      string[] strArray = dim.Split(new string[2]
      {
        SizeHelper.Separator,
        SizeHelper.AltSeparator
      }, StringSplitOptions.None);
      return strArray.Length == 2 ? SizeHelper.FromString(strArray[0], strArray[1]) : throw new InvalidOperationException("Bad Size string '{0}'".InvariantFormat((object) dim));
    }

    public static Size FromString(string widthStr, string heightStr) => new Size(int.Parse(widthStr), int.Parse(heightStr));

    public static string ToFormattedString(this Size size) => "{0}{1}{2}".InvariantFormat((object) size.Width, (object) SizeHelper.Separator, (object) size.Height);
  }
}
