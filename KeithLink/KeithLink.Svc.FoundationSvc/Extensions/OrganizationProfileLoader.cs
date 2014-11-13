using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationProfileLoader : ProfileLoaderBase
    {
       protected override string ProfileModelName
        {
            get
            {
                return "Organization";
            }
        }

        public OrganizationProfileLoader()
        {
        }

        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response)
        {
            CommerceModelSearch search = ((CommerceServer.Foundation.CommerceModelSearch)(queryOperation.SearchCriteria));
            if (search.Model.Properties.Count == 1 && search.Model.Properties[0].Key == "OrganizationType")
            { // no search criteria, so override CS behavior to load all orgs
                CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources = SiteHelper.GetCsConfig();

                String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
                string fields = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), i => i.Value));
                CommerceServer.Core.Runtime.Profiles.ProfileContext ctxt = CommerceServer.Foundation.SequenceComponents.ContextProviders.CommerceSiteContexts.Profile[SiteHelper.GetSiteName()];
                string cmdText = "SELECT " + fields + " FROM Organization WHERE " + this.ProfileEntityMappings.PropertyMappings["OrganizationType"] + " = '" + search.Model.Properties[0].Value + "'"; // todo: map out specific fields or check if field names are in ProfileEntityMappings

                // Create a new RecordsetClass object.
                ADODB.Recordset rs = new ADODB.Recordset();

                try
                {
                    // Open a RecordsetClass instance by executing the SQL statement to the CSOLEDB provider.
                    rs.Open(
                    cmdText,
                    ctxt.CommerceOleDbProvider,
                    ADODB.CursorTypeEnum.adOpenForwardOnly,
                    ADODB.LockTypeEnum.adLockReadOnly,
                    (int)ADODB.CommandTypeEnum.adCmdText);

                    // Iterate through the records.
                    while (!rs.EOF)
                    {
                        // Write out the user_id value.
                        CommerceEntity entity = new CommerceEntity("Organization");
                    
                        entity.Id = rs.Fields["GeneralInfo.org_id"].Value.ToString();
                        foreach (var prop in this.ProfileEntityMappings.PropertyMappings)
                        {
                            entity.Properties[prop.Key] = rs.Fields[prop.Value].Value;
                        }
                        response.CommerceEntities.Add(entity);
                        // Move to the next record.
                        rs.MoveNext();
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                }
            }
            else
            {
                base.ExecuteQuery(queryOperation, operationCache, response);
            }
        }
    }
}