// Decompiled with JetBrains decompiler
// Type: RdcMan.StringUtilities
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Globalization;

namespace RdcMan
{
  public static class StringUtilities
  {
    private const char SetOpenChar = '{';
    private const char SetSeparatorChar = ',';
    private const char SetCloseChar = '}';
    private const char RangeOpenChar = '[';
    private const char RangeSeparatorChar = '-';
    private const char RangeCloseChar = ']';

    public static string CultureFormat(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.CurrentUICulture, format, args);

    public static string InvariantFormat(this string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);

    public static IEnumerable<string> ExpandPattern(string pattern)
    {
      bool anyExpansions = false;
      for (int i = 0; i < pattern.Length; ++i)
      {
        char c = pattern[i];
        switch (c)
        {
          case '[':
            int closeIndex1 = pattern.IndexOf(']', i);
            if (closeIndex1 == -1)
              throw new ArgumentException(string.Format("Range not closed (missing {0}): {1}", (object) ']', (object) pattern.Substring(i)));
            string prefix1 = pattern.Substring(0, i);
            string suffix1 = pattern.Substring(closeIndex1 + 1);
            IEnumerable<string> rangeEnumerator = StringUtilities.ExpandRange(pattern.Substring(i + 1, closeIndex1 - i - 1));
            foreach (string str1 in rangeEnumerator)
            {
              foreach (string str2 in StringUtilities.ExpandPattern(suffix1))
                yield return prefix1 + str1 + str2;
            }
            anyExpansions = true;
            goto label_19;
          case '{':
            int closeIndex2 = pattern.IndexOf('}', i);
            if (closeIndex2 == -1)
              throw new ArgumentException(string.Format("Set not closed (missing {0}): {1}", (object) '}', (object) pattern.Substring(i)));
            string prefix2 = pattern.Substring(0, i);
            string suffix2 = pattern.Substring(closeIndex2 + 1);
            IEnumerable<string> setEnumerator = StringUtilities.ExpandSet(pattern.Substring(i + 1, closeIndex2 - i - 1));
            foreach (string str1 in setEnumerator)
            {
              foreach (string str2 in StringUtilities.ExpandPattern(suffix2))
                yield return prefix2 + str1 + str2;
            }
            anyExpansions = true;
            goto label_19;
          default:
            continue;
        }
      }
label_19:
      if (!anyExpansions)
        yield return pattern;
    }

    private static IEnumerable<string> ExpandSet(string set) => (IEnumerable<string>) set.Split(',');

    private static IEnumerable<string> ExpandRange(string range)
    {
      string[] rangeValues = range.Split('-');
      string low = rangeValues.Length == 2 ? rangeValues[0] : throw new ArgumentException(string.Format("Range does not contain low and high values (single {0} separator): {1}", (object) '-', (object) range));
      string high = rangeValues[1];
      if (low.Length == 0 || high.Length == 0)
        throw new ArgumentException(string.Format("Range is missing a value: {0}", (object) range));
      if (char.IsLetter(low, 0))
      {
        if (!char.IsLetter(high, 0))
          throw new ArgumentException(string.Format("Range must be homogenous (letter bounds or numeric bounds): {0}", (object) range));
        if (low.Length != 1 || high.Length != 1)
          throw new ArgumentException(string.Format("Letter range must be single character: {0}", (object) range));
        if (char.IsLower(low[0]) != char.IsLower(high[0]))
          throw new ArgumentException(string.Format("Letter range must be same case: {0}", (object) range));
        int lowValue = low.CompareTo(high) <= 0 ? (int) low[0] : throw new ArgumentException(string.Format("Range low cannot be greater than high: {0}", (object) range));
        int highValue = (int) high[0];
        for (int value = lowValue; value <= highValue; ++value)
          yield return string.Format("{0}", (object) (char) value);
      }
      else
      {
        if (!char.IsDigit(low, 0))
          throw new ArgumentException(string.Format("Malformed range (must have letter bounds or numeric bounds): {0}", (object) range));
        int lowValue;
        int highValue;
        if (!int.TryParse(low, out lowValue) || !int.TryParse(high, out highValue))
          throw new ArgumentException(string.Format("Range must be homogenous (letter bounds or numeric bounds): {0}", (object) range));
        if (lowValue > highValue)
          throw new ArgumentException(string.Format("Range low cannot be greater than high: {0}", (object) range));
        int numDigits = low.Length;
        string format = "";
        for (int index = 0; index < numDigits; ++index)
        {
          // ISSUE: reference to a compiler-generated field
          this.\u003Cformat\u003E5__24 += "0";
        }
        for (int value = lowValue; value <= highValue; ++value)
          yield return value.ToString(format);
      }
    }
  }
}
