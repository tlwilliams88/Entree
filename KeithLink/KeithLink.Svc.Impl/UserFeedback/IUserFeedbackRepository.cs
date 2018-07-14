
using Entree.Core.Models.UserFeedback;

namespace Entree.Core.Interface.UserFeedback
{
    public interface IUserFeedbackRepository
    {
        void SaveUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback);

    }
}
