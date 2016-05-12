﻿using KeithLink.Common.Core.Extensions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Interfaces.Settings;

using KeithLink.Common.Core.Models.Settings;

using KeithLink.Svc.Core;

using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    /// <summary>
    /// end points that are only to be used by sys admins
    /// </summary>
    [Authorize(Roles = Constants.ROLE_NAME_SYSADMIN)]
    public class AdminController : BaseController {
        #region attributes
        private readonly IAppSettingLogic            _appSettings;
        private readonly IEventLogRepository         _log;
        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="logRepo"></param>
        /// <param name="appSettingsLogic"></param>
        public AdminController(IUserProfileLogic profileLogic, IEventLogRepository logRepo, IAppSettingLogic appSettingsLogic) : base(profileLogic) {
            _appSettings = appSettingsLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// return all of the application settings
        /// </summary>
        /// <returns>list of application settings</returns>
        [HttpGet]
        [ApiKeyedRoute("appsettings")]
        public OperationReturnModel<List<Setting>> ReadAllSettings() {
            OperationReturnModel<List<Setting>> retVal = new OperationReturnModel<List<Setting>>();

            try {
                retVal.SuccessResponse = _appSettings.ReadAllSettings();
                retVal.IsSuccess = true;
            } catch(Exception ex) {
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;

                _log.WriteErrorLog("Exception encountered while reading all settings", ex);
            }

            return retVal;
        }

        /// <summary>
        /// change the value of the application setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ApiKeyedRoute("appsettings")]
        public OperationReturnModel<bool> UpdateSetting(Setting model) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                SettingUpdate results = _appSettings.SaveSetting(model.Key, model.Value);

                string message = "App Setting updated by {UserName}. Key = {Key}, Original Value = {OrgValue}, Updated Value = {NewValue}";
                object stringValues = new {
                    UserName = this.AuthenticatedUser.UserId,
                    Key = results.Key,
                    OrgValue = results.OriginalValue,
                    NewValue = results.UpdatedValue
                };

                _log.WriteInformationLog(message.Inject(stringValues));

                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            } catch(Exception ex) {
                retVal.SuccessResponse = false;
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;

                _log.WriteErrorLog("Exception encountered while updating settings", ex);
            }

            return retVal;
        }
        #endregion
    }
}