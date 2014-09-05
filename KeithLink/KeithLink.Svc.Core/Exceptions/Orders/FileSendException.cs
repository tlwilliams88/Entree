using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class FileSendException : Exception
    {
        #region ctor
        public FileSendException(string Exceptionmessage, string AffectedFile)
            : base(Exceptionmessage)
        {
            this.AffectedFile = AffectedFile;
        }

        public FileSendException(string ExceptionMessage, string AffectedFile, Exception ex)
            : base(ExceptionMessage, ex)
        {
            this.AffectedFile = AffectedFile;
        }
        #endregion

        #region properties
        public string AffectedFile { get; set; }
        #endregion
    }
}
