using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Interfaces.Settings;

using KeithLink.Common.Core.Models.Settings;

using KeithLink.Common.Impl.Repository.Logging;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KeithLink.Common.Impl.Repository.Settings
{
    public class DBAppSettingsRepositoryImpl : BaseDataConnection, IDBAppSettingsRepository
    {
        #region attributes
        private const int INDEX_COMMENT = 2;
        private const int INDEX_DISABLED = 3;
        private const int INDEX_KEY = 0;
        private const int INDEX_VALUE = 1;
        private const string KEY_UPDATEFLAG = "DBChangeValue";
        private const string PARMNAME_COMMENT = "Comment";
        private const string PARMNAME_DISABLED = "Disabled";
        private const string PARMNAME_KEY = "Key";
        private const string PARMNAME_VALUE = "Value";
        private const string SPNAME_GETONE = "[Configuration].[GetAppSetting]";
        private const string SPNAME_GETALL = "[Configuration].[ReadAppSettings]";
        private const string SPNAME_UPDATE = "[Configuration].[SaveAppSetting]";

        static DBAppSettingsRepositoryImpl _instance = new DBAppSettingsRepositoryImpl();
        private IEventLogRepository _log;
        private Dictionary<string, string> dict;
        private StringBuilder uniqueValue;
        private DateTime uniqueCheck;
        #endregion

        #region ctor
        private DBAppSettingsRepositoryImpl() {
            _log = new EventLogRepositoryImpl(Configuration.ApplicationName);

            Init();
        }
        #endregion

        #region methods
        private void CheckForAppSettingsChange() {
            try {
                TimeSpan t = DateTime.Now - uniqueCheck;
                if(t.TotalSeconds > int.Parse(Constants.DBAPPSETTINGS_TIME_THRESHOLD_MINUTES)) {
                    Setting updateFlag = Read(KEY_UPDATEFLAG);

                    if(updateFlag.Value.Equals(uniqueValue.ToString(), StringComparison.InvariantCultureIgnoreCase) == false) {
                        Init();
                    }
                }
            } catch(Exception ex) {
                _log.WriteErrorLog("AppSettings UniqueCheck failed", ex);
            }
        }

        public static DBAppSettingsRepositoryImpl GetInstance() {
            if(_instance == null) {
                _instance = new DBAppSettingsRepositoryImpl();
            }

            return _instance;
        }

        private string GetInstanceValue(string key, string defaultval) {
            CheckForAppSettingsChange();

            try {
                string val = dict[key];

                if (val == null) return defaultval;

                return val;
            } catch (Exception ex) {
                // Apparently we expect some key's not to be found
                //_log.WriteErrorLog(" DBAppSettingsRepository[" + key + "]", ex);
                return null;
            }
        }

        public static string GetValue(string key, string defval) {
            return DBAppSettingsRepositoryImpl.GetInstance().GetInstanceValue(key, defval);
        }

        private void Init() {
            try {
                _log.WriteInformationLog("Initializing DBAppSettingsRepository");

                List<Setting> settings = ReadAll();

                dict = settings.Where(s => s.Key.Equals(KEY_UPDATEFLAG, StringComparison.InvariantCultureIgnoreCase) == false )
                               .ToDictionary(s => s.Key, s => s.Value);

                Setting updateFlag = settings.Where(s => s.Key.Equals(KEY_UPDATEFLAG, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if(updateFlag != null) {
                    uniqueValue = new StringBuilder(updateFlag.Value);
                } else {
                    uniqueValue = new StringBuilder(string.Empty);
                }

                uniqueCheck = DateTime.Now;

                _log.WriteInformationLog(" DBAppSettingsRepository, " + dict.Count + " settings");
                _log.WriteInformationLog("Init DBAppSettingsRepository Complete");
            } catch(Exception ex) {
                _log.WriteErrorLog("Failed to initialize DBAppSettingsRepository", ex);
            }
        }


        public Setting Read(string key) {
            Setting retVal = new Setting();

            using (SqlDataReader rdr = GetDataReader(SPNAME_GETONE, PARMNAME_KEY, key)) {
                if(rdr.Read()) {
                    retVal.Key = rdr.GetString(INDEX_KEY);
                    retVal.Value = rdr.GetString(INDEX_VALUE);
                    retVal.Comment = rdr.GetString(INDEX_COMMENT);
                    retVal.Disabled = rdr.GetBoolean(INDEX_DISABLED);
                }
            }

            return retVal;
        }

        public List<Setting> ReadAll() {
            List<Setting> retVal = new List<Setting>();

            using(SqlDataReader rdr = GetDataReader(SPNAME_GETALL)) {
                while(rdr.Read()) {
                    Setting mySetting = new Setting();

                    mySetting.Key = rdr.GetString(INDEX_KEY);
                    mySetting.Value = rdr.GetString(INDEX_VALUE);
                    mySetting.Comment = rdr.GetString(INDEX_COMMENT);
                    mySetting.Disabled = rdr.GetBoolean(INDEX_DISABLED);

                    retVal.Add(mySetting);
                }
            }

            return retVal;
        }

        public void Update(string key, string value, string comment, bool disabled) {
            SqlCommand cmd = new SqlCommand(SPNAME_UPDATE);
            cmd.Parameters.AddWithValue(PARMNAME_COMMENT, comment);
            cmd.Parameters.AddWithValue(PARMNAME_DISABLED, disabled);
            cmd.Parameters.AddWithValue(PARMNAME_KEY, key);
            cmd.Parameters.AddWithValue(PARMNAME_VALUE, value);

            ExecuteCommand(cmd);
        }
        #endregion

    }
}
