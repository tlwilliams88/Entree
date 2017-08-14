using System;

namespace KeithLink.Common.Core.Seams {
    public class SystemTime {
        #region attributes
        private static DateTime _date;
        #endregion

        #region methods
        public static void Set(DateTime custom) {
            _date = custom;
        }

        public static void Reset() {
            _date = DateTime.MinValue;;
        }
        #endregion

        #region properties
        public static DateTime Now {
            get {
                if(_date != DateTime.MinValue) {
                    return _date;
                } else {
                    return DateTime.Now;
                }
            }
        }
        #endregion
    }
}
