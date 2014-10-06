using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Extensions {
    public static class GuidExtensions {
        public static string ToCommerceServerFormat(this Guid value) {
            //This is the format required by Commerce Server Guids
            //32 digits separated by hyphens, enclosed in braces:
            //{00000000-0000-0000-0000-000000000000}
            return value.ToString("B");
        }
    }
}
