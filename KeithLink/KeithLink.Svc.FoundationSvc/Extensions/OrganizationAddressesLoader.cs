using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core.Extensions;

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

        static object addressLoaderLock = new object();
        public override void ExecuteQuery(CommerceServer.Foundation.CommerceQueryOperation queryOperation, CommerceServer.Foundation.OperationCacheDictionary operationCache, CommerceServer.Foundation.CommerceQueryOperationResponse response)
        {
            int numToTake = 2000;
            lock(addressLoaderLock)
            {
                for (int i = 0; i < response.CommerceEntities.Count && response.CommerceEntities.Count < numToTake; i += numToTake )
                { // only load addresses if we have less than 2000; any more than that will require a incoming filter 
                    List<string> preferredAddressIds = new List<string>();
                    Dictionary<string, string> addressToParentIdMap = new Dictionary<string,string>();
                    List<CommerceServer.Foundation.CommerceEntity> currentBatch = response.CommerceEntities.Skip(i).Take(numToTake).ToList();

                    foreach (var entity in currentBatch)
                    {
                        string preferredAddressId = entity.GetPropertyValue("GeneralInfo.preferred_address") as string;
                        if (!String.IsNullOrEmpty(preferredAddressId))
                        {
                            preferredAddressIds.Add("'" + preferredAddressId + "'");
                            addressToParentIdMap.Add(preferredAddressId, entity.Id);
                        }
                    }
                    // query them out
                    if (preferredAddressIds.Count == 0)
                        continue;

                    CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection csResources = SiteHelper.GetCsConfig();

                    String connStr = csResources["Biz Data Service"]["s_BizDataStoreConnectionString"].ToString();
                    //ProfileContext pContext = CommerceSiteContexts.Profile[GetSiteName()];
                    string fields = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), p => p.Value));
					string keys = string.Join(", ", Array.ConvertAll(this.ProfileEntityMappings.PropertyMappings.ToArray(), p => p.Key));
                    CommerceServer.Core.Runtime.Profiles.ProfileContext ctxt = CommerceServer.Foundation.SequenceComponents.ContextProviders.CommerceSiteContexts.Profile[SiteHelper.GetSiteName()];

                    
					string dataSQLText = "SELECT *  FROM [BEK_Commerce_profiles].[dbo].[Addresses] WHERE u_address_id in (" + String.Join(",", preferredAddressIds.ToArray()) + ")";

					
                   
                    try
                    {

						using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connStr))
						{
							conn.Open();

							using (System.Data.OleDb.OleDbCommand cmdRead = new System.Data.OleDb.OleDbCommand(dataSQLText, conn))
							{
								using (System.Data.OleDb.OleDbDataReader dataReader = cmdRead.ExecuteReader())
								{
									while (dataReader.Read())
									{
										CommerceServer.Foundation.CommerceEntity entity = new CommerceServer.Foundation.CommerceEntity("Address");
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
											.Properties.Add("PreferredAddress", new CommerceServer.Foundation.CommerceRelationship(entity));
									}
									dataReader.Close();
								}
							}
						}

                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Error loading organization addresses", ex);
                    }
					
                }
            }
        }

        protected override bool CanDeleteRelatedItem(string relatedItemId)
        {
            return true;
        }
    }
}