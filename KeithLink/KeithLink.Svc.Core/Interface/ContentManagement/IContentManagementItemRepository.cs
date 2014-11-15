using KeithLink.Svc.Core.Models.ContentManagement.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ContentManagement
{
	public interface IContentManagementItemRepository : IBaseEFREpository<ContentItem>
	{
	}
}
