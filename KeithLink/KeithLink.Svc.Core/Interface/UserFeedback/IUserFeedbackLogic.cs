using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.UserFeedback;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.UserFeedback
{
    public interface IUserFeedbackLogic
    {
        int SubmitUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback);
    }
}
