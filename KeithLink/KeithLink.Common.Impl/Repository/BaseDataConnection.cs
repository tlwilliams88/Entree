using KeithLink.Common.Core.Interfaces;

using System;
using System.Data;
using System.Data.SqlClient;

namespace KeithLink.Common.Impl.Repository {
    public abstract class BaseDataConnection : IDisposable, IDatabaseConnection {
        #region attributes
        private SqlConnection   _connection;
        private string          _connectionString;
        private bool            _disposedValue;
        private bool            _extendedTimeOut;
        private bool            _pool;
        #endregion

        #region ctor
        public BaseDataConnection() {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BEKDBContext"].ConnectionString;

            _connection = new SqlConnection(_connectionString);
            _disposedValue = false;
            _extendedTimeOut = false;
            _pool = false;
        }
        #endregion

        #region methods
        public void Dispose(bool disposing) {
            if(_disposedValue == false){
                if(disposing) {
                    if(_connection != null) { _connection.Dispose(); }

                    _connectionString = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ExecuteCommand(SqlCommand cmd) {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Connection;

            if(this.Connection.State == ConnectionState.Closed) { this.Connection.Open(); }

            cmd.ExecuteNonQuery();

            if(!_pool) {
                this.Connection.Close();
                //this.Connection.Dispose();
            }

        }

        public void ExecuteCommand(string sql) {
            SqlCommand cmd = new SqlCommand(sql);

            ExecuteCommand(cmd);
        }

        public void ExecuteCommand(string sql, string parmName, object value) {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue(parmName, value);

            ExecuteCommand(cmd);
        }

        public SqlDataReader GetDataReader(SqlCommand cmd) {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Connection;

            if(this.Connection.State == ConnectionState.Closed) { this.Connection.Open(); }

            if(_pool) {
                return cmd.ExecuteReader(); 
            } else {
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public SqlDataReader GetDataReader(string sql) {
            SqlCommand cmd = new SqlCommand(sql);

            return GetDataReader(cmd);
        }

        public SqlDataReader GetDataReader(string sql, string parmName, object value) {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue(parmName, value);

            return GetDataReader(cmd);
        }
        #endregion

        #region properties
        public SqlConnection Connection {
            get {
                return _connection;
            }
            set {
                _connection = value;
            }
        }

        public bool PoolConnection {
            get {
                return _pool;
            }
            set {
                _pool = value;
            }
        }
        #endregion
    }
}
