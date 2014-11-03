using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.EF
{
    public class UserTopicSubscription : BaseEFModel
    {
        public Guid UserId { get; set; }

        public NotificationType NotificationType { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "varchar")]
        public string NotificationEndpoint { get; set; }
    }
}
