
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.UserFeedback;

namespace KeithLink.Svc.Core.Interface.UserFeedback
{
    public interface IUserFeedbackRepository
    {
        int SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback);

    }
}
