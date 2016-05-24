using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.Repository.Settings
{
    public class AppSettingsDictionary
    {
        private static Dictionary<string, string> dict;

        public static int CountSettings => dict.Keys.Count;

        public static StringBuilder UniqueValue { get; set; }

        public static DateTime UniqueCheck { get; set; }

        public static void AddSetting(string key, string value)
        {
            if (dict.ContainsKey(key) == false)
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = value; 
            }
        }

        public static void AddSettings(Dictionary<string, string> newdict)
        {
            dict = newdict;
        }

        public static void Reset()
        {
            if(dict == null)
            {
                dict = new Dictionary<string, string>();
            }
            if (dict.Keys.Count > 0)
            {
                dict.Clear();
            }
        }
        public static string GetSetting(string key)
        {
            return dict[key];
        }
    }
}
