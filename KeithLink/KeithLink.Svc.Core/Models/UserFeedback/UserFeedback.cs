using KeithLink.Svc.Core.Enumerations.Messaging;

using System;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.UserFeedback
{
    [DataContract(Name = "userfeedback")]
    [Serializable]
    public class UserFeedback
    {
        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "audience")]
        public Audience Audience { get; set; }

        [DataMember(Name = "browserUserAgent")]
        public string BrowserUserAgent { get; set; }

        [DataMember(Name = "browserVendor")]
        public string BrowserVendor { get; set; }

    }
}
