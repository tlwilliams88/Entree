using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Enumerations;
using KeithLink.Common.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.AuditLog
{
	public class AuditLogRepositoryImpl: IAuditLogRepository
	{
		public void WriteToAuditLog(AuditType type, string actor = null, string information = null)
		{
			try
			{
				using (var conn = new SqlConnection(Configuration.AuditLogConnectionString))
				{
					using (var cmd = new SqlCommand("[dbo].[spCreateAuditLogEntry]", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
						cmd.Parameters.Add("@TypeDescription", SqlDbType.NVarChar).Value = EnumUtils<AuditType>.GetDescription(type);
						cmd.Parameters.Add("@Actor", SqlDbType.NVarChar).Value = actor;
						cmd.Parameters.Add("@Information", SqlDbType.NVarChar).Value = information;

						cmd.CommandTimeout = 0;
						conn.Open();
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex) { } //For now, swallow any exception
		}
	}
}
