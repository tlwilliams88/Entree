using System;

namespace KeithLink.Svc.Core.Exceptions.Profile
{
    public class NoApiKeyProvidedException : Exception
    {
        #region ctor
        public NoApiKeyProvidedException() { }

        public NoApiKeyProvidedException(string Message) : base(Message) { }

        public NoApiKeyProvidedException(string Message, Exception ex) : base(Message, ex) { }
        #endregion
    }
}
