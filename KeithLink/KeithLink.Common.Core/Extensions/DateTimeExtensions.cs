using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions
{
    public static class DateTimeExtensions {
        #region attributes
        private static string centralTimeZoneCode = "Central Standard Time";
        #endregion

        #region methods
        public static string CentralTimeZoneName(this DateTime centralTime)
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(centralTimeZoneCode);
            if (centralTimeZone.IsDaylightSavingTime(centralTime))
                return centralTimeZone.DaylightName;
            else
                return centralTimeZone.StandardName;
        }

        public static DateTime ToCentralTime(this DateTime localTime)
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(centralTimeZoneCode);
            return TimeZoneInfo.ConvertTime(localTime, centralTimeZone);
        }

        public static string ToLongDateFormat(this DateTime value) {
            return value.ToString("MM/dd/yyyy");
        }

        public static string ToLongDateFormatWithTime(this DateTime value) {
            return value.ToString("MM/dd/yyyy HH:mm:ss");
        }

        public static string ToShortDateFormat(this DateTime value) {
            return value.ToString("M/d/yyyy");
        }

        public static string ToShortDateFormatWithTime(this DateTime value) {
            return value.ToString("M/d/yyyy HH:mm:ss");
        }

        public static string ToYearFirstFormat(this DateTime currentTime) {
            return currentTime.ToString("yyyyMMdd");
        }

        public static string ToYearFirstFormatWithTime(this DateTime currentTime) {
            return currentTime.ToString("yyyyMMddHHmmss");
        }
        #endregion
    }
}
