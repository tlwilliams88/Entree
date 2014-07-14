using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.ETL
{
    public interface ICategoryLogic
    {
        void ProcessStagedCategories();
        void ProcessStagedItems();
        void ProcessStagedBranches();
    }
}
