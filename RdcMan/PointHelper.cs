// Decompiled with JetBrains decompiler
// Type: RdcMan.PointHelper
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;

namespace RdcMan
{
  public static class PointHelper
  {
    public static readonly string Separator = ", ";

    public static Point Parse(string s)
    {
      string[] strArray = s.Split(new string[1]
      {
        PointHelper.Separator
      }, StringSplitOptions.None);
      return strArray.Length == 2 ? new Point(int.Parse(strArray[0]), int.Parse(strArray[1])) : throw new InvalidOperationException("Bad Point string '{0}'".InvariantFormat((object) s));
    }

    public static string ToFormattedString(this Point point) => "{0}{1}{2}".InvariantFormat((object) point.X, (object) PointHelper.Separator, (object) point.Y);
  }
}
