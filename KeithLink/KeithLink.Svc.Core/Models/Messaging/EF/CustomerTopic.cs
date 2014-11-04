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
    public class CustomerTopic : BaseEFModel
    {
        [MaxLength(9)]
        [Column(TypeName="varchar")]
        public string CustomerNumber { get; set; }

        [MaxLength(255)]
        [Column(TypeName="varchar")]
        public string ProviderTopicId { get; set; }

        public virtual ICollection<UserTopicSubscription> Subscriptions { get; set; }
        public MessageType MessageType { get; set; }
    }
}
