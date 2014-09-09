using System;

namespace KeithLink.Svc.Core.Exceptions.Orders
{
    public class LateSocketException : Exception
    {
        #region ctor
        public LateSocketException(string ExceptionMessage, string[] AffectedFiles) : base (ExceptionMessage)
        {
            this.AffectedFiles = AffectedFiles;
        }

        public LateSocketException(string ExceptionMessage, string[] AffectedFiles, Exception ex) : base(ExceptionMessage, ex) 
        {
            this.AffectedFiles = AffectedFiles;
        }
        #endregion

        #region properties
        public string[] AffectedFiles { get; set; }
        #endregion
    }
}
