using System;

namespace KeithLink.Svc.Core.Models {
    public abstract class AuditableEntity : Entity {
        #region attributes
        private DateTime _created;

        private DateTime _modified;
        #endregion

        #region methods
        private DateTime ConvertToUtc(DateTime value) {
            DateTime retVal = new DateTime();

            if(value.Kind == DateTimeKind.Unspecified) {
                retVal = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
            } else if(value.Kind == DateTimeKind.Local) {
                retVal = value.ToUniversalTime();
            } else {
                retVal = value;
            }

            return retVal;
        }
        #endregion

        #region properties
        public DateTime CreatedUtc {
            get { return _created; }
            set { _created = ConvertToUtc(value); }
        }
        public DateTime ModifiedUtc {
            get { return _modified; }
            set { _modified = ConvertToUtc(value); }
        }
        #endregion

    }
}
