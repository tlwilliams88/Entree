using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ApplicationHealth
{
    public interface IQueueHealthCheck
    {
        void CheckQueueProperties();
    }
}
