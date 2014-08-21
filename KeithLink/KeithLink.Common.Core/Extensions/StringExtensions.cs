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
	}
}
