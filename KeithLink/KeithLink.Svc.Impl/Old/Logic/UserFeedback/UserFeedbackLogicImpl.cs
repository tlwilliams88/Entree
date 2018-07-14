using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Enumerations.Messaging;
using Entree.Core.Interface.UserFeedback;
using Entree.Core.Models.UserFeedback;

using System;

namespace KeithLink.Svc.Impl.Logic.UserFeedback
{
    public class UserFeedbackLogicImpl : IUserFeedbackLogic
    {
        #region attributes
        private readonly IUserFeedbackRepository _userFeedbackRepo;
        private readonly IEventLogRepository _eventLog;
        #endregion

        #region ctor
        public UserFeedbackLogicImpl(
                                    IUserFeedbackRepository userFeedbackRepo,
                                    IEventLogRepository eventLog
                                    )
        {
            _userFeedbackRepo = userFeedbackRepo;
            _eventLog = eventLog;
        }
        #endregion

        #region methods

        /// <summary>
        /// Validates all of the attributes of the user's feedback.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void AssertUserFeedback(Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (userFeedback.Subject.Length == 0)
                throw new ApplicationException("Subject is blank.");

            if (userFeedback.Content.Length == 0)
                throw new ApplicationException("Content is blank.");

            if (Enum.IsDefined(typeof(Audience), userFeedback.Audience) == false)
                throw new ApplicationException("Audience value is invalid.");

        }


        /// <summary>
        /// Saves user feedback.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userFeedback"></param>
        /// <returns></returns>
        public void SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (userFeedback == null)
                throw new ArgumentNullException("userFeedback");

            AssertUserFeedback(userFeedback);

            _userFeedbackRepo.SaveUserFeedback(context, userFeedback);
        }

        #endregion
    }
}
