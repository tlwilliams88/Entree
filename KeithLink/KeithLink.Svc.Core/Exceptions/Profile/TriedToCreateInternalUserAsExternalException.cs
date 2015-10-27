using System;

namespace KeithLink.Svc.Core.Exceptions.Profile
{
    public class TriedToCreateInternalUserAsExternalException : Exception
    {
        #region ctor
        public TriedToCreateInternalUserAsExternalException() { }

        public TriedToCreateInternalUserAsExternalException(string Message) : base(Message) { }

        public TriedToCreateInternalUserAsExternalException(string Message, Exception ex) : base(Message, ex) { }
        #endregion
    }
}
