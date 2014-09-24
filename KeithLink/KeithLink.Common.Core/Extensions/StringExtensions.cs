using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public static string ToCommerceServerFormat(this Guid value)
		{
			//This is the format required by Commerce Server Guids
			//32 digits separated by hyphens, enclosed in braces:
			//{00000000-0000-0000-0000-000000000000}
			return value.ToString("B"); 
		}
	}
}
