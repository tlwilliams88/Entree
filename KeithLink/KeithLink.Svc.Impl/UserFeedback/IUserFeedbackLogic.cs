using Entree.Core.Enumerations.Messaging;
using Entree.Core.Models.Profile;
using Entree.Core.Models.UserFeedback;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.UserFeedback
{
    public interface IUserFeedbackLogic
    {
        void SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback);
    }
}
