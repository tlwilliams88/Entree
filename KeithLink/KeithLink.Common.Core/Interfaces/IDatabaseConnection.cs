using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Interfaces {
    public interface IDatabaseConnection {
        SqlDataReader GetDataReader(SqlCommand cmd);

        SqlDataReader GetDataReader(string sql);

        SqlDataReader GetDataReader(string sql, string parmName, object value);

        void ExecuteCommand(SqlCommand cmd);

        void ExecuteCommand(string sql);

        void ExecuteCommand(string sql, string parmName, object value);
    }
}
