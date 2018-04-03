using KeithLink.Common.Core.Interfaces.Settings;

using KeithLink.Common.Core.Models.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.Logic.Settings {
    public class AppSettingLogicImpl : IAppSettingLogic {
        #region attributes
        private const string KEY_UPDATEFLAG = "DBChangeValue";
        private const string COMMENT_UPDATEFLAG = "DB Has Changed (change this to make clients reset)";
        private const string COMMENT_FEATUREFLAG = "Feature Flags";

        private readonly IDBAppSettingsRepository   _repo;
        #endregion

        #region ctor
        public AppSettingLogicImpl(IDBAppSettingsRepository appSettingsRepo) {
            _repo = appSettingsRepo;
        }
        #endregion

        #region methods
        public Setting ReadFeatureFlag(string key) {
            Setting settng = _repo.Read(key);
            if (settng.Comment == COMMENT_FEATUREFLAG) {
                return settng;
            }
            return null;
        }

        public List<Setting> ReadAllSettings() {
            return _repo.ReadAll()
                        .Where(s => s.Key.Equals(KEY_UPDATEFLAG, StringComparison.InvariantCultureIgnoreCase) == false)
                        .OrderBy(s => s.Comment)
                        .ThenBy(s => s.Key)
                        .ToList();
        }

        public SettingUpdate SaveSetting(string key, string value) {
            // update the setting using the original values for comments and disabled
            Setting originalSetting = _repo.Read(key);

            _repo.Update(key, value, originalSetting.Comment, originalSetting.Disabled);

            _repo.Update(KEY_UPDATEFLAG, getRandomNumber().ToString(), COMMENT_UPDATEFLAG, false);

            // build the return value
            SettingUpdate retVal = new SettingUpdate();
            retVal.Key = key;
            retVal.UpdatedValue = value;
            retVal.OriginalValue = originalSetting.Value;

            return retVal;
        }

        public List<SettingUpdate> SaveSettings(List<Setting> settingsToUpdate)
        {
            List<SettingUpdate> returnValue = new List<SettingUpdate>();

            foreach (Setting s in settingsToUpdate)
            {
                returnValue.Add(SaveSetting(s.Key, s.Value));
            }

            return returnValue;
        }


        private double getRandomNumber()
        {
            Random randGen = new Random(DateTime.Now.Millisecond);
            return randGen.NextDouble();
        }

        #endregion
    }
}
