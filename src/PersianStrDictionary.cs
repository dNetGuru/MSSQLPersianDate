using System.Collections.Generic;
using System.Data.SqlTypes;
using PersianMonth = PersianDate.PersianMonths;
using DayOfWeek = PersianDate.PersianDoW;

namespace dNetTools.MSSQL.PersianDate
{
    internal static class PersianStrDic
    {
        public static readonly Dictionary<DayOfWeek, SqlString> DoWDictionary =
            new Dictionary<DayOfWeek, SqlString>
                {
                    {DayOfWeek.Jomeh, "جمعه"},
                    {DayOfWeek.Doshanbeh, "دوشنبه"},
                    {DayOfWeek.Shanbeh, "شنبه"},
                    {DayOfWeek.Yekshanbeh, "یکشنبه"},
                    {DayOfWeek.Panjshanbeh, "پنج شنبه"},
                    {DayOfWeek.Seshanbeh, "سه شنبه"},
                    {DayOfWeek.Chaharshanbeh, "چهار شنبه"}
                };

        public static readonly Dictionary<PersianMonth, SqlString> MNamesDictionary =
            new Dictionary<PersianMonth, SqlString>
                {
                    {PersianMonth.Farvardin, "فروردین"},
                    {PersianMonth.Ordibehesht, "اردیبهشت"},
                    {PersianMonth.Khordad, "خرداد"},
                    {PersianMonth.Tir, "تیر"},
                    {PersianMonth.Mordad, "مرداد"},
                    {PersianMonth.Shahrivar, "شهریور"},
                    {PersianMonth.Mehr, "مهر"},
                    {PersianMonth.Aban, "آبان"},
                    {PersianMonth.Azar, "آذر"},
                    {PersianMonth.Dey, "دی"},
                    {PersianMonth.Bahman, "بهمن"},
                    {PersianMonth.Esfand, "اسفند"},
                };
    }
}