using KeithLink.Common.Impl.Repository.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Seams
{
    public class BEKConfiguration : Configuration
    {
        #region attributes
        private static Dictionary<string,string> _dict;
        #endregion

        #region methods
        public static void Add(string key, string value)
        {
            if(_dict == null) { _dict = new Dictionary<string, string>(); }
            _dict.Add(key, value);
        }

        public static void Reset()
        {
            if (_dict == null) { _dict = new Dictionary<string, string>(); }
            _dict.Clear(); ;
        }
        #endregion

        #region properties
        public static string Get(string key) {
            string value = null;
            if (_dict != null &&
                _dict.ContainsKey(key)) {
                value = _dict[key];
            } else {
                value = DBAppSettingsRepositoryImpl.GetValue(key, string.Empty);
            }
            return value;
        }
        #endregion
    }
}
