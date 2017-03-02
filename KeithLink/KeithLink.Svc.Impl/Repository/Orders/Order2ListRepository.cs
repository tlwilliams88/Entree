using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class Order2ListRepository : IOrder2ListRepository
    {
        public Order2List Read(string controlNumber)
        {
            Order2List o2l = new Order2List();
            o2l.ControlNumber = controlNumber;
            o2l.ListId = GetListId("[Orders].[ReadOrderListAssociation]", new SqlParameter("@ControlNumber", controlNumber));

            return o2l;
        }

        private long? GetListId(string procedure, SqlParameter parameter)
        {
            using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
            {
                using (var cmd = new SqlCommand(procedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(parameter);

                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var ret = cmd.ExecuteScalar();

                    if(ret != null)
                    {
                        return long.Parse(ret.ToString());
                    }
                }
            }
            return null;
        }

        public void Write(Order2List o2l)
        {
            if(o2l.ListId != null)
            {
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
                {
                    using (var cmd = new SqlCommand("[Orders].[WriteOrderListAssociation]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@ControlNumber", o2l.ControlNumber));
                        cmd.Parameters.Add(new SqlParameter("@ListId", o2l.ListId.Value));

                        cmd.CommandTimeout = 0;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Purge(int PurgeDays)
        {
            if (PurgeDays < 0)
            {
                using (var conn = new SqlConnection(Configuration.BEKDBConnectionString))
                {
                    using (var cmd = new SqlCommand("[Orders].[PurgeOrderListAssociation]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@PurgeDays", PurgeDays));

                        cmd.CommandTimeout = 0;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
