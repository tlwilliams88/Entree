using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using KeithLink.Svc.Core.Models.Documents;

namespace KeithLink.Svc.Core.Interface.DataConnection
{
     public interface IAzureContainerConnection
    {
        List<DocumentReturnModel> GetDocuments(string uriName = "");
    }
}
