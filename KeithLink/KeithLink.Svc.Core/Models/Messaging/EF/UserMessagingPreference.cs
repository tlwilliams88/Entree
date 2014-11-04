using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.EF;

namespace KeithLink.Svc.Core.Models.Messaging.EF
{
    public class UserMessagingPreference : BaseEFModel
    {
        public Guid UserId { get; set; }
        MessageType MessageType { get; set; }
        NotificationType NotificationType { get; set; }
        bool Enabled { get; set; }

        [MaxLength(9)]
        [Column(TypeName = "varchar")]
        public string CustomerNumber { get; set; }
    }
}
