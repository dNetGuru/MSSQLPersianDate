using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using dNetTools.MSSQL.PersianDate;
using Microsoft.SqlServer.Server;

#pragma warning disable 219
// ReSharper disable MemberCanBeMadeStatic

/* •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
   | Code is published under terms mentioned on the project's page (on http://www.codeplex.com/PersianDate) under MIT Licence       |
   •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
   •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
   | Copyright (c) 2008 Farzad E. (dNetGuru)                                                                                        |
   |                                                                                                                                |
   | Permission is hereby granted, free of charge, to any person obtaining a copy of this software and                              |
   | associated documentation files (the "Software"), to deal in the Software without restriction, including                        |
   | without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell                        |
   | copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the                       |
   | following conditions:                                                                                                          |
   |                                                                                                                                |
   | The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. |
   |                                                                                                                                |
   | THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED                  |
   | TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL                  |
   | THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF                  |
   | CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER                       |
   | DEALINGS IN THE SOFTWARE.                                                                                                      |
   •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
   •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
   | Keep in mind that this is the initial release, Please send me you comments, feedbacks and especially feature requests !        |
   | Contact me via the Project's Page and/or mailto:farzade@gmail.com you can also particiapate in our community @ www.Hackerz.ir  |
   •————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•  */

[StructLayout(LayoutKind.Sequential), Serializable,
 SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 20, ValidationMethodName = "Validate")]
public struct PersianDate : INullable, IEquatable<PersianDate>, IBinarySerialize
{
    private int _dOM;
    private int _cMnt;
    private bool m_Null;

    #region Properties and Data Feilds / Related Methods

    public int Year { get; internal set; }

    public int Month
    {
        get { return _cMnt; }
        internal set
        {
            if (value < 13 && value > 0) _cMnt = value;
            else
                throw new ArgumentOutOfRangeException("value",
                                                      "Month has to be less or equal to 12 while being greater than zero.");
        }
    }

    public SqlString MonthName
    {
        get { return GetPersianMonthStr(Month); }
    }

    public SqlString DayOfWeekName
    {
        get { return GetPersianDoWStr(DayOfWeek); }
    }

    public int DayOfMonth
    {
        get { return _dOM; }
        internal set
        {
            if (value < 32 && value > 0) _dOM = value;
            else
                throw new ArgumentOutOfRangeException("value",
                                                      "Day of Month has to be less or equal to 31 while being greater than zero.");
        }
    }

    public Int32 DayOfWeek
    {
        get { return (int) new PersianCalendar().GetDayOfWeek(GetGeorgianDateTimeObj()); }
    }

    public DateTime GetGeorgianDateTimeObj()
    {
        return new DateTime(Year, Month, DayOfMonth, new PersianCalendar());
    }

    #endregion

    #region ToString/Parse Methods

    public override string ToString()
    {
        return String.Format("{0}/{1}/{2}", Year, Month, DayOfMonth);
    }

    public static PersianDate FromSQLDateTime(DateTime N)
    {
        var pCl = new PersianCalendar();
        return CreateDate(pCl.GetYear(N), pCl.GetMonth(N), pCl.GetDayOfMonth(N));
    }

    public string GetStringT(DateFormat Format)
    {
        switch (Format)
        {
            case DateFormat.Minimal:
                return ToString();
            case DateFormat.MinimalWithDoW:
                return String.Format("{0} {1}", GetPersianDoWStr(DayOfWeek), ToString());
            case DateFormat.MinimalWithMonthName:
                return String.Format("{0} {1}{2}", DayOfMonth, GetPersianMonthStr(Month), Year);
            case DateFormat.Long:
                return String.Format("{3}, {0} {1} {2}", DayOfMonth, GetPersianMonthStr(Month), Year,
                                     GetPersianDoWStr(DayOfWeek));
        }
        return String.Format("{0}/{1}/{2}", Year, Month, DayOfMonth);
    }

    public string GetString(Int16 format)
    {
        return GetStringT((DateFormat) format);
    }

    public static PersianDate Parse(SqlString s)
    {
        if (s.IsNull) return Null;
        try
        {
            string[] tmpStr = s.Value.Split('/');
            if (tmpStr.Length != 3)
                throw new ArgumentException(
                    "Input date must be delimitted by '/' and contain exactly three parts as in : yy/mm/dd");
            return CreateDate(int.Parse(tmpStr[0]), int.Parse(tmpStr[1]), int.Parse(tmpStr[2]));
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Input string was not in a correct format.", ex);
        }
    }

    public static PersianDate CreateDate(int year, int month, int day)
    {
        try
        {
            var nPc = new PersianCalendar();
            var _dt = new DateTime(year, month, day, nPc);
            var tVar = new PersianDate
                           {Year = nPc.GetYear(_dt), Month = nPc.GetMonth(_dt), DayOfMonth = nPc.GetDayOfMonth(_dt)};
            if (!tVar.Validate())
                throw new ArgumentOutOfRangeException("Created Date object did not pass the Validation process.",
                                                      (Exception) null);
            return tVar;
        }
        catch
        {
            return Null;
        }
    }

    #endregion

    #region String Representations

    public static SqlString GetPersianMonthStr(int month)
    {
        return PersianStrDic.MNamesDictionary[(PersianMonths) month];
    }

    public static string GetPersianDoWStr(int dayOfWeek)
    {
        return PersianStrDic.DoWDictionary[(PersianDoW) dayOfWeek].Value;
    }

    #endregion

    #region Public Enums

    #region DateFormat enum

    public enum DateFormat : byte
    {
        Long = 0x1,
        Minimal = 0x0,
        MinimalWithDoW = 2,
        MinimalWithMonthName
    }

    #endregion

    #region PersianDoW enum

    public enum PersianDoW
    {
        Shanbeh = 1,
        Yekshanbeh,
        Doshanbeh,
        Seshanbeh,
        Chaharshanbeh,
        Panjshanbeh,
        Jomeh,
        Error = 0xFF
    }

    #endregion

    #region PersianMonths enum

    public enum PersianMonths
    {
        Farvardin = 1,
        Ordibehesht,
        Khordad,
        Tir,
        Mordad,
        Shahrivar,
        Mehr,
        Aban,
        Azar,
        Dey,
        Bahman,
        Esfand
    }

    #endregion

    #endregion

    #region Nullable Equivalent Implantation

    public static PersianDate Null
    {
        get
        {
            var h = new PersianDate {m_Null = true};
            return h;
        }
    }

    public bool IsNull
    {
        get { return (this == Null); }
    }

    #endregion

    #region Static Members

    public static PersianDate Now
    {
        get
        {
            var pCl = new PersianCalendar();
            DateTime N = DateTime.Now;
            return CreateDate(pCl.GetYear(N), pCl.GetMonth(N), pCl.GetDayOfMonth(N));
        }
    }

    public SqlString Version
    {
        get { return "PersianDate Version 0.1 Build 35 (Preview)"; }
    }

    #endregion

    #region Operator Overloads

    public static bool operator ==(PersianDate cO, PersianDate cP)
    {
        return cO.Equals(cP);
    }

    public static bool operator !=(PersianDate cM, PersianDate cD)
    {
        return !cM.Equals(cD);
    }

    #endregion

    #region Validation Procedure

    private bool Validate()
    {
        // TODO: Additional Validation (Make Non-Static)
        return true;
    }

    #endregion

    #region IEquatable/HashCode Related Stuff

    public bool Equals(PersianDate obj)
    {
        return obj.DayOfMonth == DayOfMonth && obj.Year == Year && Equals(obj.Month, Month);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof (PersianDate)) return false;
        return Equals((PersianDate) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int result = _dOM;
            result = (result*397) ^ Year;
            result = (result*397) ^ Month.GetHashCode();
            return result;
        }
    }

    #endregion

    #region Implementation of IBinarySerialize

    public void Read(BinaryReader r)
    {
        Year = r.ReadInt32();
        if (Year == 0)
        {
            this = Null;
            return;
        }
        Month = r.ReadInt32();
        DayOfMonth = r.ReadInt32();
    }

    public void Write(BinaryWriter w)
    {
        if (IsNull)
        {
            w.Write(0);
            w.Write('\0');
            w.Write((decimal) 0);
            return;
        }
        w.Write(Year);
        w.Write(Month);
        w.Write(DayOfMonth);
    }

    #endregion
}