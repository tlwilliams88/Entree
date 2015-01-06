using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions
{
	public static class StringExtensions
	{
		public static Guid ToGuid(this string value)
		{
			var parsedValue = Guid.Empty;

			if (!Guid.TryParse(value, out parsedValue))
				return Guid.Empty;

			return parsedValue;
		}

		public static DateTime? ToDateTime(this string value)
		{
			var parsedDate = new DateTime();

			if (!DateTime.TryParse(value, out parsedDate))
				return null;

			return parsedDate;
		}

		public static short? ToShort(this string value)
		{
			short parsedShort;

			if (!short.TryParse(value, out parsedShort))
				return null;

			return parsedShort;
		}

		public static double? ToDouble(this string value)
		{
			double parsedDouble;

			if (!double.TryParse(value, out parsedDouble))
				return null;
			return parsedDouble;
		}

		public static decimal? ToDecimal(this string value)
		{
			decimal parsedDecimal;

			if (!decimal.TryParse(value, out parsedDecimal))
				return null;
			return parsedDecimal;
		}

		public static int? ToInt(this string value)
		{
			int parsedInt;

			if (!int.TryParse(value, out parsedInt))
				return null;
			return parsedInt;
		}


        public static long? ToLong( this string value ) {
            long parsedLong;

            if (!long.TryParse( value, out parsedLong )) {
                return null;
            }

            return parsedLong;
        }        

	}
}
