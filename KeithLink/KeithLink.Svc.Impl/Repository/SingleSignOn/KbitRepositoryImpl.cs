using KeithLink.Svc.Core.Interface.SingleSignOn;

using System;
using System.Data.SqlClient;

namespace KeithLink.Svc.Impl.Repository.SingleSignOn {
    public class KbitRepositoryImpl : IKbitRepository, IDisposable {
        #region attributes
        private SqlConnection _con;
        private bool _disposed;
        #endregion

        #region ctor
        public KbitRepositoryImpl() {
            _con = new SqlConnection(Configuration.KbitConnectionString);
            _disposed = false;
        }
        #endregion

        #region methods
        public void AddCustomerToUser(string userName, Core.Models.SiteCatalog.UserSelectedContext customer) {
            SqlCommand cmd = new SqlCommand("usp_Entree_Add_kbit_UserId_Customer", _con);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", userName);
            cmd.Parameters.AddWithValue("@Division", customer.BranchId);
            cmd.Parameters.AddWithValue("@CustomerNbr", customer.CustomerId);

            _con.Open();

            cmd.ExecuteNonQuery();

            _con.Close();
        }

        public void Dispose() {
            if (!_disposed) {
                if (_con.State == System.Data.ConnectionState.Open) {
                    _con.Close();
                }

                _con.Dispose();
            }
        }

        public void DeleteAllCustomersForUser(string userName) {
            SqlCommand cmd = new SqlCommand("usp_Entree_Delete_kbit_UserId", _con);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserId", userName);

            _con.Open();

            cmd.ExecuteNonQuery();

            _con.Close();
        }
        #endregion
    }
}
