using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationAddressesLoader : CommerceServer.Foundation.SequenceComponents.RelatedProfileProcessorBase
    {
        protected override string CommercePreferredIdKeyName
        {
            get
            {
                return "GeneralInfo.preferred_address";
            }
        }

        protected override string CommerceRelatedIdsKeyName
        {
            get
            {
                return "GeneralInfo.address_list";
            }
        }

        protected override string ParentProfileModelName
        {
            get
            {
                return "Organization";
            }
        }

        protected override string PreferredItemRelationshipName
        {
            get
            {
                return "PreferredAddress";
            }
        }

        protected override string ProfileModelName
        {
            get
            {
                return "Address";
            }
        }

        protected override string RelationshipName
        {
            get
            {
                return "Addresses";
            }
        }

        public OrganizationAddressesLoader()
        {
        }

        public override void ExecuteQuery(CommerceServer.Foundation.CommerceQueryOperation queryOperation, CommerceServer.Foundation.OperationCacheDictionary operationCache, CommerceServer.Foundation.CommerceQueryOperationResponse response)
        {
            List<string> preferredAddressIds = new List<string>();
            foreach (var entity in response.CommerceEntities)
            {
                string preferredAddressId = entity.GetPropertyValue("GeneralInfo.preferred_address") as string;
                if (!String.IsNullOrEmpty(preferredAddressId))
                    preferredAddressIds.Add(preferredAddressId);
            }
            // query them out
            CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources = SiteHelper.GetCsConfig();

            String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
            //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
            string fields = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), i => i.Value));
            CommerceServer.Core.Runtime.Profiles.ProfileContext ctxt = CommerceServer.Foundation.SequenceComponents.ContextProviders.CommerceSiteContexts.Profile[SiteHelper.GetSiteName()];
            string cmdText = "SELECT " + fields + " FROM Address WHERE address_id in " + "(" + String.Join(",", preferredAddressIds.ToArray()) + ")"; // todo: map out specific fields or check if field names are in ProfileEntityMappings

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
                    CommerceServer.Foundation.CommerceEntity entity = new CommerceServer.Foundation.CommerceEntity("Address");

                    entity.Id = rs.Fields["GeneralInfo.address_id"].Value.ToString();
                    foreach (var prop in this.ProfileEntityMappings.PropertyMappings)
                    {
                        entity.Properties[prop.Key] = rs.Fields[prop.Value].Value;
                    }
                    response.CommerceEntities.Add(entity);
                    // Move to the next record.
                    rs.MoveNext();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error loading organization addresses", ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
        }

        protected override bool CanDeleteRelatedItem(string relatedItemId)
        {
            return true;
        }
    }
}