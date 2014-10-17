using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Events.EventArgs
{
    public class ExceptionEventArgs : System.EventArgs
    {
        #region constructor
        public ExceptionEventArgs(Exception e)
        {
            Exception = e;
        }
        #endregion

        #region properties
        public Exception Exception { get; set; }
        #endregion
    }
}
