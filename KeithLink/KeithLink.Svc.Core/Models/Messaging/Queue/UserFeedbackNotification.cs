using KeithLink.Svc.Core.Enumerations.Messaging;

using System;
using System.Text;
using System.Runtime.Serialization;

using KeithLink.Svc.Core.Models.UserFeedback;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    [DataContract(Name = "userfeedbacknotification")]
    public class UserFeedbackNotification : BaseNotification
    {
        public UserFeedbackNotification()
        {
            this.NotificationType = NotificationType.UserFeedback;
        }

        [DataMember(Name = "context")]
        public UserFeedbackContext Context { get; set; }

        [DataMember(Name = "userFeedback")]
        public UserFeedback.UserFeedback UserFeedback { get; set; }

    }
}
