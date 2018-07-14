﻿// KeithLink
using Entree.Core.Interface.Profile;
using Entree.Core.Models.Customers.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class ItemHistoryRepositoryImpl : EFBaseRepository<ItemHistory>, IItemHistoryRepository {
        public ItemHistoryRepositoryImpl( IUnitOfWork unitOfWork ) : base( unitOfWork ) { }
    }
}
