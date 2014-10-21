using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Events.EventArgs
{
    public class ReceivedFileEventArgs
    {
        #region attributes
        #endregion

        #region ctor
        public ReceivedFileEventArgs(string fileData)
        {
            FileData = fileData;
        }
        #endregion

        #region properties
        public string FileData { get; set; }
        #endregion
    }
}
