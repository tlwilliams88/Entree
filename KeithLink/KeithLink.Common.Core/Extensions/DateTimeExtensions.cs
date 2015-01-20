using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static string centralTimeZoneCode = "Central Standard Time";

        public static DateTime ToCentralTime(this DateTime localTime)
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(centralTimeZoneCode);
            return TimeZoneInfo.ConvertTime(localTime, centralTimeZone);
        }

        public static string CentralTimeZoneName(this DateTime centralTime)
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(centralTimeZoneCode);
            if (centralTimeZone.IsDaylightSavingTime(centralTime))
                return centralTimeZone.DaylightName;
            else
                return centralTimeZone.StandardName;
        }
    }
}
