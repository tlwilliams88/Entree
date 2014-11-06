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
    public class UserMessage : BaseEFModel
    {
        [MaxLength(9)]
        [Column(TypeName = "varchar")]
        public string CustomerNumber { get; set; }

        [MaxLength(55)]
        [Column(TypeName = "varchar")]
        public string UserId { get; set; }

        public NotificationType NotificationType { get; set; }
        public DateTime? MessageReadUtc { get; set; }
        public string Body { get; set; }
    }
}
