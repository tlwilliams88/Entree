using Dapper;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.DataConnection
{
    public class DapperDatabaseConnection
    {
        #region ctor
        public DapperDatabaseConnection(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }
        #endregion

        #region methods
        ///// <summary>
        ///// execute a sql command that does not return any results
        ///// </summary>
        ///// <param name="cmd">the sql command</param>
        ///// <remarks>
        ///// jwames - 6/27/2015 - original code
        ///// </remarks>
        public void ExecuteCommand(CommandDefinition cmd)
        {
            Connection.Open();

            Connection.Execute(cmd);

            Connection.Close();
        }

        /// <summary>
        /// execute a stored procedure by name
        /// </summary>
        /// <param name="sql">the procedure name</param>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public void ExecuteSPCommand(string sql)
        {
            ExecuteCommand(GetSPCommandDefinition(sql));
        }

        /// <summary>
        /// execute a stored procedure by name that uses a single parameter
        /// </summary>
        /// <param name="sql">the procedure name</param>
        /// <param name="parmName">the parameter name</param>
        /// <param name="value">the value of the paramter</param>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public void ExecuteSPCommand(string sql, string parmName, object value)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(parmName, value);

            ExecuteCommand(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// execute a stored procedure by name with parameters
        /// </summary>
        /// <param name="sql">stored procedure name</param>
        /// <param name="parms">parameters</param>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public void ExecuteSPCommand(string sql, DynamicParameters parms)
        {
            ExecuteCommand(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// execute a sql command that returns a single value
        /// </summary>
        /// <param name="cmd">the sql command</param>
        /// <remarks>
        /// jwames - 7/18/2015 - original code
        /// </remarks>
        public T ExecuteScalarCommand<T>(CommandDefinition cmd)
        {
            Connection.Open();

            var retVal = Connection.ExecuteScalar<T>(cmd);

            Connection.Close();

            return retVal;
        }

        /// <summary>
        /// execute a stored procedure by name
        /// </summary>
        /// <param name="sql">the procedure name</param>
        /// <remarks>
        /// jwames - 7/18/2015 - original code
        /// </remarks>
        public T ExecuteScalarSPCommand<T>(string sql)
        {
            return ExecuteScalarCommand<T>(GetSPCommandDefinition(sql));
        }

        /// <summary>
        /// execute a stored procedure by name that uses a single parameter
        /// </summary>
        /// <param name="sql">the procedure name</param>
        /// <param name="parmName">the parameter name</param>
        /// <param name="value">the value of the paramter</param>
        /// <remarks>
        /// jwames - 7/18/2015 - original code
        /// </remarks>
        public T ExecuteScalarSPCommand<T>(string sql, string parmName, object value)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(parmName, value);

            return ExecuteScalarCommand<T>(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// execute a stored procedure by name with parameters
        /// </summary>
        /// <param name="sql">stored procedure name</param>
        /// <param name="parms">parameters</param>
        /// <remarks>
        /// jwames - 7/18/2015 - original code
        /// </remarks>
        public T ExecuteScalarSPCommand<T>(string sql, DynamicParameters parms)
        {
            return ExecuteScalarCommand<T>(GetSPCommandDefinition(sql, parms));
        }

        private CommandDefinition GetSPCommandDefinition(string sql)
        {
            return new CommandDefinition(sql, commandType: CommandType.StoredProcedure, flags: CommandFlags.None);
        }

        private CommandDefinition GetSPCommandDefinition(string sql, DynamicParameters parms)
        {
            return new CommandDefinition(sql, parms, commandType: CommandType.StoredProcedure, flags: CommandFlags.None);
        }

        /// <summary>
        /// get the query results for the sql command
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="cmd">the command with stored procedure and parameters already set</param>
        /// <returns>list of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public List<T> Read<T>(CommandDefinition cmd)
        {
            Connection.Open();

            IEnumerable<T> results = Connection.Query<T>(cmd);

            List<T> retVal = results.ToList();

            Connection.Close();

            return retVal;
        }

        /// <summary>
        /// get the query results for the specified stored procedure
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">the procedure name</param>
        /// <returns>list of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public List<T> ReadSP<T>(string sql)
        {
            return Read<T>(GetSPCommandDefinition(sql));
        }

        /// <summary>
        /// get the query results for the specified stored procedure and single parameter
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">the procedure name</param>
        /// <param name="parmName">the parameter name</param>
        /// <param name="value">the parameter value</param>
        /// <returns>list of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public List<T> ReadSP<T>(string sql, string parmName, object value)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(parmName, value);

            return Read<T>(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// get the query results for the specified stored procedure with parameters
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">stored procedure name</param>
        /// <param name="parms">parameters</param>
        /// <returns>list of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public List<T> ReadSP<T>(string sql, DynamicParameters parms)
        {
            return Read<T>(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// get the query results for the sql command
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="cmd">the command with stored procedure and parameters already set</param>
        /// <returns>a single instance of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public T ReadOne<T>(CommandDefinition cmd)
        {
            Connection.Open();

            IEnumerable<T> result = Connection.Query<T>(cmd);

            T retVal = result.FirstOrDefault();

            Connection.Close();

            return retVal;
        }

        /// <summary>
        /// get the query results for the specified stored procedure
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">the procedure name</param>
        /// <returns>single instance of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public T ReadOneSP<T>(string sql)
        {
            return ReadOne<T>(GetSPCommandDefinition(sql));
        }

        /// <summary>
        /// get the query results for the specified stored procedure and single parameter
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">the procedure name</param>
        /// <param name="parmName">the parameter name</param>
        /// <param name="value">the parameter value</param>
        /// <returns>single instane of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public T ReadOneSP<T>(string sql, string parmName, object value)
        {
            DynamicParameters parms = new DynamicParameters();
            parms.Add(parmName, value);

            return ReadOne<T>(GetSPCommandDefinition(sql, parms));
        }

        /// <summary>
        /// get the query results for the specified stored procedure with multiple parameters)
        /// </summary>
        /// <typeparam name="T">data model</typeparam>
        /// <param name="sql">stored procedure name</param>
        /// <param name="parms">dynamic parameters</param>
        /// <returns>single instance of type</returns>
        /// <remarks>
        /// jwames - 6/27/2015 - original code
        /// </remarks>
        public T ReadOneSP<T>(string sql, DynamicParameters parms)
        {
            return ReadOne<T>(GetSPCommandDefinition(sql, parms));
        }
        #endregion

        public IEnumerable<T> QueryInline<T>(string sql, object parms = null)
        {
             return Connection.Query<T>(sql, parms);
        }

        public T ReadOneInline<T>(string sql, object parms = null)
        {
            return Connection.QuerySingle(sql, parms);
        }
        public void ExecuteInline(string sql, object parms = null)
        {
            Connection.Execute(sql, parms);
        }

        #region properties
        public SqlConnection Connection { get; set; }
        #endregion
    }
}