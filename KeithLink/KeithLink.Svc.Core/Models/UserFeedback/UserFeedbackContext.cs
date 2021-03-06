﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.UserFeedback
{
    [DataContract]
    [Serializable]
    public class UserFeedbackContext
    {
        [DataMember(Name = "userId")]
        public Guid UserId { get; set; }

        [DataMember(Name = "userFirstName")]
        public string UserFirstName { get; set; }

        [DataMember(Name = "userLastName")]
        public string UserLastName { get; set; }

        [DataMember(Name = "branchid")]
        public string BranchId { get; set; }

        [DataMember(Name = "customerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "customerName")]
        public string CustomerName { get; set; }

        [DataMember(Name = "salesRepName")]
        public string SalesRepName { get; set; }

        [DataMember(Name = "sourceName")]
        public string SourceName { get; set; }

        [DataMember(Name = "targetName")]
        public string TargetName { get; set; }

        [DataMember(Name = "sourceEmailAddress")]
        public string SourceEmailAddress { get; set; }

        [DataMember(Name = "targetEmailAddress")]
        public string TargetEmailAddress { get; set; }

    }
}
