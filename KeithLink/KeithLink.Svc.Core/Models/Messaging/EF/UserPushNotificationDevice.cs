using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging.EF
{
    public class UserPushNotificationDevice : BaseEFModel
    { // might benefit from a three part index on userid, device id and device os to enforce uniqueness
        [Index("IX_UserId")]
        public Guid UserId { get; set; }

        [MaxLength(255)]
        [Required]
        public string DeviceId { get; set; }

        [MaxLength(255)]
        [Required]
        public string ProviderToken { get; set; } // token from apns or gcm

        [MaxLength(255)]
        public string ProviderEndpointId { get; set; } // ARN from amazon to send messages to; may be blank initially

        [Required]
        public DeviceOS DeviceOS { get; set; }
    }
}
