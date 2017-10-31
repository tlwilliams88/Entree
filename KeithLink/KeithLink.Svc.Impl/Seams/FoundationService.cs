using System.Collections.Generic;

using CommerceServer.Foundation;

namespace KeithLink.Svc.Impl.Seams {
    public class FoundationService {
        #region attributes
        public static List<CommerceRequest> Requests { get; set; }
        #endregion

        public static CommerceResponse ExecuteRequest(CommerceRequest request) {
            if (Requests != null) {
                Requests.Add(request);
                return null;
            }
            return Helpers.FoundationService.ExecuteRequest(request);
        }
    }
}