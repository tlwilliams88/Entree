
namespace KeithLink.Svc.Core.Models.Common {
    public class QueueReturn {
        #region properties
        public ulong DeliveryTag { get; set; }

        public string Message { get; set; }
        #endregion
    }
}
