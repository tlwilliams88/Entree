using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Order
{
	public enum LineType
	{
		NoChange,
		Change,
		Add,
		Delete
	}

	public enum OrderQueueLocation
	{
		Normal,
		History,
		Error,
		Reprocess
	}

	public enum OrderSource
	{
		Entree,
        DSR,
        CustomerService,
        KeithNet,
        Other
	}

	public enum OrderType
	{
		NormalOrder,
		ChangeOrder,
		DeleteOrder
	}

	public enum UnitOfMeasure
	{
		Case,
		Package
	}
}
