using KeithLink.Common.Core.Extensions;

using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Profiles;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents;
using CommerceServer.Foundation.SequenceComponents.ContextProviders;

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class OrganizationAddressesLoader : RelatedProfileProcessorBase {
        #region attributes
        static object _addressLoaderLock;
        #endregion

        #region ctor
        public OrganizationAddressesLoader() {
            _addressLoaderLock = new object();
        }
        #endregion

        #region methods
        public override void ExecuteQuery(CommerceQueryOperation queryOperation, OperationCacheDictionary operationCache, CommerceQueryOperationResponse response) {
            int numToTake = 2000;
            lock (_addressLoaderLock) {
                for (int i = 0; i < response.CommerceEntities.Count && response.CommerceEntities.Count < numToTake; i += numToTake) { // only load addresses if we have less than 2000; any more than that will require a incoming filter 
                    List<string> preferredAddressIds = new List<string>();
                    Dictionary<string, string> addressToParentIdMap = new Dictionary<string, string>();
                    List<CommerceEntity> currentBatch = response.CommerceEntities.Skip(i).Take(numToTake).ToList();

                    foreach (var entity in currentBatch) {
                        string preferredAddressId = entity.GetPropertyValue("GeneralInfo.preferred_address") as string;
                        if (!String.IsNullOrEmpty(preferredAddressId) && !addressToParentIdMap.ContainsKey(preferredAddressId)) {
                            preferredAddressIds.Add("'" + preferredAddressId + "'");
                            addressToParentIdMap.Add(preferredAddressId, entity.Id);
                        }
                    }
                    // query them out
                    if (preferredAddressIds.Count == 0)
                        continue;

                    CommerceResourceCollection csResources = SiteHelper.GetCsConfig();

                    String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                    //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
                    string fields = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), p => p.Value));
                    string keys = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), p => p.Key));
                    ProfileContext ctxt = CommerceSiteContexts.Profile[SiteHelper.GetSiteName()];

                    string dataSQLText = "SELECT *  FROM [BEK_Commerce_profiles].[dbo].[Addresses] WHERE u_address_id in (" + String.Join(",", preferredAddressIds.ToArray()) + ")";

                    try {

                        using (OleDbConnection conn = new OleDbConnection(connStr)) {
                            conn.Open();

                            using (OleDbCommand cmdRead = new OleDbCommand(dataSQLText, conn)) {
                                using (OleDbDataReader dataReader = cmdRead.ExecuteReader()) {
                                    while (dataReader.Read()) {
                                        CommerceEntity entity = new CommerceEntity("Address");

                                        entity.Id = dataReader.GetString("u_address_id");
                                        entity.SetPropertyValue("LastName", dataReader.GetString("u_last_name"));
                                        entity.SetPropertyValue("FirstName", dataReader.GetString("u_first_name"));
                                        entity.SetPropertyValue("AddressName", dataReader.GetString("u_address_name"));
                                        entity.SetPropertyValue("AddressType", dataReader.GetNullableInt("i_address_type"));
                                        entity.SetPropertyValue("Description", dataReader.GetString("u_description"));
                                        entity.SetPropertyValue("Line1", dataReader.GetString("u_address_line1"));
                                        entity.SetPropertyValue("Line2", dataReader.GetString("u_address_line2"));
                                        entity.SetPropertyValue("City", dataReader.GetString("u_city"));
                                        entity.SetPropertyValue("StateProvinceCode", dataReader.GetString("u_region_code"));
                                        entity.SetPropertyValue("StateProvinceName", dataReader.GetString("u_region_name"));
                                        entity.SetPropertyValue("ZipPostalCode", dataReader.GetString("u_postal_code"));
                                        entity.SetPropertyValue("CountryRegionCode", dataReader.GetString("u_country_code"));
                                        entity.SetPropertyValue("CountryRegionName", dataReader.GetString("u_country_name"));
                                        entity.SetPropertyValue("Telephone", dataReader.GetString("u_tel_number"));
                                        entity.SetPropertyValue("TelephoneExtension", dataReader.GetString("u_tel_extension"));
                                        entity.SetPropertyValue("LocaleId", dataReader.GetString("i_locale"));
                                        entity.SetPropertyValue("DateModified", dataReader.GetNullableDateTime("dt_date_last_changed"));
                                        entity.SetPropertyValue("DateCreated", dataReader.GetNullableDateTime("dt_date_created"));

                                        currentBatch.Where(x => x.Id == (addressToParentIdMap[entity.Id])).FirstOrDefault()
                                            .Properties.Add("PreferredAddress", new CommerceRelationship(entity));
                                    }
                                    dataReader.Close();
                                }
                            }
                        }

                    } catch (Exception ex) {
                        throw new ApplicationException("Error loading organization addresses", ex);
                    }

                }
            }
        }
        #endregion

        #region properties
        protected override bool CanDeleteRelatedItem(string relatedItemId) {
            return true;
        }

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
        #endregion
    }
}