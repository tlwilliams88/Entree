using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException() : base("Invalid response received from host") { }
    }
}
