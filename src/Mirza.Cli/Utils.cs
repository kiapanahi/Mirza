using System;
using System.Globalization;

namespace Mirza.Cli
{
    public static class Utils
    {
        public static string GetPersianDate(DateTime dt)
        {
            var pc = new PersianCalendar();

            var year = pc.GetYear(dt);
            var month = pc.GetMonth(dt).ToString().PadLeft(2, '0');
            var day = pc.GetDayOfMonth(dt).ToString().PadLeft(2, '0');
            return $"{year}/{month}/{day}";
        }

        public static string GetCurrentDateInPersian()
        {
            return GetPersianDate(DateTime.Today);
        }
    }
}