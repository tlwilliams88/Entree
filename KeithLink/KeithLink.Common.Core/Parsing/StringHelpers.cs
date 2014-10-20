using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Parsing
{
    public static class StringHelpers
    {
        /// <summary>
        /// Get a specific "field" from a line of text based on index and length.
        /// </summary>
        /// <param name="Index">Index to start parse at</param>
        /// <param name="Length">Length of field</param>
        /// <param name="Line">Line to parse field from</param>
        /// <returns></returns>
        public static string GetField(int Index, int Length, string Line)
        {
            return Line.Substring(Index, Length).Trim();
        }
    }
}
