﻿using Entree.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.EF;
using System.Data.Entity;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ListRepositoryImpl : EFBaseRepository<List>, IListRepository
	{
		public ListRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
        

		public IQueryable<List> ReadListForCustomer(Core.Models.SiteCatalog.UserSelectedContext catalogInfo, bool headerOnly)
		{
			if(headerOnly)
				return this.Entities.Include(t => t.Shares).Where(l => 
					(l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)) || 
					(l.Shares.Any(s => s.CustomerId.Equals(catalogInfo.CustomerId) && s.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase))));
			else
				return this.Entities.Include(i => i.Items).Include(t => t.Shares).Where(l =>  
					(l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)) ||
					(l.Shares.Any(s => s.CustomerId.Equals(catalogInfo.CustomerId) && s.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase))));
		}
        
		
		
	}
}
