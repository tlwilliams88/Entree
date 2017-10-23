using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommerceServer.Foundation;

using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Seams
{
    public class FoundationService
    {
        #region attributes
        public static List<CommerceRequest> Requests { get; set; }
        #endregion

        public static CommerceResponse ExecuteRequest(CommerceRequest request)
        {
            if (Requests != null) {
                Requests.Add(request);
                return null;
            }
            else {
                return Helpers.FoundationService.ExecuteRequest(request);
            }
        }

    }
}
