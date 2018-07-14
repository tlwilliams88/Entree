﻿using Entree.Core.Interface.Configurations;
using Entree.Core.Models.Configuration.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Configurations
{
	public class ExternalCatalogRepositoryImpl: EFBaseRepository<ExternalCatalog>, IExternalCatalogRepository
	{
        public ExternalCatalogRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }
	}
}
