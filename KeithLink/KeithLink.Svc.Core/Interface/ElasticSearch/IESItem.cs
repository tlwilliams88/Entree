using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.ElasticSearch
{
    /// <summary>
    /// Interface for the generic elasticsearch item; we are using polymorphism to load through batch operations
    /// </summary>
    public interface IESItem
    {
        string ToJson();
    }
}
