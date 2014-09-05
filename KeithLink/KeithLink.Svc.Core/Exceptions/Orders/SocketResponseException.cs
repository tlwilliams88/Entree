using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class SocketResponseException:Exception
    {
        #region ctor
        public SocketResponseException(string ExceptionMessage) : base (ExceptionMessage) { }

        public SocketResponseException(string ExceptionMessage, Exception ex):base(ExceptionMessage, ex){}
        #endregion
    }
}
