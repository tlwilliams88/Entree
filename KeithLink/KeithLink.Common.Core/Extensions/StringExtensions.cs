﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions {
	public static class StringExtensions {
        public static Guid ToGuid(this string value) {
            var parsedValue = Guid.Empty;

            if (!Guid.TryParse(value, out parsedValue))
                return Guid.Empty;

            return parsedValue;
        }

        public static DateTime? ToDateTime(this string value) {
            var parsedDate = new DateTime();

            if (!DateTime.TryParse(value, out parsedDate))
                return null;

            return parsedDate;
        }

        public static DateTime ToDateTime(this string value, DateTime defaultValue) {
            var parsedDate = new DateTime();

            if (!DateTime.TryParse(value, out parsedDate)) {
                return defaultValue;
            }

            return parsedDate;
        }

        public static string ToFormattedDateString( this string value ) {
            string returnValue = String.Empty;
            var parsedDate = new DateTime();

            if (DateTime.TryParse(value, out parsedDate)) {
                returnValue = parsedDate.ToLongDateFormat();
            }

            return returnValue;
        }

        public static string ToYearFirstDate( this string value ) {
            string returnValue = String.Empty;
            var parsedDate = new DateTime();

            if (DateTime.TryParse( value, out parsedDate )) {
                var date = value.Substring( 0, 10 ).Split( Char.Parse( "/" ) );

                returnValue = String.Format( "{0}{1}{2}", date[2], date[0], date[1] );
            }

            return returnValue;

        }

        public static string ToYearFirstDateWithTime( this string value ) {
            string returnValue = String.Empty;
            var parsedDate = new DateTime();

            if (DateTime.TryParse( value, out parsedDate )) {
                var date = value.Substring( 0, 10 ).Split( Char.Parse( "/" ) );
                var time = value.Substring(11).Split(Char.Parse(":"));

                returnValue = String.Format( "{0}{1}{2}{3}", date[2], date[0], date[1], String.Join("", time) );
            }

            return returnValue;
        }

        public static short? ToShort(this string value) {
            short parsedShort;

            if (!short.TryParse(value, out parsedShort))
                return null;

            return parsedShort;
        }

        public static short ToShort(this string value, short defaultValue) {
            short parsedShort;

            if (!short.TryParse(value, out parsedShort)) {
                return defaultValue;
            }

            return parsedShort;
        }

        public static double? ToDouble(this string value) {
            double parsedDouble;

            if (!double.TryParse(value, out parsedDouble))
                return null;
            return parsedDouble;
        }

        public static double ToDouble(this string value, double defaultValue) {
            double parsedDouble;

            if (!double.TryParse(value, out parsedDouble))
                return defaultValue;

            return parsedDouble;
        }

        public static decimal? ToDecimal(this string value) {
            decimal parsedDecimal;

            if (!decimal.TryParse(value, out parsedDecimal))
                return null;
            return parsedDecimal;
        }

        public static decimal ToDecimal(this string value, decimal defaultValue) {
            decimal parsedDecimal;

            if (!decimal.TryParse(value, out parsedDecimal))
                return defaultValue;

            return parsedDecimal;
        }

        public static int? ToInt(this string value) {
            int parsedInt;

            if (!int.TryParse(value, out parsedInt))
                return null;
            return parsedInt;
        }

        public static int ToInt(this string value, int defaultValue){
            int parsedInt;

            if (!int.TryParse(value, out parsedInt))
                return defaultValue;
            return parsedInt;
        }

        public static long? ToLong(this string value) {
            long parsedLong;

            if (!long.TryParse(value, out parsedLong)) {
                return null;
            }

            return parsedLong;
        }

        public static long ToLong(this string value, long defaultValue) {
            long parsedLong;

            if (!long.TryParse(value, out parsedLong)) {
                return defaultValue;
            }

            return parsedLong;
        }

        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching object properties.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, object injectionObject) {
            return formatString.Inject(GetPropertyHash(injectionObject));
        }
  
        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching dictionary entries.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="dictionary">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, IDictionary dictionary) {
            return formatString.Inject(new Hashtable(dictionary));
        }
  
        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching hashtable entries.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="attributes">A <see cref="Hashtable"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with hastable keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, Hashtable attributes) {
            string result = formatString;
            
            if (attributes == null || formatString == null)
                return result;
  
            foreach (string attributeKey in attributes.Keys) {
                result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
            }

            return result;
        }
  
        /// <summary>
        /// Replaces all instances of a 'key' (e.g. {foo} or {foo:SomeFormat}) in a string with an optionally formatted value, and returns the result.
        /// </summary>
        /// <param name="formatString">The string containing the key; unformatted ({foo}), or formatted ({foo:SomeFormat})</param>
        /// <param name="key">The key name (foo)</param>
        /// <param name="replacementValue">The replacement value; if null is replaced with an empty string</param>
        /// <returns>The input string with any instances of the key replaced with the replacement value</returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue) {
            string result = formatString;
            //regex replacement of key with value, where the generic key format is:
            //Regex foo = new Regex("{(foo)(?:}|(?::(.[^}]*)}))");
            Regex attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");  //for key = foo, matches {foo} and {foo:SomeFormat}
       
            //loop through matches, since each key may be used more than once (and with a different format string)
            foreach (Match m in attributeRegex.Matches(formatString)) {
                string replacement = m.ToString();

                if (m.Groups[2].Length > 0)  {
                    //matched {foo:SomeFormat}
                    //do a double string.Format - first to build the proper format string, and then to format the replacement value
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                } else {
                    //matched {foo}
                    replacement = (replacementValue ?? string.Empty).ToString();
                }
                //perform replacements, one match at a time
                result = result.Replace(m.ToString(), replacement);  //attributeRegex.Replace(result, replacement, 1);
            }
    
            return result;
        }
  
        /// <summary>
        /// Creates a HashTable based on current object state.
        /// <remarks>Copied from the MVCToolkit HtmlExtensionUtility class</remarks>
        /// </summary>
        /// <param name="properties">The object from which to get the properties</param>
        /// <returns>A <see cref="Hashtable"/> containing the object instance's property names and their values</returns>
        private static Hashtable GetPropertyHash(object properties) {
            Hashtable values = null;
        
            if (properties != null) {
                values = new Hashtable();
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(properties);
    
                foreach (PropertyDescriptor prop in props) {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }

            return values;
        }

	}
}