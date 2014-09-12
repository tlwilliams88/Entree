using System;

namespace KeithLink.Svc.Core.Exceptions.Profile
{
    public class InvalidApiKeyException : Exception
    {
        #region ctor
        public InvalidApiKeyException() { }

        public InvalidApiKeyException(string Message) : base(Message) { }

        public InvalidApiKeyException(string Message, Exception ex) : base(Message, ex) { }
        #endregion
    }
}
