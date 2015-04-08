using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Utility.NotificationParser.Models {
    public class EtaNotification {
        [Key]
        public int NotificationId { get; set; }

        public DateTime? ActualTime { get; set; }

        [Column(TypeName="varchar"), MaxLength(3)]
        public string Branch { get; set; }

        public DateTime? EstimatedTime { get; set; }

        [Column(TypeName = "varchar"), MaxLength(8)]
        public string OrderId { get; set; }

        public bool OutOfSequence { get; set; }

        [Column(TypeName = "varchar"), MaxLength(5)]
        public string RouteId { get; set; }

        public DateTime? ScheduledTime { get; set; }

        [Column(TypeName = "varchar"), MaxLength(3)]
        public string StopNumber { get; set; }
    }
}
