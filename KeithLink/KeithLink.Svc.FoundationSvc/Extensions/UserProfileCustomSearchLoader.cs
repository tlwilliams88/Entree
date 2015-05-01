using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core.Extensions;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation.SequenceComponents.Utility;
using CommerceServer.Foundation.SequenceComponents.ContextProviders;
using CommerceServer.Foundation.SequenceComponents.CSHelpers;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
	public class UserProfileCustomSearchLoader : ProfileLoaderBase
	{
		protected override string ProfileModelName
		{
			get { return "UserProfileCustomSearch"; }
		}

		public override void ExecuteQuery(CommerceServer.Foundation.CommerceQueryOperation queryOperation, CommerceServer.Foundation.OperationCacheDictionary operationCache, CommerceServer.Foundation.CommerceQueryOperationResponse response)
		{
			CommerceModelSearch search = queryOperation.GetSearchCriteria<CommerceModelSearch>();
			ParameterChecker.CheckForNull(search, "searchCriteria");

			if (!String.IsNullOrEmpty(search.WhereClause))
			{
				var sqlFormat = " SELECT u_user_id, u_first_name, u_last_name, u_email_address FROM [BEK_Commerce_profiles].[dbo].[UserObject] {0}";
				var sql = string.Format(sqlFormat, search.WhereClause.IndexOf("where", StringComparison.CurrentCultureIgnoreCase) >= 0 ? search.WhereClause : "WHERE " + search.WhereClause);

				CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources = SiteHelper.GetCsConfig();
				String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
				
				using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr))
				{
					conn.Open();
					using (System.Data.OleDb.OleDbCommand cmdRead = new System.Data.OleDb.OleDbCommand(sql, conn))
					{
						using (System.Data.OleDb.OleDbDataReader dataReader = cmdRead.ExecuteReader())
						{
							while (dataReader.Read())
							{

								CommerceEntity org = new CommerceEntity("UserProfile");
								org.Id = dataReader.GetString("u_user_id");
								org.SetPropertyValue("FirstName", dataReader.GetString("u_first_name"));
								org.SetPropertyValue("LastName", dataReader.GetString("u_last_name"));
								org.SetPropertyValue("Email", dataReader.GetString("u_email_address"));
								response.CommerceEntities.Add(org);
							}
						}
					}					
				}
			}
			else
			{
				base.ExecuteQuery(queryOperation, operationCache, response);
			}
		}
	}
}