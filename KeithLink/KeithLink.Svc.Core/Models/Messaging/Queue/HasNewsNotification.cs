using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
	[DataContract]
	public class HasNewsNotification: BaseNotification
	{
		public HasNewsNotification()
		{
			this.NotificationType = Enumerations.Messaging.NotificationType.HasNews;
		}

		[DataMember(Name = "notification")]
		public string Notification { get; set; }

		[DataMember(Name = "subject")]
		public string Subject { get; set; }

		
	}
}
