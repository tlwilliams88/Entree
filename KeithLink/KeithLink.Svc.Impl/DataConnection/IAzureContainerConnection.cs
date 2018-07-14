using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Entree.Core.Models.Documents;

namespace Entree.Core.Interface.DataConnection
{
     public interface IAzureContainerConnection
    {
        List<DocumentReturnModel> GetDocuments(string containerName, string directoryName, string uriName = "");
    }
}
