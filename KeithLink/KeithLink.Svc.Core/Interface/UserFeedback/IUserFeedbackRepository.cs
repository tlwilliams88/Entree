
using KeithLink.Svc.Core.Models.UserFeedback;

namespace KeithLink.Svc.Core.Interface.UserFeedback
{
    public interface IUserFeedbackRepository
    {
        void SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback);

    }
}
