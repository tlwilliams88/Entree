using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Common.Core
{
    public class ValueParsingUtil
    {
        public static bool ParseBool(string value, string defaultValue)
        {
            bool? parsedValue = ParseBool(value);
            if (parsedValue == null)
                parsedValue = ParseBool(defaultValue);
            if (parsedValue == null)
                throw new FormatException(string.Format("'{0}' and '{1}' cannot be parsed as a boolean value", value, defaultValue));

            return parsedValue.Value;
        }

        private static bool? ParseBool(string value)
        {
            if (value.Trim() == "1" || value.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (value.Trim() == "0" || value.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                return false;

            return null;
        }

        public static int ParseInt(string value, string defaultValue)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return int.Parse(defaultValue);
            }
        }

        public static double ParseDouble(string value, string defaultValue)
        {
            try
            {
                return double.Parse(value);
            }
            catch
            {
                return double.Parse(defaultValue);
            }
        }

        public static DateTime ParseDateTime(string value, string defaultValue)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {
                return DateTime.Parse(defaultValue);
            }
        }

        public static Match ParseLDAPConnectionString(string ldap)
        {
            return Regex.Match(ldap, @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:\-@]+)\/(?<Container>[\w= ,@-]+)*");
        }
    }
}
