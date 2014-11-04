using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeithLink.Svc.Core.Models.EF;

namespace KeithLink.Svc.Core.Models.Messaging
{
    public class CustomerTopic : BaseEFModel
    {
        [MaxLength(55)]
        [Column(TypeName="varchar")]
        public string CustomerId { get; set; }

        [MaxLength(25)]
        [Column(TypeName = "varchar")]
        public string MessageType { get; set; }

        [MaxLength(255)]
        [Column(TypeName="varchar")]
        public string ProviderTopicId { get; set; }
    }
}
