using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public class UserContextEqualityComparer : IEqualityComparer<UserSelectedContext>
    {
        public bool Equals(UserSelectedContext x, UserSelectedContext y)
        {
            return (x.CustomerId + "|" + x.BranchId == y.CustomerId + "|" + y.BranchId);
        }

        public int GetHashCode(UserSelectedContext obj)
        {
            return ((obj.CustomerId.GetHashCode() * 397) ^ (obj.BranchId.GetHashCode()));
        }
    }
}
