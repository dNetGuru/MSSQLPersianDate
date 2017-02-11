using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
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

[Serializable, SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 30)]

public struct PersianDateTime : INullable, IEquatable<PersianDateTime>, IBinarySerialize
{
    private bool m_Null;
    private PersianDate pDate;

    #region Properties and Data Feilds / Related Methods

    #region Date Related

    public int Year
    {
        get { return pDate.Year; }
        set { pDate.Year = value; }
    }

    public int Month
    {
        get { return pDate.Month; }
        private set { pDate.Month = value; }
    }

    public SqlString MonthName
    {
        get { return PersianDate.GetPersianMonthStr(Month); }
    }

    public SqlString DayOfWeekName
    {
        get { return PersianDate.GetPersianDoWStr(DayOfWeek); }
    }

    public int DayOfMonth
    {
        get { return pDate.DayOfMonth; }
        private set { pDate.DayOfMonth = value; }
    }

    public Int32 DayOfWeek
    {
        get { return pDate.DayOfWeek; }
    }

    public DateTime GetGeorgianDateTimeObj()
    {
        return pDate.GetGeorgianDateTimeObj();
    }

    #endregion

    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }
    public int Millisecond { get; set; }

    public long Ticks
    {
        get { return GetGeorgianDateTimeObj().Ticks; }
    }

    #endregion

    #region ToString/Parse Methods

    public override string ToString()
    {
        return String.Format("{0}/{1}/{2} {3}:{4}:{5}.{6}", Year, Month, DayOfMonth, Hour, Minute, Second, Millisecond);
    }

    public static PersianDateTime FromSQLDateTime(DateTime N)
    {
        var pCl = new PersianCalendar();
        return CreateDateTimeWMs
            (pCl.GetYear(N), pCl.GetMonth(N), pCl.GetDayOfMonth(N), pCl.GetHour(N), pCl.GetMinute(N), pCl.GetSecond(N),
             (int) pCl.GetMilliseconds(N));
    }

    public string GetStringT(PersianDate.DateFormat Format)
    {
        switch (Format)
        {
            case PersianDate.DateFormat.Minimal:
                return ToString();
            case PersianDate.DateFormat.MinimalWithDoW:
                return String.Format("{0} {1}", PersianDate.GetPersianDoWStr(DayOfWeek), ToString());
            case PersianDate.DateFormat.MinimalWithMonthName:
                return String.Format("{0} {1}{2}", DayOfMonth, PersianDate.GetPersianMonthStr(Month), Year);
            case PersianDate.DateFormat.Long:
                return String.Format("{3}, {0} {1} {2} {4}:{5}:{6}.{7}", DayOfMonth,
                                     PersianDate.GetPersianMonthStr(Month), Year,
                                     PersianDate.GetPersianDoWStr(DayOfWeek), Hour, Minute, Second, Millisecond);
        }
        return ToString();
    }

    public string GetString(Int16 format)
    {
        return GetStringT((PersianDate.DateFormat) format);
    }

    public static PersianDateTime Parse(SqlString s)
    {
        if (s.IsNull) return Null;
        try
        {
            string[] tmpStr = s.Value.Split('/');
            string[] sTmpStr = tmpStr[2].Split(':');
            int cutI = sTmpStr[0].IndexOf(' ');
            if (tmpStr.Length != 3 || sTmpStr.Length != 3 || cutI < 0)
                throw new ArgumentException
                    ("Input date must be delimitted by '/' and contain exactly three parts as in : YY/MM/DD HH:MM:SS.ms (providing milliseconds is optional)");
            string secStr = sTmpStr[2];
            string mSecStr = "0";
            int dotI = secStr.IndexOf('.');
            if (dotI > 0)
            {
                secStr = secStr.Substring(0, dotI);
                mSecStr = sTmpStr[2].Substring(dotI + 1);
            }
            return CreateDateTimeWMs
                (int.Parse(tmpStr[0]), int.Parse(tmpStr[1]), int.Parse(sTmpStr[0].Substring(0, cutI)),
                 int.Parse(sTmpStr[0].Substring(cutI + 1)), int.Parse(sTmpStr[1]), int.Parse(secStr), int.Parse(mSecStr));
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Input string was not in a correct format.", ex);
        }
    }

    #region CreateDateTime Overloads

    public static PersianDateTime CreateDateTimeWMs(int year, int month, int day, int hour, int min, int sec, int milsec)
    {
        try
        {
            var nPc = new PersianCalendar();
            var _dt = new DateTime(year, month, day, hour, min, sec, milsec, nPc);
            var tVar = new PersianDateTime
                           {
                               pDate = new PersianDate(),
                               Year = nPc.GetYear(_dt),
                               Month = nPc.GetMonth(_dt),
                               DayOfMonth = nPc.GetDayOfMonth(_dt),
                               Hour = nPc.GetHour(_dt),
                               Minute = nPc.GetMinute(_dt),
                               Second = nPc.GetSecond(_dt),
                               Millisecond = (int) nPc.GetMilliseconds(_dt)
                           };
            return tVar;
        }
        catch
        {
            return Null;
        }
    }

    public static PersianDateTime CreateDateTime(int year, int month, int day, int hour, int min, int sec)
    {
        try
        {
            var nPc = new PersianCalendar();
            var _dt = new DateTime(year, month, day, hour, min, sec, nPc);
            var tVar = new PersianDateTime
                           {
                               pDate = new PersianDate(),
                               Year = nPc.GetYear(_dt),
                               Month = nPc.GetMonth(_dt),
                               DayOfMonth = nPc.GetDayOfMonth(_dt),
                               Hour = nPc.GetHour(_dt),
                               Minute = nPc.GetMinute(_dt),
                               Second = nPc.GetSecond(_dt),
                               Millisecond = (int) nPc.GetMilliseconds(_dt)
                           };
            return tVar;
        }
        catch
        {
            return Null;
        }
    }

    #endregion

    #endregion

    #region Nullable Equivalent Implantation

    public static PersianDateTime Null
    {
        get
        {
            var h = new PersianDateTime {m_Null = true, pDate = new PersianDate()};
            return h;
        }
    }

    public bool IsNull
    {
        get { return (this == Null); }
    }

    #endregion

    #region IEquatable/HashCode Related Stuff

    public bool Equals(PersianDateTime obj)
    {
        return obj.DayOfMonth == DayOfMonth && obj.Year == Year && Equals(obj.Month, Month) &&
               obj.Hour == Hour && obj.Minute == Minute && obj.Second == Second && obj.Millisecond == Millisecond;
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof (PersianDateTime)) return false;
        return Equals((PersianDateTime) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int result = DayOfMonth;
            result = (result*397) ^ Year;
            result = (result*397) ^ Month.GetHashCode();
            result = (result*397) ^ Hour;
            result = (result*397) ^ Ticks.GetHashCode();
            result = (result*397) ^ Minute;
            result = (result*397) ^ Second.GetHashCode();
            result = (result*397) ^ Millisecond;
            return result;
        }
    }

    #endregion

    public static PersianDateTime NOW
    {
        get
        {
            var pCl = new PersianCalendar();
            DateTime N = DateTime.Now;
            return CreateDateTimeWMs
                (pCl.GetYear(N), pCl.GetMonth(N), pCl.GetDayOfMonth(N), pCl.GetHour(N), pCl.GetMinute(N),
                 pCl.GetSecond(N), (int) pCl.GetMilliseconds(N));
        }
    }

    public SqlString Version
    {
        get { return "PersianDateTime Version 0.1 Build 28 (Preview)"; }
    }

    #region Operator Overloads

    public static bool operator ==(PersianDateTime cO, PersianDateTime cP)
    {
        return cO.Equals(cP);
    }

    public static bool operator !=(PersianDateTime cM, PersianDateTime cD)
    {
        return !cM.Equals(cD);
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
        Hour = r.ReadInt32();
        Minute = r.ReadInt32();
        Second = r.ReadInt32();
        Millisecond = r.ReadInt32();
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
        w.Write(Hour);
        w.Write(Minute);
        w.Write(Second);
        w.Write(Millisecond);
    }

    #endregion
}