using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace KeithLink.Svc.Core.Interface.DataConnection
{
     public interface IDapperDatabaseConnection
    {
        void ExecuteCommand(CommandDefinition cmd);
        void ExecuteSPCommand(string sql);
        void ExecuteSPCommand(string sql, string parmName, object value);
        void ExecuteSPCommand(string sql, DynamicParameters parms);
        T ExecuteScalarCommand<T>(CommandDefinition cmd);
        T ExecuteScalarSPCommand<T>(string sql);
        T ExecuteScalarSPCommand<T>(string sql, string parmName, object value);
        T ExecuteScalarSPCommand<T>(string sql, DynamicParameters parms);
        List<T> Read<T>(CommandDefinition cmd);
        List<T> ReadSP<T>(string sql);
        List<T> ReadSP<T>(string sql, string parmName, object value);
        List<T> ReadSP<T>(string sql, DynamicParameters parms);
        T ReadOne<T>(CommandDefinition cmd);
        T ReadOneSP<T>(string sql);
        T ReadOneSP<T>(string sql, string parmName, object value);
        T ReadOneSP<T>(string sql, DynamicParameters parms);
        IEnumerable<T> Query<T>(string sql, object parms = null);
        IEnumerable<dynamic> Query(string sql, object parms = null);
        T ReadOne<T>(string sql, object parms = null);
        void Execute(string sql, object parms = null);
    }
}
