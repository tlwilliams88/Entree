using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions
{
    public static class SQLDataRowExtentions
    {
        public static int? GetNullableInt(this DataRow row, string colName)
        {
            if (row[colName] == DBNull.Value)
                return null;

            var value = row[colName].ToString();

            int intValue = 0;
            int.TryParse(value, out intValue);

            return (int?)intValue;
        }

        public static int GetInt(this DataRow row, string colName)
        {
            var value = row[colName].ToString();

            int intValue = 0;
            int.TryParse(value, out intValue);

            return (int)intValue;
        }

        public static byte? GetNullableByte(this DataRow row, string colName)
        {
            if (row[colName] == DBNull.Value)
                return null;

            var value = row[colName].ToString();

            byte byteValue = 0;
            byte.TryParse(value, out byteValue);

            return (byte?)byteValue;
        }

        public static byte GetByte(this DataRow row, string colName)
        {
            var value = row[colName].ToString();

            byte byteValue = 0;
            byte.TryParse(value, out byteValue);

            return (byte)byteValue;
        }

        public static bool GetBool(this DataRow row, string colName)
        {
            return (bool)row[colName];
        }

        public static bool? GetNullableBool(this DataRow row, string colName)
        {
            return row[colName] == DBNull.Value ? null : (bool?)row[colName];
        }

        public static string GetString(this DataRow row, string colName, bool trim = true)
        {
            return row[colName] == DBNull.Value ? string.Empty : trim ? row[colName].ToString().Trim() : row[colName].ToString();
        }

        public static char? GetNullableChar(this DataRow row, string colName)
        {
            return row[colName] == DBNull.Value ? null : (char?)char.Parse(row[colName].ToString());
        }

        public static Guid GetGuid(this DataRow row, string colName)
        {
            return row[colName] == DBNull.Value ? Guid.Empty : Guid.Parse(row[colName].ToString());
        }

        public static DateTime? GetNullableDateTime(this DataRow row, string colName)
        {
            return row[colName] == DBNull.Value ? null : (DateTime?)row[colName];
        }

        public static double? GetNullableDouble(this DataRow row, string colName)
        {
            if (row[colName] == DBNull.Value)
                return null;

            var value = row[colName].ToString();

            double doubleValue = 0;
            double.TryParse(value, out doubleValue);

            return (double?)doubleValue;
        }
        public static decimal? GetNullableDecimal(this DataRow row, string colName)
        {
            if (row[colName] == DBNull.Value)
                return null;

            var value = row[colName].ToString();

            decimal doubleValue = 0;
            decimal.TryParse(value, out doubleValue);

            return (decimal?)doubleValue;
        }

        public static decimal GetDecimal(this DataRow row, string colName)
        {
            var value = row[colName].ToString();

            decimal decimalValue = 0;
            decimal.TryParse(value, out decimalValue);

            return decimalValue;
        }

        public static double GetDouble(this DataRow row, string colName)
        {
            var value = row[colName].ToString();

            double doubleValue = 0;
            double.TryParse(value, out doubleValue);

            return doubleValue;
        }

        public static Guid? GetNullableGuid(this DataRow row, string colName)
        {
            if (row[colName] == DBNull.Value)
                return null;

            return Guid.Parse(row[colName].ToString());
        }
    }
}
