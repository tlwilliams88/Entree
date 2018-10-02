using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Models.Logging
{
    public class TransactionContext
    {
        public string TransactionType { get; set; }

        public string TransactionId { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }
    }
}
