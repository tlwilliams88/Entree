using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web;

namespace KeithLink.Svc.InternalSvc
{
	public class ErrorHandlerBehavior: BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(ErrorServiceBehavior); }
		}

		protected override object CreateBehavior()
		{
			return new ErrorServiceBehavior();
		}
	}
}