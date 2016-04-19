using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.SettingsRepo
{
    public class DBAppSettingsRepositoryImpl
    {
        static DBAppSettingsRepositoryImpl _instance = new DBAppSettingsRepositoryImpl();
        private Dictionary<string, string> dict;
        private string uniqueValue;
        private DateTime uniqueCheck;
        private IEventLogRepository _log;

        public static DBAppSettingsRepositoryImpl getInstance()
        {
            if (_instance == null)
            {
                _instance = new DBAppSettingsRepositoryImpl();
            }
            return _instance;
        }
        private DBAppSettingsRepositoryImpl()
        {
            // Initialize
            string name = KeithLink.Common.Impl.Configuration.ApplicationName;
            _log = new EventLogRepositoryImpl(name);
            Init();
        }
        private void Init()
        {
            try
            {
                _log.WriteInformationLog("Initializing DBAppSettingsRepository");
                dict = new Dictionary<string, string>();
                using (var conn = new SqlConnection(KeithLink.Common.Impl.Configuration.AppDataConnectionString))
                {
                    using (var cmd = new SqlCommand("[Configuration].[ReadAppSettings]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                try
                                {
                                    string key = Convert.ToString(reader["Key"]);
                                    if (key.Equals("DBChangeValue", StringComparison.CurrentCultureIgnoreCase)) uniqueValue = Convert.ToString(reader["Value"]);
                                    else dict.Add(key, Convert.ToString(reader["Value"]));
                                }
                                catch (Exception ex)
                                {
                                    _log.WriteErrorLog(" DBAppSettingsRepository[" + reader["Key"] + "]", ex);
                                }
                            }
                        }
                        uniqueCheck = DateTime.Now;
                        _log.WriteInformationLog(" DBAppSettingsRepository, " + dict.Count + " settings");
                    }
                }
                _log.WriteInformationLog("Init DBAppSettingsRepository Complete");
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Failed to initialize DBAppSettingsRepository", ex);
            } 
        }
        public static string GetValue(string key, string defval)
        {
            DBAppSettingsRepositoryImpl appsettings = DBAppSettingsRepositoryImpl.getInstance();
            return appsettings.GetInstanceValue(key, defval);
        }
        private string GetInstanceValue(string key, string defaultval)
        {
            CheckForAppSettingsChange();
            try
            {
                string val = dict[key];
                //_log.WriteInformationLog(" DBAppSettingsRepository, dict[" + key + "] = " + val);
                if (val == null) return defaultval;
                return val;
            }
            catch (Exception ex)
            {
                // Apparently we expect some key's not to be found
                //_log.WriteErrorLog(" DBAppSettingsRepository[" + key + "]", ex);
                return null;
            }
        }

        private void CheckForAppSettingsChange()
        {
            try
            {
                TimeSpan t = DateTime.Now - uniqueCheck;
                if (t.TotalSeconds > 3)
                {
                    uniqueCheck = DateTime.Now;
                    using (var conn = new SqlConnection(KeithLink.Common.Impl.Configuration.AppDataConnectionString))
                    {
                        using (var cmd = new SqlCommand("[Configuration].[CheckAppSettingsForChange]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            conn.Open();
                            string checkval = cmd.ExecuteScalar().ToString();
                            if (checkval.Equals(uniqueValue, StringComparison.CurrentCultureIgnoreCase) == false)
                            {
                                Init();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _log.WriteErrorLog("AppSettings UniqueCheck failed", ex);
            }
        }
    }
}
