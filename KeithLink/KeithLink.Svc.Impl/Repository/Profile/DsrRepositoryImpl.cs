using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class DsrRepositoryImpl : EFBaseRepository<Dsr>, IDsrRepository {

        public DsrRepositoryImpl( IUnitOfWork unitOfWork ) : base( unitOfWork ) { }

        public Dsr GetDsrByBranchAndDsrNumber( string branchId, string dsrNumber ) {
            if (branchId == null)
                throw new Exception( "Branch cannot be null" );
            if (dsrNumber == null)
                throw new Exception( "DsrNumber cannot be null" );

            return this.Entities.Where( d => d.BranchId == branchId && d.DsrNumber == dsrNumber ).FirstOrDefault();
        }

    }
}
